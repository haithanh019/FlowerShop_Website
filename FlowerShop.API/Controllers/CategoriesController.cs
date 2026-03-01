using FlowerShop.Application;
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
    }
}
