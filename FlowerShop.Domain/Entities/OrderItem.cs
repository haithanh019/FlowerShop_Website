using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Domain.Entities
{
    public class OrderItem
    {
        [Key]
        public Guid OrderItemID { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid OrderID { get; set; }
        public Order? Order { get; set; }

        public Guid FlowerID { get; set; }
        public Flower? Flower { get; set; }

        public int Quantity { get; set; }

        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }

        [Precision(18, 2)]
        public decimal LineTotal { get; set; }
    }
}
