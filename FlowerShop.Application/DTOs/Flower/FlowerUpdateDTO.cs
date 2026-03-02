using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Application
{
    public class FlowerUpdateDTO
    {
        [Required]
        [MaxLength(150)]
        public string FlowerName { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        public bool IsActive { get; set; }

        public Guid CategoryID { get; set; }
    }
}
