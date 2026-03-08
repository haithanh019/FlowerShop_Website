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

        public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object data, string? token = null)
        {
            var client = _httpClientFactory.CreateClient("FlowerShop.API");
            client.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]!);

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(endpoint, content);

            var responseBody = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(responseBody, options);

            return apiResponse ?? new ApiResponse<T>("Error to connect with API");
        }
        public async Task<ApiResponse<T>> GetAsync<T>(string endpoint, string? token = null)
        {
            var client = _httpClientFactory.CreateClient("FlowerShop.API");
            client.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]!);

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync(endpoint);
            var responseBody = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(responseBody, options);

            return apiResponse ?? new ApiResponse<T>("Error to connect with API");
        }
    

        public async Task<T?> GetODataAsync<T>(string endpoint, string? token = null)
        {
            var client = _httpClientFactory.CreateClient("FlowerShop.API");
            client.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]!);

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

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
    }
}