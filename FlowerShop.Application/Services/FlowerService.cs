using AutoMapper;
using AutoMapper.QueryableExtensions;
using FlowerShop.Domain;
using FlowerShop.Infrastructure;
using FlowerShop.Utility;
using Microsoft.AspNetCore.OData.Query;
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
        [EnableQuery]
        public IQueryable<FlowerDTO> GetFlowersOData()
        {
            return _unitOfWork.FlowerRepository.GetQuery().ProjectTo<FlowerDTO>(_mapper.ConfigurationProvider);
        }
        public async Task<ApiResponse<FlowerDTO>> GetFlowerByIdAsync(Guid id)
        {
            var flower = await _unitOfWork.FlowerRepository.GetQuery()
                    .Include(f => f.Category)
                    .Include(f => f.FlowerImages)
                    .FirstOrDefaultAsync(f => f.FlowerID == id);
            if (flower == null)
            {
                throw new NotFoundException("Flower not found");
            }
            var respone = _mapper.Map<FlowerDTO>(flower);
            return new ApiResponse<FlowerDTO>(respone);
        }

        public async Task<ApiResponse<FlowerDTO>> CreateFlowerAsync(FlowerCreateDTO dto)
        {
            var flower = await _unitOfWork.FlowerRepository.GetByAsync(c => c.FlowerName == dto.FlowerName);
            if (flower != null)
            {
                throw new BadRequestException($"Flower with name '{dto.FlowerName}' already exists.");
            }
            var category = await _unitOfWork.CategoryRepository.GetByIDAsync(dto.CategoryID);
            if (category == null)
            {
                throw new NotFoundException($"Category with ID {dto.CategoryID} not found.");
            }
            var Flower = _mapper.Map<Flower>(dto);
            Flower.Category = category;
            Flower.FlowerImages = new List<FlowerImage>();

            await _unitOfWork.FlowerRepository.AddAsync(Flower);
            await _unitOfWork.SaveAsync();

            var response = _mapper.Map<FlowerDTO>(Flower);
            return new ApiResponse<FlowerDTO>(response, "Create flower successfully");
        }

        public async Task<ApiResponse<FlowerDTO>> UpdateFlowerAsync(Guid id, FlowerUpdateDTO dto)
        {
            var Flower = await _unitOfWork.FlowerRepository.GetByIDAsync(id);
            if (Flower == null)
            {
                throw new NotFoundException("Flower not found");
            }

            var duplicateFlower = await _unitOfWork.FlowerRepository.GetByAsync(c => c.FlowerName == dto.FlowerName && c.FlowerID != id);
            if (duplicateFlower != null)
            {
                throw new BadRequestException($"Flower name '{dto.FlowerName}' is already taken.");
            }

            _mapper.Map(dto, Flower);
            _unitOfWork.FlowerRepository.Update(Flower);
            await _unitOfWork.SaveAsync();

            var respone = _mapper.Map<FlowerDTO>(Flower);
            return new ApiResponse<FlowerDTO>(respone, "Update Flower Successfully");
        }

        public async Task<ApiResponse<bool>> DeleteFlowerAsync(Guid id)
        {
            var Flower = await _unitOfWork.FlowerRepository.GetByIDAsync(id);

            if (Flower == null)
            {
                throw new NotFoundException("Flower not found");
            }

            _unitOfWork.FlowerRepository.Delete(Flower);
            await _unitOfWork.SaveAsync();
            return new ApiResponse<bool>(true, "Delete Flower Successfully");
        }
    }
}
