using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Application
{
    public class CartItemDTO
    {
        [Key]
        public Guid CartItemID { get; set; }
        public Guid CartID { get; set; }
        public Guid FlowerID { get; set; }
        public int Quantity { get; set; }
        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }
        public DateTime CreatedAt { get; set; }

        public string FlowerName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }

        public decimal TotalPrice => Quantity * UnitPrice;
    }
}
