using FlowerShop.Domain;

namespace FlowerShop.Application
{
    public class PaymentUpdateDTO
    {
        public PaymentStatus PaymentStatus { get; set; }
        public string? TransactionID { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? WebhookReceivedAt { get; set; }
    }
}
