using Microsoft.AspNetCore.Http;

namespace FlowerShop.Utility
{
    public class CloudinaryUploadResult
    {
        public string Url { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
    }

    public interface ICloudinaryService
    {
        Task<CloudinaryUploadResult> UploadAsync(IFormFile file, string folder = "flower_shop");
        Task DeleteAsync(string publicId);
    }
}
