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
                throw new NotFoundException("Không tìm thấy danh mục.");
            }
            var respone = _mapper.Map<CategoryDTO>(categories);
            return new ApiResponse<CategoryDTO>(respone);
        }

        public async Task<ApiResponse<CategoryDTO>> CreateCategoryAsync(CategoryCreateDTO dto)
        {
            var existingCategory = await _unitOfWork.CategoryRepository.GetByAsync(c => c.Name == dto.Name);
            if (existingCategory != null)
            {
                throw new BadRequestException($"Danh mục đã tồn tại.");
            }

            var category = _mapper.Map<Category>(dto);

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
                throw new NotFoundException("Không tìm thấy danh mục");
            }

            var duplicateCategory = await _unitOfWork.CategoryRepository.GetByAsync(c => c.Name == dto.Name && c.CategoryID != id);
            if (duplicateCategory != null)
            {
                throw new BadRequestException($"Danh mục với tên '{dto.Name}' đã được đặt.");
            }

            _mapper.Map(dto, category);
            _unitOfWork.CategoryRepository.Update(category);
            await _unitOfWork.SaveAsync();

            var respone = _mapper.Map<CategoryDTO>(category);
            return new ApiResponse<CategoryDTO>(respone, "Cập nhật danh mục thành công");
        }

        public async Task<ApiResponse<bool>> DeleteCategoryAsync(Guid id)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIDAsync(id);

            if (category == null)
            {
                throw new NotFoundException("Không tìm thấy danh mục");
            }

            _unitOfWork.CategoryRepository.Delete(category);
            await _unitOfWork.SaveAsync();
            return new ApiResponse<bool>(true, "Xóa danh mục thành công");
        }
    }
}
