using FlowerShop.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FlowerShop.API
{
    public class OrdersController : ODataController
    {
        private readonly IFacadeService _facadeService;
        public OrdersController(IFacadeService facadeService)
        {
            _facadeService = facadeService;
        }

        [EnableQuery]
        public IQueryable<OrderDTO> Get() => _facadeService.OrderService.GetOrdersOData();

        [EnableQuery]
        public async Task<IActionResult> Get([FromRoute] Guid key)
        {
            var result = await _facadeService.OrderService.GetOrderByIDAsync(key);
            return Ok(result);
        }

        public async Task<IActionResult> Post([FromQuery] Guid id, [FromBody] OrderCreateDTO dto)
        {
            var result = await _facadeService.OrderService.CreateOrderFromCartAsync(id, dto);
            return Ok(result);
        }

        public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] OrderUpdateDTO dto)
        {
            var result = await _facadeService.OrderService.UpdateOrderStatusAsync(key, dto);
            return Ok(result);
        }

        public async Task<IActionResult> Delete([FromRoute] Guid key)
        {
            var result = await _facadeService.OrderService.CancelOrderAsync(key);
            return Ok(result);
        }
    }
}
