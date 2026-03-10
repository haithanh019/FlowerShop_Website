using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Application
{
    public class OrderItemDTO
    {
        [Key]
        public Guid OrderItemID { get; set; }
        public Guid FlowerID { get; set; }
        public string FlowerName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }

        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }

        [Precision(18, 2)]
        public decimal LineTotal { get; set; }
    }
}
