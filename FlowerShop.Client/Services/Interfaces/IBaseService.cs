using FlowerShop.Utility;

namespace FlowerShop.Client
{
    public interface IBaseService
    {
        Task<T?> GetODataAsync<T>(string endpoint, string? token = null);
        Task<ApiResult<T>> GetAsync<T>(string endpoint, string? token = null);
        Task<ApiResult<T>> PostAsync<T>(string endpoint, object data, string? token = null);
        Task<ApiResult<T>> PostMultipartAsync<T>(string endpoint, MultipartFormDataContent content, string? token = null);
        Task<ApiResult<T>> PutAsync<T>(string endpoint, object data, string? token = null);
        Task<ApiResult<T>> PutMultipartAsync<T>(string endpoint, MultipartFormDataContent content, string? token = null);
        Task<ApiResult<T>> DeleteAsync<T>(string endpoint, string? token = null);
    }
}
