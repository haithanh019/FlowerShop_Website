using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Application
{
    public class CategoryDTO
    {
        [Key]
        public Guid CategoryID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
