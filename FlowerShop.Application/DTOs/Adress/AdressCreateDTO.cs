using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Application
{
    public class AddressCreateDTO
    {
        [Required]
        public Guid UserID { get; set; }

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
