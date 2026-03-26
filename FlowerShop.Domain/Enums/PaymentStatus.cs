using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Domain
{
    public enum PaymentStatus
    {
        [Display(Name = "Pending")]
        Pending = 0,

        [Display(Name = "Paid")]
        Paid = 1,

        [Display(Name = "Cancelled")]
        Cancelled = 2,

        [Display(Name = "Refunded")]
        Refunded = 3,
    }
}
