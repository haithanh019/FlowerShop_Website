using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Application
{
    public class UserDTO
    {
        [Key]
        public Guid UserID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty; // JWT Token
    }
}
