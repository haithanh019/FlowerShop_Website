using FlowerShop.Domain;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Application
{
    public class OrderCreateDTO
    {
        [Required(ErrorMessage = "Vui lòng chọn giỏ hàng")]
        public Guid CartID { get; set; }

        [Required(ErrorMessage = "Địa chỉ giao hàng là bắt buộc")]
        [MaxLength(200)]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Precision(18, 2)]
        [Range(0, double.MaxValue)]
        public decimal ShippingFee { get; set; } = 0;

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        public PaymentMethod PaymentMethod { get; set; }

        public string? Note { get; set; }
        public string? ReturnUrl { get; set; }
        public string? CancelUrl { get; set; }
    }
}
