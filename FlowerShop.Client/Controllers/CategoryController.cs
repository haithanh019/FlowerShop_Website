using FlowerShop.Application;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Client
{
    public class CategoryController : Controller
    {
        private readonly IBaseService _baseService;

        public CategoryController(IBaseService baseService)
        {
            _baseService = baseService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _baseService.GetODataAsync<IEnumerable<CategoryDTO>>("/Odata/Categories");

            if (categories == null)
            {
                ViewBag.ErrorMessage = "Không thể lấy danh sách danh mục.";
                return View(new List<CategoryDTO>());
            }
            return View(categories);
        }
    }
}
