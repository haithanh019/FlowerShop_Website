using FlowerShop.Utility;

namespace FlowerShop.Client
{
    public interface IBaseService
    {
        Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data, string? token = null);
    }
}
