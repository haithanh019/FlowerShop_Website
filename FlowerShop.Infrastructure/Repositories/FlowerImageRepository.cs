using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FlowerShop.Domain;
using FlowerShop.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FlowerShop.Infrastructure
{
    public class FlowerImageRepository : GenericRepository<FlowerImage>, IFlowerImageRepository
    {
        private readonly FlowerShopDbContext _db;
        private readonly Cloudinary _cloudinary;
        public FlowerImageRepository(FlowerShopDbContext db, IOptions<CloudinarySettings> options)
            : base(db)
        {
            _db = db;

            var account = new Account(
                    options.Value.CloudName,
                    options.Value.ApiKey,
                    options.Value.ApiSecret
                );
            _cloudinary = new Cloudinary(account);
        }
        public async Task UploadImageAsync(IFormFile file, string folder, FlowerImage image)
        {
            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false,
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            image.PublicID = result.PublicId;
            image.Url = result.SecureUrl.ToString();
            _db.FlowerImages.Add(image);
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
                return false;

            var deletionParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deletionParams);

            if (result.Error != null)
                return false;

            if (result.Result == "ok" || result.Result == "not found")
            {
                var image = await _db.FlowerImages.FirstOrDefaultAsync(i => i.PublicID == publicId);
                if (image != null)
                {
                    _db.FlowerImages.Remove(image);
                    await _db.SaveChangesAsync();
                }
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteImagesAsync(List<string> publicIds)
        {
            if (publicIds == null || !publicIds.Any())
                return false;

            var deleteTasks = publicIds.Select(publicId => DeleteImageAsync(publicId));

            var results = await Task.WhenAll(deleteTasks);

            return results.All(r => r);
        }
    }
}
