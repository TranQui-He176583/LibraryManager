using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WPF_Staff_Admin.Models;
namespace WPF_Staff_Admin.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"] + "/api" ?? "http://localhost:5000/api";

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var timeoutSeconds = configuration.GetValue<int>("ApiSettings:Timeout", 30);
            _httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
        }

        public async Task<ApiResponse<T>> GetAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{endpoint}");
                return await ParseResponse<T>(response);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<T>(ex.Message);
            }
        }

        public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object? data = null)
        {
            try
            {
                var content = data != null
                    ? new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
                    : null;

                var response = await _httpClient.PostAsync($"{_baseUrl}/{endpoint}", content);
                return await ParseResponse<T>(response);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<T>(ex.Message);
            }
        }

        public async Task<ApiResponse<T>> PutAsync<T>(string endpoint, object data)
        {
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_baseUrl}/{endpoint}", content);
                return await ParseResponse<T>(response);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<T>(ex.Message);
            }
        }

        public async Task<ApiResponse<T>> DeleteAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{endpoint}");
                return await ParseResponse<T>(response);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse<T>(ex.Message);
            }
        }
    
        public async Task<ApiResponse> PostAsync(string endpoint, object? data = null)
        {
            try
            {
                var content = data != null
                    ? new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
                    : null;

                var response = await _httpClient.PostAsync($"{_baseUrl}/{endpoint}", content);
                return await ParseResponse(response);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(ex.Message);
            }
        }

        public async Task<ApiResponse> PutAsync(string endpoint, object data)
        {
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_baseUrl}/{endpoint}", content);
                return await ParseResponse(response);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(ex.Message);
            }
        }

        public async Task<ApiResponse> DeleteAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{endpoint}");
                return await ParseResponse(response);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(ex.Message);
            }
        }

        private async Task<ApiResponse<T>> ParseResponse<T>(HttpResponseMessage response)
        {
            var jsonString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<T>
                {
                    Success = false,
                    Message = $"HTTP {response.StatusCode}",
                    Error = jsonString
                };
            }

            var apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(jsonString, _jsonOptions);
            return apiResponse ?? new ApiResponse<T> { Success = false, Message = "Không thể parse response" };
        }

        private async Task<ApiResponse> ParseResponse(HttpResponseMessage response)
        {
            var jsonString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = $"HTTP {response.StatusCode}",
                    Error = jsonString
                };
            }

            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(jsonString, _jsonOptions);
            return apiResponse ?? new ApiResponse { Success = false, Message = "Không thể parse response" };
        }

        private ApiResponse<T> CreateErrorResponse<T>(string error)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = "Lỗi kết nối API",
                Error = error
            };
        }

        private ApiResponse CreateErrorResponse(string error)
        {
            return new ApiResponse
            {
                Success = false,
                Message = "Lỗi kết nối API",
                Error = error
            };
        }
    }


}
