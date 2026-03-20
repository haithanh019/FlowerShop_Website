using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Application
{
    public class FlowerCreateDTO
    {
        [Required(ErrorMessage = "Flower Name is required")]
        [MaxLength(150)]
        public string FlowerName { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be >= 0")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be >= 0")]
        public int StockQuantity { get; set; }

        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Category is required")]
        public Guid CategoryID { get; set; }
        public ICollection<string>? Urls { get; set; }
        public ICollection<string>? PublicIds { get; set; }
    }
}
