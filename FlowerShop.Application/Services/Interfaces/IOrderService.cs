using FlowerShop.Utility;

namespace FlowerShop.Application
{
    public interface IOrderService
    {
        IQueryable<OrderDTO> GetOrdersOData();
        Task<ApiResult<OrderDTO>> GetOrderByIDAsync(Guid id);
        Task<ApiResult<IEnumerable<OrderDTO>>> GetOrdersByUserAsync(Guid id);
        Task<ApiResult<OrderDTO>> CreateOrderFromCartAsync(Guid id, OrderCreateDTO dto);
        Task<ApiResult<OrderDTO>> UpdateOrderStatusAsync(Guid id, OrderUpdateDTO dto);
        Task<ApiResult<bool>> CancelOrderAsync(Guid id);
    }
}
