using FlowerShop.Utility;

namespace FlowerShop.Application
{
    public interface ICartItemService
    {
        Task<ApiResponse<CartItemDTO>> AddToCartAsync(CartItemCreateDTO dto);
        Task<ApiResponse<CartItemDTO>> UpdateCartItemAsync(Guid id, CartItemUpdateDTO dto);
        Task<ApiResponse<bool>> RemoveCartItemAsync(Guid id);
    }
}
