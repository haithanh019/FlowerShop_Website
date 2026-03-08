using FlowerShop.Application;
using FlowerShop.Client.Models;
using FlowerShop.Client.Models.Home;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FlowerShop.Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBaseService _baseService;

        public HomeController(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel();

            var categoryResponse = await _baseService.GetODataAsync<IEnumerable<CategoryDTO>>("/Odata/Categories");
            if (categoryResponse != null)
            {
                viewModel.Categories = categoryResponse;
            }

            var flowerResponse = await _baseService.GetODataAsync<IEnumerable<FlowerDTO>>("/Odata/Flowers");
            if (flowerResponse != null)
            {
                viewModel.Flowers = flowerResponse;
            }

            return View(viewModel);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
