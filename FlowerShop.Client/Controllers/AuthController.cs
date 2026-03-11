using FlowerShop.Application;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Client.Controllers
{
    public class AuthController : Controller
    {
        private readonly IBaseService _baseService;

        public AuthController(IBaseService baseService)
        {
            _baseService = baseService;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginDTO model)
        {
            if (!ModelState.IsValid) return View(model);

            var response = await _baseService.PostAsync<UserDTO>("api/Users/Login", model);

            if (response.Success && response.Data != null)
            {
                // Save Session
                HttpContext.Session.SetString("JWToken", response.Data.Token);
                HttpContext.Session.SetString("UserID", response.Data.UserID.ToString());
                HttpContext.Session.SetString("UserName", response.Data.FullName);
                HttpContext.Session.SetString("Email", response.Data.Email);
                HttpContext.Session.SetString("Role", response.Data.Role);
                TempData["SuccessMessage"] = "Đăng nhập thành công!";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Đăng nhập thất bại.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterDTO model)
        {
            if (!ModelState.IsValid) return View(model);

            var response = await _baseService.PostAsync<UserDTO>("api/Users/Register", model);

            if (response.Success)
            {
                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Đăng ký thất bại.");
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
