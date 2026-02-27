using FlowerShop.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Domain.Entities
{
    public class User
    {
        [Key]
        public Guid UserID { get; set; } = Guid.NewGuid();

        [Required, MaxLength(100)]
        public string FullName { get; set; } = "";

        [Required, MaxLength(150)]
        public string Email { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";

        public UserRole Role { get; set; } = UserRole.Customer;

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        public DateTime? ResetToken { get; set; }

        public Cart? Cart { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}
