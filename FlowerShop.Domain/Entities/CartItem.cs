using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Domain.Entities
{
    public class CartItem
    {
        [Key]
        public Guid CartItemID { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid CartID { get; set; }
        public Cart? Cart { get; set; }

        public Guid FlowerID { get; set; }
        public Flower? Flower { get; set; }

        public int Quantity { get; set; } = 1;

        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }
    }
}
