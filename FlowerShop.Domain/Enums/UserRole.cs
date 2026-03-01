using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Domain
{
    public enum UserRole
    {
        [Display(Name = "User")]
        Customer = 0,

        [Display(Name = "Admin")]
        Admin = 1,
    }
}
