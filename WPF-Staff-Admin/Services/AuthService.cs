using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WPF_Staff_Admin.Models;
namespace WPF_Staff_Admin.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private UserDTO? _currentUser;
        private string? _token;

        public AuthService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:5000";
        }

        public bool IsAuthenticated => _currentUser != null && !string.IsNullOrEmpty(_token);

        public UserDTO? CurrentUser => _currentUser;

        public string? Token => _token;

        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest loginRequest)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/Auth/login", loginRequest);

                if (!response.IsSuccessStatusCode)
                {
                    // Thử parse error response
                    try
                    {
                        var errorResponse = await response.Content.ReadFromJsonAsync<ApiResponse>(
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        
                        return new ApiResponse<LoginResponse>
                        {
                            Success = false,
                            Message = errorResponse?.Message ?? $"Lỗi: {response.StatusCode}"
                        };
                    }
                    catch
                    {
                        return new ApiResponse<LoginResponse>
                        {
                            Success = false,
                            Message = $"Không kết nối được tới Server. Status: {response.StatusCode}"
                        };
                    }
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (apiResponse == null || apiResponse.Data == null)
                {
                    return new ApiResponse<LoginResponse>
                    {
                        Success = false,
                        Message = "Không nhận được dữ liệu từ Server"
                    };
                }

                var loginResponse = apiResponse.Data;
                _currentUser = new UserDTO
                {
                    UserId = loginResponse.UserId,
                    RoleId = loginResponse.RoleId,
                    RoleName = loginResponse.roleName,
                    FullName = loginResponse.FullName,
                    Username = loginResponse.UserName,
                    Email = loginResponse.Email,
                };
                
                if (!string.IsNullOrEmpty(loginResponse.Token))
                {
                    _token = loginResponse.Token;
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
                }

                return new ApiResponse<LoginResponse>
                {
                    Success = true,
                    Message = apiResponse.Message ?? "Đăng nhập thành công!",
                    Data = loginResponse,
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                return new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}",
                };
            }
        }

        public Task LogoutAsync()
        {
            _currentUser = null;
            _token = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
            return Task.CompletedTask;
        }
    }
}
