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

            var response = _mapper.Map<FlowerDTO>(flower);
            return new ApiResponse<FlowerDTO>(response);
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

            ValidateImages(dto.Urls?.ToList());

            var flower = _mapper.Map<Flower>(dto);
            flower.Category = category;
            flower.FlowerImages = [];

            await _unitOfWork.FlowerRepository.AddAsync(flower);
            await _unitOfWork.SaveAsync();

            await UploadFlowerImagesAsync(flower, flower.FlowerID, dto.Urls, dto.PublicIds);
            await _unitOfWork.SaveAsync();

            var response = _mapper.Map<FlowerDTO>(flower);
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

            await SyncFlowerImagesAsync(flower, dto.DeleteImageIds, dto.Urls, dto.PublicIds);

            _unitOfWork.FlowerRepository.Update(flower);
            await _unitOfWork.SaveAsync();

            var response = _mapper.Map<FlowerDTO>(flower);
            return new ApiResponse<FlowerDTO>(response, "Cập nhật hoa thành công");
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
                _unitOfWork.FlowerImageRepository.DeleteRange(flower.FlowerImages);
            }

            _unitOfWork.FlowerRepository.Delete(flower);
            await _unitOfWork.SaveAsync();

            return new ApiResponse<bool>(true, "Xóa hoa thành công");
        }

        // ─────────────────────────────────────────
        // PRIVATE HELPERS
        // ─────────────────────────────────────────

        private async Task UploadFlowerImagesAsync(
            Flower flower,
            Guid flowerID,
            ICollection<string>? imageUrls,
            ICollection<string>? publicIds)
        {
            if (imageUrls == null || !imageUrls.Any())
                return;

            var ids = publicIds?.ToList() ?? new List<string>();

            var images = imageUrls
                .Select((url, i) => new FlowerImage
                {
                    ProductImageID = Guid.NewGuid(),
                    FlowerID = flowerID,
                    Flower = flower,
                    Url = url,
                    PublicID = i < ids.Count ? ids[i] : string.Empty,
                })
                .ToList();

            await _unitOfWork.FlowerImageRepository.AddRangeAsync(images);
        }

        private static void ValidateImages(ICollection<string>? images, int existingCount = 0)
        {
            if (images == null || !images.Any())
                return;

            var errors = new Dictionary<string, string[]>();
            var totalCount = existingCount + images.Count;

            if (totalCount > 5)
                errors.Add(nameof(images), ["ERROR_MAXIMUM_IMAGE"]);

            if (errors.Any())
                throw new ValidationException(errors);
        }


        private async Task SyncFlowerImagesAsync(
            Flower flower,
            ICollection<Guid>? deleteImageIds,
            ICollection<string>? urls,
            ICollection<string>? publicIds)
        {
            if (deleteImageIds != null && deleteImageIds.Any())
            {
                var imagesToDelete = flower.FlowerImages
                    .Where(img => deleteImageIds.Contains(img.ProductImageID))
                    .ToList();

                if (imagesToDelete.Any())
                {
                    _unitOfWork.FlowerImageRepository.DeleteRange(imagesToDelete);

                    foreach (var img in imagesToDelete)
                        flower.FlowerImages.Remove(img);
                }
            }

            ValidateImages(urls?.ToList(), existingCount: flower.FlowerImages.Count);

            await UploadFlowerImagesAsync(flower, flower.FlowerID, urls, publicIds);
        }
    }
}
