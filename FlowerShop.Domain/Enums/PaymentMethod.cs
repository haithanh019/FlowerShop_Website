using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Domain.Enums
{
    public enum PaymentMethod
    {
        [Display(Name = "Cash On Delivery")]
        CashOnDelivery = 0,

        [Display(Name = "Banking")]
        PayOS = 1,
    }
}
