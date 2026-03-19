using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Application
{
    public class CartItemCreateDTO
    {
        [Required]
        public Guid CartID { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn sản phẩm")]
        public Guid FlowerID { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; } = 1;
    }
}
