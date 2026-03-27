using FlowerShop.Application;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PaymentController(IBaseService baseService) : Controller
    {
        private readonly IBaseService _baseService = baseService;

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

            var token = HttpContext.Session.GetString("JWToken");

            var payments = await _baseService.GetODataAsync<IEnumerable<PaymentDTO>>(
                "Odata/Payments?$orderby=CreatedAt desc", token);

            return View(payments ?? new List<PaymentDTO>());
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] PaymentUpdateDTO dto)
        {
            var guard = CheckAdmin();
            if (guard != null) return Json(new { success = false, message = "Không có quyền." });

            var token = HttpContext.Session.GetString("JWToken");

            var res = await _baseService.PutAsync<PaymentDTO>(
                $"api/Payments/{id}", dto, token);

            return Json(new
            {
                success = res.Success,
                message = res.Success ? "Cập nhật thành công." : res.Message ?? "Cập nhật thất bại."
            });
        }
    }
}