using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Domain
{
    public class Order
    {
        [Key]
        public Guid OrderID { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid UserID { get; set; }
        public User? User { get; set; }

        public OrderStatus OrderStatus { get; set; } = OrderStatus.Processing;
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CashOnDelivery;
        [MaxLength(200)]
        public string? ShippingAddress { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        [Precision(18, 2)]
        public decimal Subtotal { get; set; }

        [Precision(18, 2)]
        public decimal ShippingFee { get; set; }

        [Precision(18, 2)]
        public decimal TotalAmount { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public Payment? Payment { get; set; }
    }
}
