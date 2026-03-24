using FlowerShop.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FlowerShop.API.Controllers
{
    public class FlowersController(IFacadeService facadeService) : ODataController
    {
        private readonly IFacadeService _facadeService = facadeService;

        [EnableQuery]
        public IQueryable<FlowerDTO> Get() => _facadeService.FlowerService.GetFlowersOData();

        [EnableQuery]
        public async Task<IActionResult> Get([FromRoute] Guid key)
        {
            var result = await _facadeService.FlowerService.GetFlowerByIDAsync(key);
            return Ok(result);
        }

        public async Task<IActionResult> Post([FromForm] FlowerCreateDTO dto)
        {
            var result = await _facadeService.FlowerService.CreateFlowerAsync(dto);
            return Ok(result);
        }

        public async Task<IActionResult> Put([FromRoute] Guid key, [FromForm] FlowerUpdateDTO dto)
        {
            var result = await _facadeService.FlowerService.UpdateFlowerAsync(key, dto);
            return Ok(result);
        }

        public async Task<IActionResult> Delete([FromRoute] Guid key)
        {
            var result = await _facadeService.FlowerService.DeleteFlowerAsync(key);
            return Ok(result);
        }

    }
}
