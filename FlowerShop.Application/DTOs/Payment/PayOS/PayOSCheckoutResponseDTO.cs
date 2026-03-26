namespace FlowerShop.Application
{
    public class PayOSCheckoutResponseDTO
    {
        public Guid PaymentID { get; set; }
        public Guid OrderID { get; set; }
        public long OrderCode { get; set; }
        public string PaymentUrl { get; set; } = string.Empty;
    }

}
