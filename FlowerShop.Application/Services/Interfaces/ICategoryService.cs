using FlowerShop.Utility;

namespace FlowerShop.Application
{
    public interface ICategoryService
    {
        IQueryable<CategoryDTO> GetCategoriesOData();
        Task<ApiResponse<CategoryDTO>> GetCategoryByIDAsync(Guid id);
        Task<ApiResponse<CategoryDTO>> CreateCategoryAsync(Guid id);
        Task<ApiResponse<CategoryDTO>> UpdateCategoryAsync(Guid id, CategoryUpdateDTO model);
        Task<ApiResponse<bool>> DeleteCategoryAsync(Guid id);
    }
}
