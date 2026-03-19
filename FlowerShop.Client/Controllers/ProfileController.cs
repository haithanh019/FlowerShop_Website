using FlowerShop.Application;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Client
{
    public class ProfileController : Controller
    {
        private readonly IBaseService _baseService;

        public ProfileController(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var userId = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var response = await _baseService.GetAsync<UserDTO>($"Odata/Users/{userId}", token);
            if (response == null || !response.Success || response.Data == null)
            {
                TempData["ErrorMessage"] = "Không tải được thông tin tài khoản.";
                return View(new ProfileViewModel());
            }

            var vm = new ProfileViewModel
            {
                UserID = response.Data.UserID,
                Email = response.Data.Email ?? string.Empty,
                FullName = response.Data.FullName ?? string.Empty,
                PhoneNumber = response.Data.PhoneNumber ?? string.Empty,
                Role = response.Data.Role ?? string.Empty
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            var token = HttpContext.Session.GetString("JWToken");
            var userId = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu cập nhật không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var updateDto = new UserUpdateDTO
            {
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber
            };

            var response = await _baseService.PutAsync<UserDTO>($"/Odata/Users/{userId}", updateDto, token);

            if (response.Success && response.Data != null)
            {
                HttpContext.Session.SetString("FullName", response.Data.FullName ?? string.Empty);
                HttpContext.Session.SetString("Email", response.Data.Email ?? string.Empty);
                TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Cập nhật hồ sơ thất bại.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var userId = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var response = await _baseService.DeleteAsync<bool>($"/Odata/Users/{userId}", token);

            if (response.Success)
            {
                HttpContext.Session.Clear();
                TempData["SuccessMessage"] = "Tài khoản đã được xóa.";
                return RedirectToAction("Register", "Auth");
            }

            TempData["ErrorMessage"] = response.Message ?? "Xóa tài khoản thất bại.";
            return RedirectToAction(nameof(Index));
        }
    }
}
