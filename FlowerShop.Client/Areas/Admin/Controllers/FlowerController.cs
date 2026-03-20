using FlowerShop.Application;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FlowerController : Controller
    {
        private readonly IBaseService _baseService;

        public FlowerController(IBaseService baseService)
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

        private async Task LoadCategoriesAsync()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var categories = await _baseService.GetODataAsync<IEnumerable<CategoryDTO>>("Odata/Categories", token);
            ViewBag.Categories = categories ?? new List<CategoryDTO>();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var guard = CheckAdmin();
            if (guard != null) return guard;

            var flowers = await _baseService.GetODataAsync<IEnumerable<FlowerDTO>>("Odata/Flowers");
            if (flowers == null)
            {
                TempData["ErrorMessage"] = "Không thể tải danh sách hoa.";
                flowers = new List<FlowerDTO>();
            }

            await LoadCategoriesAsync();
            return View(flowers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FlowerCreateDTO model)
        {
            var guard = CheckAdmin();
            if (guard != null) return guard;

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                return RedirectToAction(nameof(Index));
            }

            var token = HttpContext.Session.GetString("JWToken");
            var response = await _baseService.PostAsync<FlowerDTO>("Odata/Flowers", model, token);

            if (response.Success)
                TempData["SuccessMessage"] = $"Đã thêm hoa \"{model.FlowerName}\" thành công!";
            else
                TempData["ErrorMessage"] = response.Message ?? "Thêm hoa thất bại.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, FlowerUpdateDTO model)
        {
            var guard = CheckAdmin();
            if (guard != null) return guard;

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
                return RedirectToAction(nameof(Index));
            }

            var token = HttpContext.Session.GetString("JWToken");
            var response = await _baseService.PutAsync<FlowerDTO>($"Odata/Flowers({id})", model, token);

            if (response.Success)
                TempData["SuccessMessage"] = $"Đã cập nhật hoa \"{model.FlowerName}\"!";
            else
                TempData["ErrorMessage"] = response.Message ?? "Cập nhật hoa thất bại.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var guard = CheckAdmin();
            if (guard != null) return guard;

            var token = HttpContext.Session.GetString("JWToken");
            var response = await _baseService.DeleteAsync<bool>($"Odata/Flowers({id})", token);

            if (response.Success)
                TempData["SuccessMessage"] = "Đã xóa hoa thành công!";
            else
                TempData["ErrorMessage"] = response.Message ?? "Xóa hoa thất bại.";

            return RedirectToAction(nameof(Index));
        }
    }
}
