using FlowerShop.Utility;

namespace FlowerShop.Application
{
    public interface ICartService
    {
        Task<ApiResponse<CartDTO>> CreateCartAsync(Guid id);
        Task<ApiResponse<CartDTO>> GetCartByUserIDAsync(Guid id);
    }
}
