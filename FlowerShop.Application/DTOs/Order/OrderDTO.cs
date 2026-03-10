using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Application
{
    public class OrderDTO
    {
        [Key]
        public Guid OrderID { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserID { get; set; }
        public string OrderStatus { get; set; } = string.Empty;

        public string? ShippingAddress { get; set; }
        public string? PhoneNumber { get; set; }

        [Precision(18, 2)]
        public decimal Subtotal { get; set; }

        [Precision(18, 2)]
        public decimal ShippingFee { get; set; }

        [Precision(18, 2)]
        public decimal TotalAmount { get; set; }

        public List<OrderItemDTO> OrderItems { get; set; } = new();
    }
}
