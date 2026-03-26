using FlowerShop.Domain;
using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Application
{
    public class PaymentCreateDTO
    {
        [Required]
        public Guid OrderID { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        public string? ReturnUrl { get; set; }
        public string? CancelUrl { get; set; }
    }
}
