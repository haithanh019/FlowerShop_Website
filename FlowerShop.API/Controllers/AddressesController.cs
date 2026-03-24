using FlowerShop.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FlowerShop.API.Controllers
{
    public class AddressesController(IFacadeService facadeService) : ODataController
    {
        private readonly IFacadeService _facadeService = facadeService;

        [EnableQuery]
        public IQueryable<AddressDTO> Get() => _facadeService.AddressService.GetAddressesOData();

        [EnableQuery]
        public async Task<IActionResult> Get([FromRoute] Guid key)
        {
            var result = await _facadeService.AddressService.GetAddressByIDAsync(key);
            return Ok(result);
        }

        public async Task<IActionResult> Post([FromBody] AddressCreateDTO dto)
        {
            var result = await _facadeService.AddressService.CreateAddressAsync(dto);
            return Ok(result);
        }

        public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] AddressUpdateDTO dto)
        {
            var result = await _facadeService.AddressService.UpdateAddressAsync(key, dto);
            return Ok(result);
        }

        public async Task<IActionResult> Delete([FromRoute] Guid key)
        {
            var result = await _facadeService.AddressService.DeleteAddressAsync(key);
            return Ok(result);
        }
    }
}
