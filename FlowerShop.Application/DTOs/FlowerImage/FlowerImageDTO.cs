using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Application
{
    public class FlowerImageDTO
    {
        [Key]
        public Guid FlowerImageID { get; set; }
        public string Url { get; set; } = string.Empty;
        public string? PublicID { get; set; }
        public Guid FlowerID { get; set; }
    }
}
