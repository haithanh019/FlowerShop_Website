using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Client
{
    public class ProfileViewModel
    {
        public Guid UserID { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Vai trò")]
        public string Role { get; set; } = string.Empty;
    }
}
