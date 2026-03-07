using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Application
{
    public class CartDTO

    {
        [Key]
        public Guid CartID { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public Guid UserID { get; set; }
        public ICollection<CartItemDTO> CartItems { get; set; } = new List<CartItemDTO>();
    }
}
