using FlowerShop.Utility;

namespace FlowerShop.Application
{
    public interface IPaymentService
    {
        Task<ApiResult<PaymentDTO>> CreateCODPaymentAsync(Guid orderID, decimal amount);
        Task<ApiResult<PayOSCheckoutResponseDTO>> CreatePayOSPaymentAsync(Guid orderID, decimal amount, string returnUrl, string cancelUrl);
        Task<ApiResult<bool>> HandlePayOSWebhookAsync(PayOSWebhookDTO webhook);
        Task<ApiResult<PaymentDTO>> GetPaymentByOrderIDAsync(Guid orderID);
        Task<ApiResult<PaymentDTO>> UpdatePaymentStatusAsync(Guid id, PaymentUpdateDTO dto);
        Task<ApiResult<bool>> CancelPendingPaymentAsync(Guid orderID);
    }
}
