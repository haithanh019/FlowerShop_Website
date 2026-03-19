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

            var cartID = cartResponse.Data.CartID;

            var dto = new CartItemCreateDTO
            {
                FlowerID = flowerID,
                Quantity = 1
            };

            var addResponse = await _baseService.PostAsync<CartItemDTO>(
                $"Odata/CartItems", dto, token);

            if (addResponse.Success)
                TempData["SuccessMessage"] = "Đã thêm vào giỏ hàng!";
            else
                TempData["ErrorMessage"] = "Thêm vào giỏ hàng thất bại.";

            return RedirectToAction("Index", "Flower");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(Guid cartItemID, int quantity)
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            if (quantity <= 0)
                return await RemoveItem(cartItemID);

            var dto = new CartItemUpdateDTO { Quantity = quantity };

            var response = await _baseService.PutAsync<CartItemDTO>(
                $"Odata/CartItems({cartItemID})", dto, token);

            if (!response.Success)
                TempData["ErrorMessage"] = "Cập nhật thất bại.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(Guid cartItemID)
        {
            var token = HttpContext.Session.GetString("JWToken");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            var response = await _baseService.DeleteAsync<bool>(
                $"Odata/CartItems({cartItemID})", token);

            if (response.Success)
                TempData["SuccessMessage"] = "Đã xóa sản phẩm khỏi giỏ hàng.";
            else
                TempData["ErrorMessage"] = "Xóa thất bại.";

            return RedirectToAction(nameof(Index));
        }
    }
}

