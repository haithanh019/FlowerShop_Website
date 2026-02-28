using FlowerShop.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace FlowerShop.Infrastructure.Repositories.Interfaces
{
    public interface IFlowerImageRepository : IGenericRepository<FlowerImage>
    {
        Task UploadImageAsync(IFormFile file, string folder, FlowerImage image);
        Task<bool> DeleteImageAsync(string publicId);
        Task<bool> DeleteImagesAsync(List<string> publicIds);
    }
}
