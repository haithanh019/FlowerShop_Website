using FlowerShop.Utility;

namespace FlowerShop.Client
{
    public interface IBaseService
    {
        Task<T?> GetODataAsync<T>(string endpoint, string? token = null);
        Task<ApiResponse<T>> GetAsync<T>(string endpoint, string? token = null);
        Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data, string? token = null);
        Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data, string? token = null);
        Task<ApiResponse<T>> DeleteAsync<T>(string endpoint, string? token = null);
    }
}
