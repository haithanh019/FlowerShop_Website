using FlowerShop.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FlowerShop.API.Controllers
{
    public class CartItemsController(IFacadeService facadeService) : ODataController
    {
        private readonly IFacadeService _facadeService = facadeService;

        public async Task<IActionResult> Post([FromBody] CartItemCreateDTO dto)
        {
            var result = await _facadeService.CartItemService.AddToCartAsync(dto);
            return Ok(result);
        }

        public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] CartItemUpdateDTO dto)
        {
            var result = await _facadeService.CartItemService.UpdateCartItemAsync(key, dto);
            return Ok(result);
        }

        public async Task<IActionResult> Delete([FromRoute] Guid key)
        {
            var result = await _facadeService.CartItemService.RemoveCartItemAsync(key);
            return Ok(result);
        }
    }
}
