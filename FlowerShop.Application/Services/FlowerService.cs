using AutoMapper;
using AutoMapper.QueryableExtensions;
using FlowerShop.Domain;
using FlowerShop.Infrastructure;
using FlowerShop.Utility;
using Microsoft.AspNetCore.Http;
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
            return _unitOfWork.FlowerRepository
                .GetQuery()
                .ProjectTo<FlowerDTO>(_mapper.ConfigurationProvider);
        }

        public async Task<ApiResponse<FlowerDTO>> GetFlowerByIDAsync(Guid id)
        {
            var flower = await _unitOfWork.FlowerRepository.GetQuery()
                .Include(f => f.Category)
                .Include(f => f.FlowerImages)
                .FirstOrDefaultAsync(f => f.FlowerID == id);

            if (flower == null)
                throw new NotFoundException("Không tìm thấy hoa");

            return new ApiResponse<FlowerDTO>(_mapper.Map<FlowerDTO>(flower));
        }

        public async Task<ApiResponse<FlowerDTO>> CreateFlowerAsync(FlowerCreateDTO dto)
        {
            var existing = await _unitOfWork.FlowerRepository
                .GetByAsync(c => c.FlowerName == dto.FlowerName);
            if (existing != null)
                throw new BadRequestException($"Hoa với tên '{dto.FlowerName}' đã tồn tại.");

            var category = await _unitOfWork.CategoryRepository.GetByIDAsync(dto.CategoryID);
            if (category == null)
                throw new NotFoundException($"Danh mục với ID {dto.CategoryID} không tìm thấy.");

            ValidateImages(dto.FlowerImages?.Select(f => f.FileName).ToList());

            var flower = _mapper.Map<Flower>(dto);
            flower.Category = category;
            flower.FlowerImages = [];

            await _unitOfWork.FlowerRepository.AddAsync(flower);
            await _unitOfWork.SaveAsync();

            await UploadFlowerImagesAsync(flower, dto.FlowerImages);
            await _unitOfWork.SaveAsync();

            return new ApiResponse<FlowerDTO>(_mapper.Map<FlowerDTO>(flower), "Thêm hoa thành công");
        }

        public async Task<ApiResponse<FlowerDTO>> UpdateFlowerAsync(Guid id, FlowerUpdateDTO dto)
        {
            var flower = await _unitOfWork.FlowerRepository.GetByAsync(
                f => f.FlowerID == id,
                trackChanges: true,
                includeProperties: "Category,FlowerImages"
            );
            if (flower == null)
                throw new NotFoundException("Không tìm thấy hoa");

            var duplicate = await _unitOfWork.FlowerRepository
                .GetByAsync(c => c.FlowerName == dto.FlowerName && c.FlowerID != id);
            if (duplicate != null)
                throw new BadRequestException($"Hoa với tên '{dto.FlowerName}' đã được đặt.");

            if (flower.CategoryID != dto.CategoryID)
            {
                var newCategory = await _unitOfWork.CategoryRepository.GetByIDAsync(dto.CategoryID);
                if (newCategory == null)
                    throw new NotFoundException($"Không tìm thấy danh mục {dto.CategoryID}");
                flower.Category = newCategory;
            }

            _mapper.Map(dto, flower);

            await SyncFlowerImagesAsync(flower, dto.DeleteImageIds, dto.FlowerImages);

            _unitOfWork.FlowerRepository.Update(flower);
            await _unitOfWork.SaveAsync();

            return new ApiResponse<FlowerDTO>(_mapper.Map<FlowerDTO>(flower), "Cập nhật hoa thành công");
        }

        public async Task<ApiResponse<bool>> DeleteFlowerAsync(Guid id)
        {
            var flower = await _unitOfWork.FlowerRepository.GetByAsync(
                f => f.FlowerID == id,
                trackChanges: true,
                includeProperties: "FlowerImages"
            );
            if (flower == null)
                throw new NotFoundException("Không tìm thấy hoa");

            if (flower.FlowerImages != null && flower.FlowerImages.Any())
            {
                var publicIds = flower.FlowerImages
                    .Where(img => !string.IsNullOrEmpty(img.PublicID))
                    .Select(img => img.PublicID!)
                    .ToList();

                if (publicIds.Any())
                    await _unitOfWork.FlowerImageRepository.DeleteImagesAsync(publicIds);
            }

            _unitOfWork.FlowerRepository.Delete(flower);
            await _unitOfWork.SaveAsync();

            return new ApiResponse<bool>(true, "Xóa hoa thành công");
        }

        // ─────────────────────────────────────────
        // PRIVATE HELPERS
        // ─────────────────────────────────────────

        private async Task UploadFlowerImagesAsync(Flower flower, ICollection<IFormFile>? files)
        {
            if (files == null || !files.Any()) return;

            foreach (var file in files)
            {
                var image = new FlowerImage { FlowerID = flower.FlowerID, Flower = flower };
                await _unitOfWork.FlowerImageRepository.UploadImageAsync(file, "flower_shop", image);
            }
        }

        private static void ValidateImages(ICollection<string>? fileNames, int existingCount = 0)
        {
            if (fileNames == null || !fileNames.Any()) return;

            if (existingCount + fileNames.Count > 5)
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    [nameof(fileNames)] = ["ERROR_MAXIMUM_IMAGE"]
                });
        }

        private async Task SyncFlowerImagesAsync(
            Flower flower,
            ICollection<string>? deletePublicIds,
            ICollection<IFormFile>? newImages)
        {
            if (deletePublicIds != null && deletePublicIds.Any())
            {
                await _unitOfWork.FlowerImageRepository.DeleteImagesAsync(deletePublicIds.ToList());
                flower.FlowerImages = flower.FlowerImages
                    .Where(img => !deletePublicIds.Contains(img.PublicID))
                    .ToList();
            }

            ValidateImages(
                newImages?.Select(f => f.FileName).ToList(),
                existingCount: flower.FlowerImages.Count);

            await UploadFlowerImagesAsync(flower, newImages);
        }
    }
}
