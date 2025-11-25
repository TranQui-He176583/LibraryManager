using System.Net.Http;
using System.IO;
using Microsoft.Extensions.Configuration;
using WPF_Staff_Admin.Models;
namespace WPF_Staff_Admin.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public FileUploadService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
        }

        public async Task<ApiResponse<string>> UploadBookImageAsync(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Message = "File không tồn tại"
                    };
                }

                using var form = new MultipartFormDataContent();
                using var fileStream = File.OpenRead(filePath);
                using var fileContent = new StreamContent(fileStream);

                var fileName = Path.GetFileName(filePath);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                form.Add(fileContent, "file", fileName);

                var response = await _httpClient.PostAsync($"{_baseUrl}/api/Books/book-image", form);
                var jsonString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Message = $"Upload thất bại: {response.StatusCode}",
                        Error = jsonString
                    };
                }

                var apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<string>>(
                    jsonString,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                if (apiResponse != null && apiResponse.Success && apiResponse.Data != null)
                {
                    apiResponse.Data = $"{_baseUrl}{apiResponse.Data}";
                }

                return apiResponse ?? new ApiResponse<string> { Success = false, Message = "Không thể parse response" };
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Lỗi upload ảnh",
                    Error = ex.Message
                };
            }
        }

        public async Task<ApiResponse> DeleteBookImageAsync(int? bookId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(
                    $"{_baseUrl}/api/Books/book-image?bookId={bookId}"
                );

                var jsonString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = $"Xóa ảnh thất bại: {response.StatusCode}"
                    };
                }

                var apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiResponse>(
                    jsonString,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                return apiResponse ?? new ApiResponse { Success = false, Message = "Không thể parse response" };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Lỗi xóa ảnh",
                    Error = ex.Message
                };
            }
        }
    }
}
