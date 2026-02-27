using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Domain.Entities
{
    public class Cart
    {
        [Key]
        public Guid CartID { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public Guid UserID { get; set; }
        public User? User { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
