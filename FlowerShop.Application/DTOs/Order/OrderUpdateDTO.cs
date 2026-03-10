using FlowerShop.Domain;
using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Application
{
    public class OrderUpdateDTO
    {
        [Required]
        public OrderStatus OrderStatus { get; set; }
    }
}
