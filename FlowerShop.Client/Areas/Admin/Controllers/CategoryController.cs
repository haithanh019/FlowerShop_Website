using FlowerShop.Application;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IBaseService _baseService;

        public CategoryController(IBaseService baseService)
        {
            _baseService = baseService;
        }
        private IActionResult? CheckAdmin()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(token) || role != "Admin")
                return RedirectToAction("Login", "Auth", new { area = "" });

            return null;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var guard = CheckAdmin();
            if (guard != null) return guard;

            var categories = await _baseService.GetODataAsync<IEnumerable<CategoryDTO>>("Odata/Categories");
            if (categories == null)
            {
                TempData["ErrorMessage"] = "Không thể tải danh sách danh mục.";
                return View(new List<CategoryDTO>());
            }
            return View(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateDTO model)
        {
            var guard = CheckAdmin();
            if (guard != null) return guard;

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var token = HttpContext.Session.GetString("JWToken");
            var response = await _baseService.PostAsync<CategoryDTO>("Odata/Categories", model, token);

            if (response.Success)
                TempData["SuccessMessage"] = $"Đã tạo danh mục \"{model.CategoryName}\" thành công!";
            else
                TempData["ErrorMessage"] = response.Message ?? "Tạo danh mục thất bại.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CategoryUpdateDTO model)
        {
            var guard = CheckAdmin();
            if (guard != null) return guard;

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var token = HttpContext.Session.GetString("JWToken");
            var response = await _baseService.PutAsync<CategoryDTO>($"Odata/Categories({id})", model, token);

            if (response.Success)
                TempData["SuccessMessage"] = $"Đã cập nhật danh mục \"{model.CategoryName}\"!";
            else
                TempData["ErrorMessage"] = response.Message ?? "Cập nhật danh mục thất bại.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var guard = CheckAdmin();
            if (guard != null) return guard;

            var token = HttpContext.Session.GetString("JWToken");
            var response = await _baseService.DeleteAsync<bool>($"Odata/Categories({id})", token);

            if (response.Success)
                TempData["SuccessMessage"] = "Đã xóa danh mục thành công!";
            else
                TempData["ErrorMessage"] = response.Message ?? "Xóa danh mục thất bại.";

            return RedirectToAction(nameof(Index));
        }
    }
}
