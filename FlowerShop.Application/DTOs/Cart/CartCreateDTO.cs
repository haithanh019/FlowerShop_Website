using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Application
{
    public class CartCreateDTO
    {
        [Required]
        public Guid UserID { get; set; }
    }
}
