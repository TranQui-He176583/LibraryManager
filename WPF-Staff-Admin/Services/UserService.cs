using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using WPF_Staff_Admin.Models;

namespace WPF_Staff_Admin.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public UserService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
        }

        public async Task<ApiResponse<IEnumerable<UserDTO>>> GetAllUsersAsync(string? role = null)
        {
            try
            {
                var url = $"{_baseUrl}/api/Users";
                if (!string.IsNullOrEmpty(role))
                {
                    url += $"?role={role}";
                }
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>();
                    return new ApiResponse<IEnumerable<UserDTO>> { Success = true, Data = users };
                }
                return new ApiResponse<IEnumerable<UserDTO>> { Success = false, Message = "Không thể tải danh sách người dùng" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<UserDTO>> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<IEnumerable<UserDTO>>> SearchUsersAsync(string term)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/Users/search?term={term}");
                if (response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>();
                    return new ApiResponse<IEnumerable<UserDTO>> { Success = true, Data = users };
                }
                return new ApiResponse<IEnumerable<UserDTO>> { Success = false, Message = "Lỗi khi tìm kiếm" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<UserDTO>> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse<UserDTO>> CreateUserAsync(CreateUserRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/Users", request);
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDTO<UserDTO>>();
                return new ApiResponse<UserDTO>
                {
                    Success = result?.Success ?? false,
                    Message = result?.Message,
                    Data = result?.Data
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<UserDTO> { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse> UpdateUserAsync(int userId, UserUpdateDTO dto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/api/Users/manage/{userId}", dto);
                return await response.Content.ReadFromJsonAsync<ApiResponse>() ?? new ApiResponse { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse> ToggleUserStatusAsync(int userId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{_baseUrl}/api/Users/{userId}/toggle-status", null);
                return await response.Content.ReadFromJsonAsync<ApiResponse>() ?? new ApiResponse { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<ApiResponse> ExtendMembershipAsync(int userId, int months)
        {
            try
            {
                var request = new ExtendMembershipRequest { Months = months };
                var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/api/Users/{userId}/extend-membership", request);
                return await response.Content.ReadFromJsonAsync<ApiResponse>() ?? new ApiResponse { Success = false };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = ex.Message };
            }
        }
    }

    // Temporary helper class for ReadFromJsonAsync of Server's ApiResponseDTO
    public class ApiResponseDTO<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}
