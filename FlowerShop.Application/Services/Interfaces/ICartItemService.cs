using FlowerShop.Utility;

namespace FlowerShop.Application
{
    public interface ICartItemService
    {
        Task<ApiResult<CartItemDTO>> AddToCartAsync(CartItemCreateDTO dto);
        Task<ApiResult<CartItemDTO>> UpdateCartItemAsync(Guid id, CartItemUpdateDTO dto);
        Task<ApiResult<bool>> RemoveCartItemAsync(Guid id);
    }
}
