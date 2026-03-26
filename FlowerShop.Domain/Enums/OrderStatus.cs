using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Domain
{
    public enum OrderStatus
    {
        [Display(Name = "Processing")]
        Processing = 0,

        [Display(Name = "Confirmed")]
        Confirmed = 1,

        [Display(Name = "Delivering")]
        Delivering = 2,

        [Display(Name = "Completed")]
        Completed = 3,

        [Display(Name = "Canceled")]
        Cancelled = 4,
    }
}
