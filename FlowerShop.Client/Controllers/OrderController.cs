using FlowerShop.Application;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Client.Controllers
{
    public class OrderController(IBaseService baseService) : Controller
    {
        private readonly IBaseService _baseService = baseService;

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var userID = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userID))
                return RedirectToAction("Login", "Auth");

            var res = await _baseService.GetAsync<IEnumerable<OrderDTO>>(
                $"Odata/Orders({userID})", token);

            var orders = res.Success && res.Data != null
                ? res.Data.OrderByDescending(o => o.CreatedAt).ToList()
                : new List<OrderDTO>();

            return View(orders);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> CancelOrder(Guid id)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
                return Json(new { success = false, message = "Chưa đăng nhập." });

            var res = await _baseService.DeleteAsync<bool>(
                $"Odata/Orders({id})", token);

            return Json(new
            {
                success = res.Success,
                message = res.Success ? "Đã hủy đơn hàng." : res.Message ?? "Hủy thất bại."
            });
        }
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var userID = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userID))
                return RedirectToAction("Login", "Auth");

            var cartRes = await _baseService.GetAsync<CartDTO>(
                $"Odata/Carts({userID})", token);

            if (!cartRes.Success || cartRes.Data == null || !cartRes.Data.CartItems.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống, không thể thanh toán.";
                return RedirectToAction("Index", "Cart");
            }

            return View(cartRes.Data);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderCreateDTO dto)
        {
            var token = HttpContext.Session.GetString("JWToken");
            var userID = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userID))
                return Json(new { success = false, message = "Chưa đăng nhập." });

            var res = await _baseService.PostAsync<OrderDTO>(
                $"Odata/Orders?id={userID}", dto, token);

            if (!res.Success || res.Data == null)
                return Json(new { success = false, message = res.Message ?? "Đặt hàng thất bại." });

            return Json(new { success = true, orderID = res.Data.OrderID });
        }

        [HttpGet]
        public async Task<IActionResult> Confirmation(Guid id)
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            var res = await _baseService.GetAsync<OrderDTO>(
                $"Odata/Orders({id})", token);

            if (!res.Success || res.Data == null)
                return RedirectToAction("Index", "Flower");

            return View(res.Data);
        }
    }
}
