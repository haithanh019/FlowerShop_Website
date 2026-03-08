using AutoMapper;
using AutoMapper.QueryableExtensions;
using FlowerShop.Domain;
using FlowerShop.Infrastructure;
using FlowerShop.Utility;
using Microsoft.AspNetCore.OData.Query;

namespace FlowerShop.Application
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [EnableQuery]
        public IQueryable<CategoryDTO> GetCategoriesOData()
        {
            return _unitOfWork.CategoryRepository.GetQuery().ProjectTo<CategoryDTO>(_mapper.ConfigurationProvider);
        }
        public async Task<ApiResponse<CategoryDTO>> GetCategoryByIDAsync(Guid id)
        {
            var categories = await _unitOfWork.CategoryRepository.GetByIDAsync(id);
            if (categories == null)
            {
                throw new NotFoundException("Category not found");
            }
            var respone = _mapper.Map<CategoryDTO>(categories);
            return new ApiResponse<CategoryDTO>(respone);
        }

        public async Task<ApiResponse<CategoryDTO>> CreateCategoryAsync(Guid id)
        {
            var existingCategory = await _unitOfWork.CategoryRepository.GetByIDAsync(id);
            if (existingCategory != null)
            {
                throw new BadRequestException($"Category already exists.");
            }

            var category = _mapper.Map<Category>(id);

            await _unitOfWork.CategoryRepository.AddAsync(category);
            await _unitOfWork.SaveAsync();

            var response = _mapper.Map<CategoryDTO>(category);
            return new ApiResponse<CategoryDTO>(response);
        }

        public async Task<ApiResponse<CategoryDTO>> UpdateCategoryAsync(Guid id, CategoryUpdateDTO dto)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIDAsync(id);
            if (category == null)
            {
                throw new NotFoundException("Category not found");
            }

            var duplicateCategory = await _unitOfWork.CategoryRepository.GetByAsync(c => c.Name == dto.Name && c.CategoryID != id);
            if (duplicateCategory != null)
            {
                throw new BadRequestException($"Category name '{dto.Name}' is already taken.");
            }

            _mapper.Map(dto, category);
            _unitOfWork.CategoryRepository.Update(category);
            await _unitOfWork.SaveAsync();

            var respone = _mapper.Map<CategoryDTO>(category);
            return new ApiResponse<CategoryDTO>(respone, "Update Category Successfully");
        }

        public async Task<ApiResponse<bool>> DeleteCategoryAsync(Guid id)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIDAsync(id);

            if (category == null)
            {
                throw new NotFoundException("Category not found");
            }

            _unitOfWork.CategoryRepository.Delete(category);
            await _unitOfWork.SaveAsync();
            return new ApiResponse<bool>(true, "Delete Category Successfully");
        }
    }
}
