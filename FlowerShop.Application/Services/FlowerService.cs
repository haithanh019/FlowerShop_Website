using AutoMapper;
using AutoMapper.QueryableExtensions;
using FlowerShop.Domain;
using FlowerShop.Infrastructure;
using FlowerShop.Utility;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Application
{
    public class FlowerService : IFlowerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public FlowerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public IQueryable<FlowerDTO> GetFlowersOData()
        {
            return _unitOfWork.FlowerRepository.GetQuery().ProjectTo<FlowerDTO>(_mapper.ConfigurationProvider);
        }
        public async Task<ApiResponse<FlowerDTO>> GetFlowerByIDAsync(Guid id)
        {
            var flower = await _unitOfWork.FlowerRepository.GetQuery()
                    .Include(f => f.Category)
                    .Include(f => f.FlowerImages)
                    .FirstOrDefaultAsync(f => f.FlowerID == id);
            if (flower == null)
            {
                throw new NotFoundException("Không tìm thấy Hoa");
            }
            var respone = _mapper.Map<FlowerDTO>(flower);
            return new ApiResponse<FlowerDTO>(respone);
        }

        public async Task<ApiResponse<FlowerDTO>> CreateFlowerAsync(FlowerCreateDTO dto)
        {
            var flower = await _unitOfWork.FlowerRepository.GetByAsync(c => c.FlowerName == dto.FlowerName);
            if (flower != null)
            {
                throw new BadRequestException($"Hoa với tên '{dto.FlowerName}' đã tồn tại.");
            }
            var category = await _unitOfWork.CategoryRepository.GetByIDAsync(dto.CategoryID);
            if (category == null)
            {
                throw new NotFoundException($"Danh mục với ID {dto.CategoryID} không thấy.");
            }
            var Flower = _mapper.Map<Flower>(dto);
            Flower.Category = category;
            Flower.FlowerImages = new List<FlowerImage>();

            await _unitOfWork.FlowerRepository.AddAsync(Flower);
            await _unitOfWork.SaveAsync();

            var response = _mapper.Map<FlowerDTO>(Flower);
            return new ApiResponse<FlowerDTO>(response, "Thêm hoa thành công");
        }

        public async Task<ApiResponse<FlowerDTO>> UpdateFlowerAsync(Guid id, FlowerUpdateDTO dto)
        {
            var flower = await _unitOfWork.FlowerRepository.GetByAsync(
                    f => f.FlowerID == id,
                    trackChanges: true,
                    includeProperties: "Category,FlowerImages"
                );

            if (flower == null)
            {
                throw new NotFoundException("không tìm thấy hoa");
            }

            var duplicateFlower = await _unitOfWork.FlowerRepository.GetByAsync(c => c.FlowerName == dto.FlowerName && c.FlowerID != id);
            if (duplicateFlower != null)
            {
                throw new BadRequestException($"Hoa với tên '{dto.FlowerName}' đã được đặt.");
            }


            if (flower.CategoryID != dto.CategoryID)
            {
                var newCategory = await _unitOfWork.CategoryRepository.GetByIDAsync(dto.CategoryID);
                if (newCategory == null)
                    throw new NotFoundException($"Không tìm thấy danh mục {dto.CategoryID}");

                flower.Category = newCategory;
            }

            _mapper.Map(dto, flower);
            _unitOfWork.FlowerRepository.Update(flower);
            await _unitOfWork.SaveAsync();

            var respone = _mapper.Map<FlowerDTO>(flower);
            return new ApiResponse<FlowerDTO>(respone, "Cập nhật hoa thành công");
        }

        public async Task<ApiResponse<bool>> DeleteFlowerAsync(Guid id)
        {
            var Flower = await _unitOfWork.FlowerRepository.GetByIDAsync(id);

            if (Flower == null)
            {
                throw new NotFoundException("Không tìm thấy hoa");
            }

            _unitOfWork.FlowerRepository.Delete(Flower);
            await _unitOfWork.SaveAsync();
            return new ApiResponse<bool>(true, "Xá hoa thành công");
        }
    }
}
