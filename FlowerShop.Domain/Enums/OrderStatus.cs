using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Domain
{
    public enum OrderStatus
    {
        [Display(Name = "Processing")]
        Pending = 0,

        [Display(Name = "Confirm")]
        Confirmed = 1,

        [Display(Name = "Delivering")]
        Shipping = 2,

        [Display(Name = "Completed")]
        Completed = 3,

        [Display(Name = "Canceled")]
        Cancelled = 4,
    }
}
