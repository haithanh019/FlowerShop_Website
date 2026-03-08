using FlowerShop.Application;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Client.Controllers
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

            if (categories == null )
            {
                ViewBag.ErrorMessage = "Không thể lấy danh sách danh mục.";
                return View(new List<CategoryDTO>());
            }         
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateDTO model)
        {
            if (!ModelState.IsValid) return View(model);

            var token = HttpContext.Session.GetString("JWToken");

            var response = await _baseService.PostAsync<CategoryDTO>("Odata/Categories", model, token);

            if (response.Success)
            {
                TempData["SuccessMessage"] = "Tạo danh mục thành công!";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Lỗi khi tạo danh mục.");
            return View(model);
        }
    }
}
