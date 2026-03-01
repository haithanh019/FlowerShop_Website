using FlowerShop.Domain;
using Microsoft.AspNetCore.Http;

namespace FlowerShop.Infrastructure
{
    public interface IFlowerImageRepository : IGenericRepository<FlowerImage>
    {
        Task UploadImageAsync(IFormFile file, string folder, FlowerImage image);
        Task<bool> DeleteImageAsync(string publicId);
        Task<bool> DeleteImagesAsync(List<string> publicIds);
    }
}
