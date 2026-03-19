using FlowerShop.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
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

        [EnableQuery]
        public async Task<IActionResult> Get([FromRoute] Guid key)
        {
            var result = await _facadeService.CartService.GetCartByUserIDAsync(key);
            return Ok(result);
        }
    }
}
