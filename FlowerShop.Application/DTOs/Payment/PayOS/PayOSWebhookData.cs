namespace FlowerShop.Application
{
    public class PayOSWebhookData
    {
        public long OrderCode { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string TransactionDateTime { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
    }
}
