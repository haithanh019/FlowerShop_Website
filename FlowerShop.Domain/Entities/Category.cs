using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Domain
{
    public class Category
    {
        [Key]
        public Guid CategoryID { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        [MaxLength(300)]
        public string? Description { get; set; }

        public ICollection<Flower> Flowers { get; set; } = new List<Flower>();
    }
}
