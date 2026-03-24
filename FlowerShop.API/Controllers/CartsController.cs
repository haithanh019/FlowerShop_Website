using FlowerShop.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FlowerShop.API.Controllers
{
    public class CartsController(IFacadeService facadeService) : ODataController
    {
        private readonly IFacadeService _facadeService = facadeService;

        [EnableQuery]
        public async Task<IActionResult> Get([FromRoute] Guid key)
        {
            var result = await _facadeService.CartService.GetCartByUserIDAsync(key);
            return Ok(result);
        }
    }
}
