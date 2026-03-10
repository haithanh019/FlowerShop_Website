using FlowerShop.Utility;

namespace FlowerShop.Application
{
    public interface IOrderService
    {
        IQueryable<OrderDTO> GetOrdersOData();
        Task<ApiResponse<OrderDTO>> GetOrderByIDAsync(Guid id);
        Task<ApiResponse<IEnumerable<OrderDTO>>> GetOrdersByUserAsync(Guid id);
        Task<ApiResponse<OrderDTO>> CreateOrderFromCartAsync(Guid id, OrderCreateDTO dto);
        Task<ApiResponse<OrderDTO>> UpdateOrderStatusAsync(Guid id, OrderUpdateDTO dto);
        Task<ApiResponse<bool>> CancelOrderAsync(Guid id);
    }
}
