using FlowerShop.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FlowerShop.API
{
    public class FlowersController : ODataController
    {
        private readonly IFacadeService _facadeService;
        public FlowersController(IFacadeService facadeService)
        {
            _facadeService = facadeService;
        }
        [EnableQuery]
        public IQueryable<FlowerDTO> Get() => _facadeService.FlowerService.GetFlowersOData();

        [EnableQuery]
        public async Task<IActionResult> Get([FromRoute] Guid key)
        {
            var result = await _facadeService.FlowerService.GetFlowerByIDAsync(key);
            return Ok(result);
        }

        public async Task<IActionResult> Post([FromBody] FlowerCreateDTO dto)
        {
            var result = await _facadeService.FlowerService.CreateFlowerAsync(dto);
            return Ok(result);
        }

        public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] FlowerUpdateDTO dto)
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
