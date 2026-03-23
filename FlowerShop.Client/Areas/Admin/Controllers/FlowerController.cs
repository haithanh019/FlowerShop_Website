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
            var categories = await _baseService.GetODataAsync<IEnumerable<CategoryDTO>>(
                "Odata/Categories", token);
            ViewBag.Categories = categories ?? new List<CategoryDTO>();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var guard = CheckAdmin();
            if (guard != null) return guard;

            var flowers = await _baseService.GetODataAsync<IEnumerable<FlowerDTO>>(
                "Odata/Flowers?$expand=FlowerImages");

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
        public async Task<IActionResult> Create(
            [FromForm] FlowerCreateDTO model,
            IFormFileCollection? FlowerImages)
        {
            var guard = CheckAdmin();
            if (guard != null) return guard;

            if (FlowerImages != null && FlowerImages.Any())
                model.FlowerImages = (ICollection<IFormFile>?)FlowerImages;

            var token = HttpContext.Session.GetString("JWToken");

            var content = BuildMultipart(model);
            var response = await _baseService.PostMultipartAsync<FlowerDTO>(
                "Odata/Flowers", content, token);

            TempData[response.Success ? "SuccessMessage" : "ErrorMessage"] =
                response.Success
                    ? $"Đã thêm hoa \"{model.FlowerName}\" thành công!"
                    : response.Message ?? "Thêm hoa thất bại.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            Guid id,
            [FromForm] FlowerUpdateDTO model,
            IFormFileCollection? FlowerImages)
        {
            var guard = CheckAdmin();
            if (guard != null) return guard;

            if (FlowerImages != null && FlowerImages.Any())
                model.FlowerImages = (ICollection<IFormFile>?)FlowerImages;

            var deleteIds = Request.Form["DeleteImageIds"].ToList();
            if (deleteIds.Any())
                model.DeleteImageIds = deleteIds.Where(x => x != null).ToList()!;

            var token = HttpContext.Session.GetString("JWToken");

            var content = BuildMultipartUpdate(model);
            var response = await _baseService.PutMultipartAsync<FlowerDTO>(
                $"Odata/Flowers({id})", content, token);

            TempData[response.Success ? "SuccessMessage" : "ErrorMessage"] =
                response.Success
                    ? $"Đã cập nhật hoa \"{model.FlowerName}\"!"
                    : response.Message ?? "Cập nhật hoa thất bại.";

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

            TempData[response.Success ? "SuccessMessage" : "ErrorMessage"] =
                response.Success ? "Đã xóa hoa thành công!" : response.Message ?? "Xóa hoa thất bại.";

            return RedirectToAction(nameof(Index));
        }

        // ── PRIVATE HELPERS ────────────────────────────────

        private static MultipartFormDataContent BuildMultipart(FlowerCreateDTO model)
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(model.FlowerName), "FlowerName");
            content.Add(new StringContent(model.Description ?? ""), "Description");
            content.Add(new StringContent(model.Price.ToString()), "Price");
            content.Add(new StringContent(model.StockQuantity.ToString()), "StockQuantity");
            content.Add(new StringContent(model.IsActive.ToString()), "IsActive");
            content.Add(new StringContent(model.CategoryID.ToString()), "CategoryID");

            if (model.FlowerImages != null)
                foreach (var file in model.FlowerImages)
                    content.Add(new StreamContent(file.OpenReadStream()), "FlowerImages", file.FileName);

            return content;
        }

        private static MultipartFormDataContent BuildMultipartUpdate(FlowerUpdateDTO model)
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(model.FlowerName), "FlowerName");
            content.Add(new StringContent(model.Description ?? ""), "Description");
            content.Add(new StringContent(model.Price.ToString()), "Price");
            content.Add(new StringContent(model.StockQuantity.ToString()), "StockQuantity");
            content.Add(new StringContent(model.IsActive.ToString()), "IsActive");
            content.Add(new StringContent(model.CategoryID.ToString()), "CategoryID");

            if (model.FlowerImages != null)
                foreach (var file in model.FlowerImages)
                    content.Add(new StreamContent(file.OpenReadStream()), "FlowerImages", file.FileName);

            if (model.DeleteImageIds != null)
                foreach (var pid in model.DeleteImageIds)
                    content.Add(new StringContent(pid), "DeleteImageIds");

            return content;
        }
    }
}
