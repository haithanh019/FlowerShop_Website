using FlowerShop.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FlowerShop.API
{
    public class CategoriesController : ODataController
    {
        private readonly IFacadeService _facadeService;
        public CategoriesController(IFacadeService facadeService)
        {
            _facadeService = facadeService;
        }
        [EnableQuery]
        public IQueryable<CategoryDTO> Get() => _facadeService.CategoryService.GetCategoriesOData();

        [EnableQuery]
        public async Task<IActionResult> Get([FromRoute] Guid key)
        {
            var result = await _facadeService.CategoryService.GetCategoryByIDAsync(key);
            return Ok(result);
        }

        public async Task<IActionResult> Post([FromBody] CategoryCreateDTO dto)
        {
            var result = await _facadeService.CategoryService.CreateCategoryAsync(dto);
            return Ok(result);
        }

        public async Task<IActionResult> Put([FromRoute] Guid key, [FromBody] CategoryUpdateDTO dto)
        {
            var result = await _facadeService.CategoryService.UpdateCategoryAsync(key, dto);
            return Ok(result);
        }

        public async Task<IActionResult> Delete([FromRoute] Guid key)
        {
            var result = await _facadeService.CategoryService.DeleteCategoryAsync(key);
            return Ok(result);
        }
    }
}
