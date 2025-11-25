using LibraryWeb.Models;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace LibraryWeb.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public ApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            var baseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
            _apiBaseUrl = $"{baseUrl}/api";
        }

        public async Task<LoginResponseViewModel?> LoginAsync(LoginViewModel loginModel)
        {
            try
            {
                var json = JsonSerializer.Serialize(loginModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/Auth/login", content);

                if (!response.IsSuccessStatusCode)
                    return null;

                var responseJson = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<LoginResponseViewModel>>(responseJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return apiResponse?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi Login: {ex.Message}");
                return null;
            }
        }

        public async Task<ProfileViewModel?> GetProfileAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Users/{userId}");
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<ProfileViewModel>>(
                    json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return apiResponse?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi GetProfile: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateProfileAsync(int userId, UpdateProfileViewModel model)
        {
            try
            {
                var updateDto = new
                {
                    model.FullName,
                    model.Email,
                    model.PhoneNumber,
                    model.Address,
                    model.DateOfBirth
                };

                var json = JsonSerializer.Serialize(updateDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/Users/{userId}", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi UpdateProfile: {ex.Message}");
                return false;
            }
        }
        public async Task<uploadImageRespone?> UpdateProfileWithImageAsync(int userId, UpdateProfileViewModel model)
        {
            try
            {
                string? newImageUrl = null;
                if (model.ProfileImage != null)
                {
                    var uploadResponse = await UploadProfileImageAsync(userId, model.ProfileImage);
                    if (uploadResponse == null || !uploadResponse.Success)
                    {
                        return uploadResponse;
                    }
                    else
                    {
                        newImageUrl = uploadResponse.Message;
                    }
                }

                if (model.RemoveImage)
                {
                    await RemoveProfileImageAsync(userId);
                }

                var updateDto = new
                {
                    model.FullName,
                    model.Email,
                    model.PhoneNumber,
                    model.Address,
                    model.DateOfBirth,
                    ImageUrl = model.RemoveImage ? null : (newImageUrl ?? model.CurrentImageUrl)
                };

                var json = JsonSerializer.Serialize(updateDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/Users/{userId}", content);
                var responseJson = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<uploadImageRespone>(responseJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return apiResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi UpdateProfileWithImage: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordViewModel model)
        {
            try
            {
                var changePasswordDto = new
                {
                    model.CurrentPassword,
                    model.NewPassword
                };

                var json = JsonSerializer.Serialize(changePasswordDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/Users/{userId}/change-password", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi ChangePassword: {ex.Message}");
                return false;
            }
        }
        public async Task<BorrowingStatsViewModel?> GetBorrowingStatsAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Users/{userId}/stats");
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<BorrowingStatsViewModel>>(
                    json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return apiResponse?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi GetBorrowingStats: {ex.Message}");
                return null;
            }
        }
        public async Task<uploadImageRespone?> UploadProfileImageAsync(int userId, IFormFile imageFile)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                using var fileStream = imageFile.OpenReadStream();
                using var streamContent = new StreamContent(fileStream);

                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imageFile.ContentType);
                content.Add(streamContent, "file", imageFile.FileName);

                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/Users/{userId}/upload-image", content);

                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<uploadImageRespone>(
                    json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Nếu deserialize thất bại, trả về response với Success = false
                if (apiResponse == null)
                {
                    return new uploadImageRespone
                    {
                        Success = false,
                        Message = "Không thể đọc phản hồi từ server"
                    };
                }

                // Trả về response dù thành công hay thất bại để có thể kiểm tra Success
                return apiResponse; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi UploadProfileImage: {ex.Message}");
                return new uploadImageRespone
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<bool> RemoveProfileImageAsync(int userId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/Users/{userId}/remove-image");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi RemoveProfileImage: {ex.Message}");
                return false;
            }
        }




    }
}