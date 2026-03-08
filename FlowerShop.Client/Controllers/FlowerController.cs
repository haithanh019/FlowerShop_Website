using FlowerShop.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlowerShop.Client.Controllers
{
    public class FlowerController : Controller
    {
        private readonly IBaseService _baseService;

        public FlowerController(IBaseService baseService)
        {
            _baseService = baseService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var flowers = await _baseService.GetODataAsync<IEnumerable<FlowerDTO>>("/Odata/Flowers");

            if (flowers == null)
            {
                ViewBag.ErrorMessage = "Không thể lấy danh sách hoa.";
                return View(new List<FlowerDTO>());
            }
            return View(flowers);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Auth");

            var categoryResponse = await _baseService.GetODataAsync<IEnumerable<CategoryDTO>>("/Odata/Categories", token);

            if (categoryResponse != null)
            {
                ViewBag.Categories = new SelectList(categoryResponse, "CategoryID", "Name");
            }
            else
            {
                ViewBag.Categories = new SelectList(new List<CategoryDTO>(), "CategoryID", "Name");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(FlowerCreateDTO model)
        {
            if (!ModelState.IsValid) return View(model);

            var token = HttpContext.Session.GetString("JWToken");

            var response = await _baseService.PostAsync<FlowerDTO>("Odata/Flowers", model, token);

            if (response.Success)
            {
                TempData["SuccessMessage"] = "Tạo hoa thành công!";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Lỗi khi tạo hoa.");
            return View(model);
        }
    }
}
