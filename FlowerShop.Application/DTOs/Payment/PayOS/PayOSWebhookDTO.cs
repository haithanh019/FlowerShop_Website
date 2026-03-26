namespace FlowerShop.Application
{
    public class PayOSWebhookDTO
    {
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Success { get; set; }
        public PayOSWebhookData? Data { get; set; }
        public string Signature { get; set; } = string.Empty;
    }
}
