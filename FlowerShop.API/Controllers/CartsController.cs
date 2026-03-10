using FlowerShop.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FlowerShop.API
{
    public class CartsController : ODataController
    {
        private readonly IFacadeService _facadeService;
        public CartsController(IFacadeService facadeService)
        {
            _facadeService = facadeService;
        }

        [HttpGet("Odata/Carts")]
        public async Task<IActionResult> GetByUser([FromQuery] Guid userId)
        {
            var result = await _facadeService.CartService.GetCartByUserIDAsync(userId);
            return Ok(result);
        }
    }
}
