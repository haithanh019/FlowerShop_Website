using Microsoft.AspNetCore.Http;

namespace FlowerShop.Utility
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file, string folderName);
        Task DeleteImageAsync(string publicId);
    }
}
