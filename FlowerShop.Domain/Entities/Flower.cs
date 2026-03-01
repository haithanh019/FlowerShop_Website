using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Domain
{
    public class Flower
    {
        [Key]
        public Guid FlowerID { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required, MaxLength(150)]
        public string FlowerName { get; set; } = "";

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Precision(18, 2)]
        public decimal Price { get; set; }

        public int StockQuantity { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        public Guid CategoryID { get; set; }
        public Category? Category { get; set; }

        public ICollection<FlowerImage> FlowerImages { get; set; } = new List<FlowerImage>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
