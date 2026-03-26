using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Application
{
    public class PaymentDTO
    {
        [Key]
        public Guid PaymentID { get; set; }
        public Guid OrderID { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? TransactionID { get; set; }
        public long? PayOSOrderCode { get; set; }
        public string? PaymentUrl { get; set; }
    }

}
