using FlowerShop.Utility;

namespace FlowerShop.Application
{
    public interface ICategoryService
    {
        IQueryable<CategoryDTO> GetCategoriesOData();
        Task<ApiResponse<CategoryDTO>> GetCategoryByIDAsync(Guid id);
        Task<ApiResponse<CategoryDTO>> CreateCategoryAsync(CategoryCreateDTO dto);
        Task<ApiResponse<CategoryDTO>> UpdateCategoryAsync(Guid id, CategoryUpdateDTO dto);
        Task<ApiResponse<bool>> DeleteCategoryAsync(Guid id);
    }
}
