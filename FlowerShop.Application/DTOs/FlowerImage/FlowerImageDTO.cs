namespace FlowerShop.Application
{
    public class FlowerImageDTO
    {
        public Guid FlowerImageID { get; set; }
        public string Url { get; set; } = string.Empty;
        public string? PublicID { get; set; }
    }
}
