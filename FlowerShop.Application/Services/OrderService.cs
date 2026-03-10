using AutoMapper;
using AutoMapper.QueryableExtensions;
using FlowerShop.Domain;
using FlowerShop.Infrastructure;
using FlowerShop.Utility;

namespace FlowerShop.Application
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IQueryable<OrderDTO> GetOrdersOData()
        {
            return _unitOfWork.OrderRepository.GetQuery()
                .ProjectTo<OrderDTO>(_mapper.ConfigurationProvider);
        }

        public async Task<ApiResponse<OrderDTO>> GetOrderByIDAsync(Guid id)
        {
            var order = await _unitOfWork.OrderRepository.GetByAsync(
                o => o.OrderID == id,
                includeProperties: "OrderItems,OrderItems.Flower,OrderItems.Flower.FlowerImages"
            );
            if (order == null)
                throw new NotFoundException("Không tìm thấy đơn hàng.");

            return new ApiResponse<OrderDTO>(_mapper.Map<OrderDTO>(order));
        }

        public async Task<ApiResponse<IEnumerable<OrderDTO>>> GetOrdersByUserAsync(Guid userId)
        {
            var orders = await _unitOfWork.OrderRepository.FindAsync(
                o => o.UserID == userId,
                includeProperties: "OrderItems,OrderItems.Flower,OrderItems.Flower.FlowerImages"
            );
            return new ApiResponse<IEnumerable<OrderDTO>>(_mapper.Map<IEnumerable<OrderDTO>>(orders));
        }

        public async Task<ApiResponse<OrderDTO>> CreateOrderFromCartAsync(Guid userId, OrderCreateDTO dto)
        {
            var cart = await _unitOfWork.CartRepository.GetByAsync(
                c => c.CartID == dto.CartID && c.UserID == userId,
                trackChanges: true,
                includeProperties: "CartItems,CartItems.Flower,CartItems.Flower.FlowerImages"
            );
            if (cart == null)
                throw new NotFoundException("Không tìm thấy giỏ hàng.");
            if (!cart.CartItems.Any())
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
                OrderStatus = OrderStatus.Pending
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

            return new ApiResponse<OrderDTO>(_mapper.Map<OrderDTO>(created), "Đặt hàng thành công");
        }

        public async Task<ApiResponse<OrderDTO>> UpdateOrderStatusAsync(Guid id, OrderUpdateDTO dto)
        {
            var order = await _unitOfWork.OrderRepository.GetByAsync(
                o => o.OrderID == id,
                trackChanges: true,
                includeProperties: "OrderItems,OrderItems.Flower,OrderItems.Flower.FlowerImages"
            );
            if (order == null)
                throw new NotFoundException("Không tìm thấy đơn hàng.");

            if (order.OrderStatus == OrderStatus.Completed || order.OrderStatus == OrderStatus.Cancelled)
                throw new BadRequestException($"Đơn hàng đã '{order.OrderStatus}' không thể thay đổi trạng thái.");

            order.OrderStatus = dto.OrderStatus;
            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveAsync();

            return new ApiResponse<OrderDTO>(_mapper.Map<OrderDTO>(order), "Cập nhật trạng thái đơn hàng thành công");
        }

        public async Task<ApiResponse<bool>> CancelOrderAsync(Guid id)
        {
            var order = await _unitOfWork.OrderRepository.GetByAsync(
                o => o.OrderID == id,
                trackChanges: true,
                includeProperties: "OrderItems,OrderItems.Flower"
            );
            if (order == null)
                throw new NotFoundException("Không tìm thấy đơn hàng.");

            if (order.OrderStatus != OrderStatus.Pending && order.OrderStatus != OrderStatus.Confirmed)
                throw new BadRequestException("Chỉ có thể hủy đơn hàng ở trạng thái Pending hoặc Confirmed.");

            foreach (var item in order.OrderItems)
            {
                item.Flower!.StockQuantity += item.Quantity;
                _unitOfWork.FlowerRepository.Update(item.Flower);
            }

            order.OrderStatus = OrderStatus.Cancelled;
            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveAsync();

            return new ApiResponse<bool>(true, "Hủy đơn hàng thành công");
        }
    }
}
