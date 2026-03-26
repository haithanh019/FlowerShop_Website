namespace FlowerShop.Application
{
    public class PayOSCheckoutResponseDTO
    {
        public string PaymentUrl { get; set; } = string.Empty;
        public long OrderCode { get; set; }
        public Guid PaymentID { get; set; }
        public Guid OrderID { get; set; }
    }
}
