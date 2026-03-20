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
        private static async Task<ApiResponse<T>> ReadResponseAsync<T>(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            if (string.IsNullOrWhiteSpace(responseBody))
            {
                return new ApiResponse<T>($"API trả về phản hồi rỗng. HTTP {(int)response.StatusCode}");
            }

            try
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(responseBody, options);
                return apiResponse ?? new ApiResponse<T>("Không parse được phản hồi từ API.");
            }
            catch (JsonException ex)
            {
                return new ApiResponse<T>($"JSON parse error: {ex.Message} | Body: {responseBody[..Math.Min(200, responseBody.Length)]}");
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
        public async Task<ApiResponse<T>> GetAsync<T>(string endpoint, string? token = null)
        {
            var client = CreateClient(token);
            var response = await client.GetAsync(endpoint);
            return await ReadResponseAsync<T>(response);
        }
        public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data, string? token = null)
        {
            var client = CreateClient(token);
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(endpoint, content);
            return await ReadResponseAsync<T>(response);
        }
        public async Task<ApiResponse<T>> PostMultipartAsync<T>(string endpoint, MultipartFormDataContent content, string? token = null)
        {
            var client = CreateClient(token);
            var response = await client.PostAsync(endpoint, content);
            return await ReadResponseAsync<T>(response);
        }

        public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data, string? token = null)
        {
            var client = CreateClient(token);
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await client.PutAsync(endpoint, content);
            return await ReadResponseAsync<T>(response);
        }

        public async Task<ApiResponse<T>> DeleteAsync<T>(string endpoint, string? token = null)
        {
            var client = CreateClient(token);
            var response = await client.DeleteAsync(endpoint);
            return await ReadResponseAsync<T>(response);
        }
    }
}