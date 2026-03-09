using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Application
{
    public class AddressDTO
    {
        [Key]
        public Guid AddressID { get; set; }
        public Guid UserID { get; set; }
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Ward { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
    }
}
