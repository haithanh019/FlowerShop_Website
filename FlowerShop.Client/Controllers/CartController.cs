using FlowerShop.Application;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Client
{
    public class CartController : Controller
    {
        private readonly IBaseService _baseService;

        public CartController(IBaseService baseService)
        {
            _baseService = baseService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var userID = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userID))
                return RedirectToAction("Login", "Auth");

            var response = await _baseService.GetAsync<CartDTO>(
                $"Odata/Carts({userID})", token);

            if (!response.Success || response.Data == null)
            {
                TempData["ErrorMessage"] = "Không thể tải giỏ hàng.";
                return View(new CartDTO { UserID = Guid.Parse(userID) });
            }

            return View(response.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(Guid flowerID)
        {
            var token = HttpContext.Session.GetString("JWToken");
            var userID = HttpContext.Session.GetString("UserID");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userID))
                return RedirectToAction("Login", "Auth");

            var cartResponse = await _baseService.GetAsync<CartDTO>(
                $"Odata/Carts({userID})", token);

            if (!cartResponse.Success || cartResponse.Data == null)
            {
                TempData["ErrorMessage"] = "Không thể lấy thông tin giỏ hàng.";
                return RedirectToAction("Index", "Flower");
            }

            var dto = new CartItemCreateDTO
            {
                CartID = cartResponse.Data.CartID,
                FlowerID = flowerID,
                Quantity = 1
            };

            var addResponse = await _baseService.PostAsync<CartItemDTO>(
                "Odata/CartItems", dto, token);

            TempData[addResponse.Success ? "SuccessMessage" : "ErrorMessage"] =
                addResponse.Success ? "Đã thêm vào giỏ hàng!" : "Thêm vào giỏ hàng thất bại.";

            return RedirectToAction("Index", "Flower");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantityAjax(
            Guid cartItemID, [FromBody] CartItemUpdateDTO dto)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
                return Json(new { success = false, message = "Chưa đăng nhập." });

            // Quantity >= 1 đã được [Range] đảm bảo — gọi thẳng PUT
            var res = await _baseService.PutAsync<CartItemDTO>(
                $"Odata/CartItems({cartItemID})", dto, token);

            return Json(new
            {
                success = res.Success,
                message = res.Success ? "Đã cập nhật." : "Cập nhật thất bại."
            });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItemAjax(Guid cartItemID)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
                return Json(new { success = false, message = "Chưa đăng nhập." });

            var res = await _baseService.DeleteAsync<bool>(
                $"Odata/CartItems({cartItemID})", token);

            return Json(new
            {
                success = res.Success,
                message = res.Success ? "Đã xóa." : "Xóa thất bại."
            });
        }
    }
}
