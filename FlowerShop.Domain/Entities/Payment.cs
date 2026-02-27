using FlowerShop.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Domain.Entities
{
    public class Payment
    {
        [Key]
        public Guid PaymentID { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid OrderID { get; set; }
        public Order? Order { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        [Precision(18, 2)]
        public decimal Amount { get; set; }

        public DateTime? PaidAt { get; set; }

        public string? TransactionID { get; set; }
    }
}
