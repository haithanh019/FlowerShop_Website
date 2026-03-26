namespace FlowerShop.Application
{
    public class PayOSWebhookDTO
    {
        public string Code { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public PayOSWebhookData? Data { get; set; }
        public string Signature { get; set; } = string.Empty;
    }
}
