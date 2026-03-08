using FlowerShop.Application;

namespace FlowerShop.Client.Models.Home
{
    public class HomeViewModel
    {
        public IEnumerable<CategoryDTO> Categories { get; set; } = new List<CategoryDTO>();
        public IEnumerable<FlowerDTO> Flowers { get; set; } = new List<FlowerDTO>();
    }
}
