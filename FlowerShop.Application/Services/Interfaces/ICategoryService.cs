using FlowerShop.Utility;

namespace FlowerShop.Application
{
    public interface ICategoryService
    {
        IQueryable<CategoryDTO> GetCategoriesOData();
        Task<ApiResult<CategoryDTO>> GetCategoryByIDAsync(Guid id);
        Task<ApiResult<CategoryDTO>> CreateCategoryAsync(CategoryCreateDTO dto);
        Task<ApiResult<CategoryDTO>> UpdateCategoryAsync(Guid id, CategoryUpdateDTO dto);
        Task<ApiResult<bool>> DeleteCategoryAsync(Guid id);
    }
}
