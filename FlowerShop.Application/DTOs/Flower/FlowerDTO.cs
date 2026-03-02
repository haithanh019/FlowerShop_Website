using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Application
{
    public class FlowerDTO
    {
        [Key]
        public Guid FlowerID { get; set; }
        public DateTime CreatedAt { get; set; }

        [Required, MaxLength(150)]
        public string FlowerName { get; set; } = "";

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Precision(18, 2)]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }

        public Guid CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        // Danh sách hình ảnh (Nested DTO)
        public List<FlowerImageDTO> FlowerImages { get; set; } = new();
    }
}
