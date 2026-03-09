using System.ComponentModel.DataAnnotations;

namespace FlowerShop.Application
{
    public class CategoryCreateDTO
    {
        [Required(ErrorMessage = "Category Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string CategoryName { get; set; } = string.Empty;

        [MaxLength(300, ErrorMessage = "Description cannot exceed 300 characters")]
        public string? Description { get; set; }
    }
}
