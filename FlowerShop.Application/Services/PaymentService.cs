using AutoMapper;
using FlowerShop.Domain;
using FlowerShop.Infrastructure;
using FlowerShop.Utility;
using Microsoft.Extensions.Configuration;
using PayOS;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;

namespace FlowerShop.Application
{
    public class PaymentService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration) : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly PayOSClient _payOSClient = new(
            configuration["PayOS:ApiKey"]!,
            configuration["PayOS:ClientId"]!,
            configuration["PayOS:ChecksumKey"]!
        );

        // ─── COD ─────────────────────────────────────────────────────────────

        public async Task<ApiResult<PaymentDTO>> CreateCODPaymentAsync(Guid orderID, decimal amount)
        {
            var order = await _unitOfWork.OrderRepository.GetByAsync(
                o => o.OrderID == orderID,
                includeProperties: "Payment"
            ) ?? throw new NotFoundException("Không tìm thấy đơn hàng.");

            if (order.Payment != null)
                throw new BadRequestException("Đơn hàng đã có thông tin thanh toán.");

            var payment = new Payment
            {
                OrderID = orderID,
                PaymentMethod = PaymentMethod.CashOnDelivery,
                PaymentStatus = PaymentStatus.Pending,
                Amount = amount
            };

            await _unitOfWork.PaymentRepository.AddAsync(payment);
            await _unitOfWork.SaveAsync();

            return new ApiResult<PaymentDTO>(_mapper.Map<PaymentDTO>(payment), "Tạo thanh toán COD thành công.");
        }

        // ─── PayOS ───────────────────────────────────────────────────────────

        public async Task<ApiResult<PayOSCheckoutResponseDTO>> CreatePayOSPaymentAsync(
            Guid orderID, decimal amount, string returnUrl, string cancelUrl)
        {
            var order = await _unitOfWork.OrderRepository.GetByAsync(
                o => o.OrderID == orderID,
                trackChanges: true,
                includeProperties: "OrderItems,OrderItems.Flower,Payment"
            ) ?? throw new NotFoundException("Không tìm thấy đơn hàng.");

            if (order.Payment != null && order.Payment.PaymentStatus == PaymentStatus.Paid)
                throw new BadRequestException("Đơn hàng đã được thanh toán.");

            var orderCode = long.Parse(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()[^9..]);

            var paymentRequest = new CreatePaymentLinkRequest
            {
                OrderCode = orderCode,
                Amount = (int)order.TotalAmount,
                Description = $"Thanh toan #{orderCode}",
                ReturnUrl = returnUrl,
                CancelUrl = cancelUrl,
                Items = order.OrderItems.Select(i => new PaymentLinkItem
                {
                    Name = i.Flower!.FlowerName,
                    Quantity = i.Quantity,
                    Price = (int)i.UnitPrice
                }).ToList()
            };

            var paymentLink = await _payOSClient.PaymentRequests.CreateAsync(paymentRequest);

            if (order.Payment == null)
            {
                var payment = new Payment
                {
                    OrderID = orderID,
                    PaymentMethod = PaymentMethod.PayOS,
                    PaymentStatus = PaymentStatus.Pending,
                    Amount = order.TotalAmount,
                    PayOSOrderCode = orderCode,
                    PaymentUrl = paymentLink.CheckoutUrl
                };
                await _unitOfWork.PaymentRepository.AddAsync(payment);
                await _unitOfWork.SaveAsync();

                return new ApiResult<PayOSCheckoutResponseDTO>(new PayOSCheckoutResponseDTO
                {
                    PaymentID = payment.PaymentID,
                    OrderID = orderID,
                    OrderCode = orderCode,
                    PaymentUrl = paymentLink.CheckoutUrl
                }, "Tạo link thanh toán PayOS thành công.");
            }
            else
            {
                order.Payment.PayOSOrderCode = orderCode;
                order.Payment.PaymentUrl = paymentLink.CheckoutUrl;
                order.Payment.PaymentStatus = PaymentStatus.Pending;
                _unitOfWork.PaymentRepository.Update(order.Payment);
                await _unitOfWork.SaveAsync();

                return new ApiResult<PayOSCheckoutResponseDTO>(new PayOSCheckoutResponseDTO
                {
                    PaymentID = order.Payment.PaymentID,
                    OrderID = orderID,
                    OrderCode = orderCode,
                    PaymentUrl = paymentLink.CheckoutUrl
                }, "Tạo lại link thanh toán PayOS thành công.");
            }
        }

        // ─── Webhook ─────────────────────────────────────────────────────────

        public async Task<ApiResult<bool>> HandlePayOSWebhookAsync(PayOSWebhookDTO webhook)
        {
            if (webhook.Data == null)
                throw new BadRequestException("Dữ liệu webhook không hợp lệ.");

            var webhookToVerify = new Webhook
            {
                Code = webhook.Code,
                Description = webhook.Description,
                Success = webhook.Success,
                Signature = webhook.Signature,
                Data = new WebhookData
                {
                    OrderCode = webhook.Data.OrderCode,
                    Amount = webhook.Data.Amount,
                    Description = webhook.Data.Description,
                    AccountNumber = webhook.Data.AccountNumber,
                    Reference = webhook.Data.Reference,
                    TransactionDateTime = webhook.Data.TransactionDateTime,
                    Currency = "VND",
                    PaymentLinkId = webhook.Data.PaymentLinkId,
                    Code = webhook.Data.Code,
                    Description2 = webhook.Data.Desc,
                    CounterAccountBankId = webhook.Data.CounterAccountBankId ?? string.Empty,
                    CounterAccountBankName = webhook.Data.CounterAccountBankName ?? string.Empty,
                    CounterAccountName = webhook.Data.CounterAccountName ?? string.Empty,
                    CounterAccountNumber = webhook.Data.CounterAccountNumber ?? string.Empty,
                    VirtualAccountName = webhook.Data.VirtualAccountName ?? string.Empty,
                    VirtualAccountNumber = webhook.Data.VirtualAccountNumber ?? string.Empty
                }
            };

            WebhookData verifiedData;
            try
            {
                verifiedData = await _payOSClient.Webhooks.VerifyAsync(webhookToVerify);
            }
            catch
            {
                throw new BadRequestException("Chữ ký webhook không hợp lệ.");
            }

            var payment = await _unitOfWork.PaymentRepository.GetByAsync(
                p => p.PayOSOrderCode == verifiedData.OrderCode,
                trackChanges: true,
                includeProperties: "Order"
            );

            if (payment == null)
                return new ApiResult<bool>(true, "Không tìm thấy payment, bỏ qua.");

            if (payment.PaymentStatus == PaymentStatus.Paid)
                return new ApiResult<bool>(true, "Đã xử lý trước đó.");

            if (verifiedData.Code == "00")
            {
                payment.PaymentStatus = PaymentStatus.Paid;
                payment.TransactionID = verifiedData.Reference;
                payment.PaidAt = DateTime.UtcNow;
                payment.WebhookReceivedAt = DateTime.UtcNow;

                if (payment.Order != null && payment.Order.OrderStatus == OrderStatus.Processing)
                    payment.Order.OrderStatus = OrderStatus.Confirmed;
            }
            else
            {
                payment.PaymentStatus = PaymentStatus.Cancelled;
                payment.WebhookReceivedAt = DateTime.UtcNow;
            }

            _unitOfWork.PaymentRepository.Update(payment);
            await _unitOfWork.SaveAsync();

            return new ApiResult<bool>(true, "Webhook xử lý thành công.");
        }


        // ─── Query & Update ───────────────────────────────────────────────────

        public async Task<ApiResult<PaymentDTO>> GetPaymentByOrderIDAsync(Guid orderID)
        {
            var payment = await _unitOfWork.PaymentRepository.GetByAsync(
                p => p.OrderID == orderID
            ) ?? throw new NotFoundException("Không tìm thấy thông tin thanh toán.");

            return new ApiResult<PaymentDTO>(_mapper.Map<PaymentDTO>(payment));
        }

        public async Task<ApiResult<PaymentDTO>> UpdatePaymentStatusAsync(Guid id, PaymentUpdateDTO dto)
        {
            var payment = await _unitOfWork.PaymentRepository.GetByAsync(
                p => p.PaymentID == id,
                trackChanges: true
            ) ?? throw new NotFoundException("Không tìm thấy thanh toán.");

            if (payment.PaymentStatus == PaymentStatus.Paid)
                throw new BadRequestException("Thanh toán đã hoàn tất, không thể thay đổi.");

            payment.PaymentStatus = dto.PaymentStatus;

            if (dto.TransactionID != null)
                payment.TransactionID = dto.TransactionID;

            if (dto.PaidAt.HasValue)
                payment.PaidAt = dto.PaidAt;

            if (dto.WebhookReceivedAt.HasValue)
                payment.WebhookReceivedAt = dto.WebhookReceivedAt;

            _unitOfWork.PaymentRepository.Update(payment);
            await _unitOfWork.SaveAsync();

            return new ApiResult<PaymentDTO>(_mapper.Map<PaymentDTO>(payment), "Cập nhật thanh toán thành công.");
        }
    }
}
