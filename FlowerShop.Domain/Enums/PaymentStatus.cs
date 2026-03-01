using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Domain
{
    public enum PaymentStatus
    {
        [Display(Name = "Pending")]
        Pending = 0,

        [Display(Name = "Paid")]
        Accepted = 1,

        [Display(Name = "Delived")]
        Delivered = 2,

        [Display(Name = "Refunded")]
        Refunded = 3,
    }
}
