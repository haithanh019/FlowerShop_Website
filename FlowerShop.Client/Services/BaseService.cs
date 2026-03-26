using FlowerShop.Utility;
using FlowerShop.Utility.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FlowerShop.Client
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public BaseService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        // Helper method to create HttpClient with optional token
        private HttpClient CreateClient(string? token = null)
        {
            var client = _httpClientFactory.CreateClient("FlowerShop.API");
            client.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]!);

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }
        private static async Task<ApiResult<T>> ReadResponseAsync<T>(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            if (string.IsNullOrWhiteSpace(responseBody))
            {
                return new ApiResult<T>($"API trả về phản hồi rỗng. HTTP {(int)response.StatusCode}");
            }

            try
            {
                var ApiResult = JsonSerializer.Deserialize<ApiResult<T>>(responseBody, options);
                return ApiResult ?? new ApiResult<T>("Không parse được phản hồi từ API.");
            }
            catch (JsonException ex)
            {
                return new ApiResult<T>($"JSON parse error: {ex.Message} | Body: {responseBody[..Math.Min(200, responseBody.Length)]}");
            }
        }
        //=======================================================================================================
        public async Task<T?> GetODataAsync<T>(string endpoint, string? token = null)
        {
            var client = CreateClient(token);
            var response = await client.GetAsync(endpoint);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(responseBody) || !response.IsSuccessStatusCode)
            {
                return default;
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            try
            {
                var odataResult = JsonSerializer.Deserialize<ODataResponse<T>>(responseBody, options);
                return odataResult != null ? odataResult.Value : default;
            }
            catch (JsonException)
            {
                return default;
            }
        }
        public async Task<ApiResult<T>> GetAsync<T>(string endpoint, string? token = null)
        {
            var client = CreateClient(token);
            var response = await client.GetAsync(endpoint);
            return await ReadResponseAsync<T>(response);
        }
        public async Task<ApiResult<T>> PostAsync<T>(string endpoint, object data, string? token = null)
        {
            var client = CreateClient(token);
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(endpoint, content);
            return await ReadResponseAsync<T>(response);
        }
        public async Task<ApiResult<T>> PostMultipartAsync<T>(string endpoint, MultipartFormDataContent content, string? token = null)
        {
            var client = CreateClient(token);
            var response = await client.PostAsync(endpoint, content);
            return await ReadResponseAsync<T>(response);
        }
        public async Task<ApiResult<T>> PutAsync<T>(string endpoint, object data, string? token = null)
        {
            var client = CreateClient(token);
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await client.PutAsync(endpoint, content);
            return await ReadResponseAsync<T>(response);
        }
        public async Task<ApiResult<T>> PutMultipartAsync<T>(string endpoint, MultipartFormDataContent content, string? token = null)
        {
            var client = CreateClient(token);
            var response = await client.PutAsync(endpoint, content);
            return await ReadResponseAsync<T>(response);
        }
        public async Task<ApiResult<T>> DeleteAsync<T>(string endpoint, string? token = null)
        {
            var client = CreateClient(token);
            var response = await client.DeleteAsync(endpoint);
            return await ReadResponseAsync<T>(response);
        }
    }
}