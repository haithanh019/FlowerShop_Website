using FlowerShop.Utility;

namespace FlowerShop.Application
{
    public interface ICartService
    {
        Task<ApiResult<CartDTO>> GetCartByUserIDAsync(Guid id);
    }
}
