using FlowerShop.Application;
using Microsoft.AspNetCore.Mvc;

namespace FlowerShop.Client.Controllers
{
    public class FlowerController(IBaseService baseService) : Controller
    {
        private readonly IBaseService _baseService = baseService;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var flowers = await _baseService.GetODataAsync<IEnumerable<FlowerDTO>>("Odata/Flowers?$expand=FlowerImages&$filter=IsActive eq true");

            if (flowers == null)
            {
                ViewBag.ErrorMessage = "Không thể lấy danh sách hoa.";
                return View(new List<FlowerDTO>());
            }
            return View(flowers);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var response = await _baseService.GetAsync<FlowerDTO>($"/Odata/Flowers/{id}");

            if (response == null || !response.Success || response.Data == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hoa.";
                return RedirectToAction(nameof(Index));
            }

            return View(response.Data);
        }
    }
}
