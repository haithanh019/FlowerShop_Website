using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Application
{
    public class CartItemUpdateDTO
    {
        [Required]
        [Range(1, 1000, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }
    }
}
