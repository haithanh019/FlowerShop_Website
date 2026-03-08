using FlowerShop.Utility;

namespace FlowerShop.Application
{
    public interface ICartService
    {
        Task<ApiResponse<CartDTO>> GetCartByUserIDAsync(Guid id);
    }
}
