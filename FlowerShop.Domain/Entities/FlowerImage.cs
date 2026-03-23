using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Domain
{
    public class FlowerImage
    {
        [Key]
        public Guid FlowerImageID { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Url { get; set; } = string.Empty;

        [MaxLength(100)]
        public string PublicID { get; set; } = string.Empty;

        public Guid FlowerID { get; set; }
        public required Flower Flower { get; set; }
    }
}
