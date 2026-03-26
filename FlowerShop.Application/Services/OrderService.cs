using AutoMapper;
using AutoMapper.QueryableExtensions;
using FlowerShop.Domain;
using FlowerShop.Infrastructure;
using FlowerShop.Utility;

namespace FlowerShop.Application
{
    public class OrderService(IUnitOfWork unitOfWork, IMapper mapper) : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        public IQueryable<OrderDTO> GetOrdersOData()
        {
            return _unitOfWork.OrderRepository.GetQuery()
                .ProjectTo<OrderDTO>(_mapper.ConfigurationProvider);
        }

        public async Task<ApiResult<OrderDTO>> GetOrderByIDAsync(Guid id)
        {
            var order = await _unitOfWork.OrderRepository.GetByAsync(
                o => o.OrderID == id,
                includeProperties: "OrderItems,OrderItems.Flower,OrderItems.Flower.FlowerImages"
            ) ?? throw new NotFoundException("Không tìm thấy đơn hàng.");
            return new ApiResult<OrderDTO>(_mapper.Map<OrderDTO>(order));
        }

        public async Task<ApiResult<IEnumerable<OrderDTO>>> GetOrdersByUserAsync(Guid id)
        {
            var orders = await _unitOfWork.OrderRepository.FindAsync(
                o => o.UserID == id,
                includeProperties: "OrderItems,OrderItems.Flower,OrderItems.Flower.FlowerImages"
            );
            return new ApiResult<IEnumerable<OrderDTO>>(_mapper.Map<IEnumerable<OrderDTO>>(orders));
        }

        public async Task<ApiResult<OrderDTO>> CreateOrderFromCartAsync(Guid userId, OrderCreateDTO dto)
        {
            var cart = await _unitOfWork.CartRepository.GetByAsync(
                c => c.CartID == dto.CartID && c.UserID == userId,
                trackChanges: true,
                includeProperties: "CartItems,CartItems.Flower,CartItems.Flower.FlowerImages"
            ) ?? throw new NotFoundException("Không tìm thấy giỏ hàng.");
            if (cart.CartItems.Count == 0)
                throw new BadRequestException("Giỏ hàng đang trống.");

            foreach (var item in cart.CartItems)
            {
                if (!item.Flower!.IsActive)
                    throw new BadRequestException($"Hoa '{item.Flower.FlowerName}' hiện không còn kinh doanh.");
                if (item.Flower.StockQuantity < item.Quantity)
                    throw new BadRequestException(
                        $"Hoa '{item.Flower.FlowerName}' chỉ còn {item.Flower.StockQuantity} sản phẩm trong kho.");
            }

            var subtotal = cart.CartItems.Sum(ci => ci.UnitPrice * ci.Quantity);
            var totalAmount = subtotal + dto.ShippingFee;

            var order = new Order
            {
                UserID = userId,
                ShippingAddress = dto.ShippingAddress,
                PhoneNumber = dto.PhoneNumber,
                ShippingFee = dto.ShippingFee,
                Subtotal = subtotal,
                TotalAmount = totalAmount,
                OrderStatus = OrderStatus.Processing
            };

            foreach (var item in cart.CartItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    FlowerID = item.FlowerID,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    LineTotal = item.UnitPrice * item.Quantity
                });

                item.Flower!.StockQuantity -= item.Quantity;
                _unitOfWork.FlowerRepository.Update(item.Flower);
            }

            _unitOfWork.CartItemRepository.DeleteRange(cart.CartItems);

            await _unitOfWork.OrderRepository.AddAsync(order);
            await _unitOfWork.SaveAsync();

            var created = await _unitOfWork.OrderRepository.GetByAsync(
                o => o.OrderID == order.OrderID,
                includeProperties: "OrderItems,OrderItems.Flower,OrderItems.Flower.FlowerImages"
            );

            return new ApiResult<OrderDTO>(_mapper.Map<OrderDTO>(created), "Đặt hàng thành công");
        }

        public async Task<ApiResult<OrderDTO>> UpdateOrderStatusAsync(Guid id, OrderUpdateDTO dto)
        {
            var order = await _unitOfWork.OrderRepository.GetByAsync(
                o => o.OrderID == id,
                trackChanges: true,
                includeProperties: "OrderItems,OrderItems.Flower,OrderItems.Flower.FlowerImages"
            ) ?? throw new NotFoundException("Không tìm thấy đơn hàng.");
            if (order.OrderStatus == OrderStatus.Completed || order.OrderStatus == OrderStatus.Cancelled)
                throw new BadRequestException($"Đơn hàng đã '{order.OrderStatus}' không thể thay đổi trạng thái.");

            order.OrderStatus = dto.OrderStatus;
            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveAsync();

            return new ApiResult<OrderDTO>(_mapper.Map<OrderDTO>(order), "Cập nhật trạng thái đơn hàng thành công");
        }

        public async Task<ApiResult<bool>> CancelOrderAsync(Guid id)
        {
            var order = await _unitOfWork.OrderRepository.GetByAsync(
                o => o.OrderID == id,
                trackChanges: true,
                includeProperties: "OrderItems,OrderItems.Flower"
            ) ?? throw new NotFoundException("Không tìm thấy đơn hàng.");
            if (order.OrderStatus != OrderStatus.Processing && order.OrderStatus != OrderStatus.Confirmed)
                throw new BadRequestException("Chỉ có thể hủy đơn hàng ở trạng thái Processing hoặc Confirmed.");

            foreach (var item in order.OrderItems)
            {
                item.Flower!.StockQuantity += item.Quantity;
                _unitOfWork.FlowerRepository.Update(item.Flower);
            }

            order.OrderStatus = OrderStatus.Cancelled;
            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveAsync();

            return new ApiResult<bool>(true, "Hủy đơn hàng thành công");
        }
    }
}
