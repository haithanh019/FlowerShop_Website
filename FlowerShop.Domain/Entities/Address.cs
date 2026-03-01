using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Domain
{
    public class Address
    {
        [Key]
        public Guid AddressID { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserID { get; set; }
        public User? User { get; set; }

        [Required, MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string District { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Ward { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string Detail { get; set; } = string.Empty;
    }
}
