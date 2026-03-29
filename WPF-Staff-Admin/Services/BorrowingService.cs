using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WPF_Staff_Admin.Models;
using System.Net.Http.Json;

namespace WPF_Staff_Admin.Services
{
    public class BorrowingService : IBorrowingService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public BorrowingService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
        }

        //public async Task<List<BorrowingTicketDTO>> GetAllBorrowingsAsync()
        //{
        //    try
        //    {
        //        var response = await _httpClient.GetAsync($"{_baseUrl}/api/Borrowings");

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            return new List<BorrowingTicketDTO>();
        //        }

        //        var borrowings = await response.Content.ReadFromJsonAsync<List<BorrowingTicketDTO>>(
        //            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        //        );

        //        return borrowings ?? new List<BorrowingTicketDTO>();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error getting borrowings: {ex.Message}");
        //        return new List<BorrowingTicketDTO>();
        //    }
        //}
        public async Task<List<BorrowingTicketDTO>> GetAllBorrowingsAsync()
        {
            try
            {
                Console.WriteLine($"=== BorrowingService.GetAllBorrowingsAsync START ===");
                Console.WriteLine($"API URL: {_baseUrl}/api/Borrowings");

                var response = await _httpClient.GetAsync($"{_baseUrl}/api/Borrowings");

                Console.WriteLine($"Response Status: {response.StatusCode}");
                Console.WriteLine($"Response IsSuccessStatusCode: {response.IsSuccessStatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Error response: {errorContent}");
                    return new List<BorrowingTicketDTO>();
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"✅ Response JSON length: {jsonString.Length} characters");
                Console.WriteLine($"First 200 chars: {jsonString.Substring(0, Math.Min(200, jsonString.Length))}");

                var borrowings = await response.Content.ReadFromJsonAsync<List<BorrowingTicketDTO>>(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                Console.WriteLine($"✅ Deserialized {borrowings?.Count ?? 0} items");

                return borrowings ?? new List<BorrowingTicketDTO>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"❌ HTTP Error: {ex.Message}");
                return new List<BorrowingTicketDTO>();
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"❌ JSON Error: {ex.Message}");
                return new List<BorrowingTicketDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected Error: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                return new List<BorrowingTicketDTO>();
            }
        }

        public async Task<List<BorrowingTicketDTO>> GetBorrowingsByMemberAsync(int memberId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/Borrowings/member/{memberId}");

                if (!response.IsSuccessStatusCode)
                {
                    return new List<BorrowingTicketDTO>();
                }

                var borrowings = await response.Content.ReadFromJsonAsync<List<BorrowingTicketDTO>>(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                return borrowings ?? new List<BorrowingTicketDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting borrowings by member: {ex.Message}");
                return new List<BorrowingTicketDTO>();
            }
        }

        public async Task<List<BorrowingTicketDTO>> GetPendingBorrowingsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/Borrowings/pending");

                if (!response.IsSuccessStatusCode)
                {
                    return new List<BorrowingTicketDTO>();
                }

                var borrowings = await response.Content.ReadFromJsonAsync<List<BorrowingTicketDTO>>(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                return borrowings ?? new List<BorrowingTicketDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting pending borrowings: {ex.Message}");
                return new List<BorrowingTicketDTO>();
            }
        }

        public async Task<BorrowingTicketDTO?> GetBorrowingByIdAsync(int ticketId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/Borrowings/{ticketId}");

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var borrowing = await response.Content.ReadFromJsonAsync<BorrowingTicketDTO>(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                return borrowing;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting borrowing by id: {ex.Message}");
                return null;
            }
        }

        public async Task<ApiResponse<BorrowingTicketDTO>> CreateBorrowingAsync(CreateBorrowingRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/Borrowings", request);
                var jsonString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = JsonSerializer.Deserialize<ApiResponse<BorrowingTicketDTO>>(
                        jsonString,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    return errorResponse ?? new ApiResponse<BorrowingTicketDTO>
                    {
                        Success = false,
                        Message = $"Lỗi: {response.StatusCode}"
                    };
                }

                var successResponse = JsonSerializer.Deserialize<BorrowingTicketDTO>(
                    jsonString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                return new ApiResponse<BorrowingTicketDTO>
                {
                    Success = true,
                    Message = "Tạo phiếu mượn thành công",
                    Data = successResponse
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<BorrowingTicketDTO>
                {
                    Success = false,
                    Message = "Lỗi kết nối API",
                    Error = ex.Message
                };
            }
        }

        public async Task<ApiResponse> ApproveBorrowingAsync(ApproveBorrowingRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/Borrowings/approve", request);
                var jsonString = await response.Content.ReadAsStringAsync();

                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(
                    jsonString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = $"Lỗi: {response.StatusCode}"
                    };
                }

                return apiResponse ?? new ApiResponse
                {
                    Success = true,
                    Message = "Duyệt mượn sách thành công"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Lỗi kết nối API",
                    Error = ex.Message
                };
            }
        }

        public async Task<ApiResponse> RejectBorrowingAsync(RejectBorrowingRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/Borrowings/reject", request);
                var jsonString = await response.Content.ReadAsStringAsync();

                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(
                    jsonString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (!response.IsSuccessStatusCode)
                {
                    return apiResponse ?? new ApiResponse
                    {
                        Success = false,
                        Message = $"Lỗi: {response.StatusCode}"
                    };
                }

                return apiResponse ?? new ApiResponse
                {
                    Success = true,
                    Message = "Từ chối mượn sách thành công"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Lỗi kết nối API",
                    Error = ex.Message
                };
            }
        }

        public async Task<ApiResponse> ReturnBookAsync(ReturnBookRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/Borrowings/return", request);
                var jsonString = await response.Content.ReadAsStringAsync();

                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(
                    jsonString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (!response.IsSuccessStatusCode)
                {
                    return apiResponse ?? new ApiResponse
                    {
                        Success = false,
                        Message = $"Lỗi: {response.StatusCode}"
                    };
                }

                return apiResponse ?? new ApiResponse
                {
                    Success = true,
                    Message = "Trả sách thành công"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Lỗi kết nối API",
                    Error = ex.Message
                };
            }
        }

        public async Task<ApiResponse> ReportIssueAsync(ReportIssueRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/Fines/report-issue", request);
                var jsonString = await response.Content.ReadAsStringAsync();

                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(
                    jsonString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (!response.IsSuccessStatusCode)
                {
                    return apiResponse ?? new ApiResponse
                    {
                        Success = false,
                        Message = $"Lỗi: {response.StatusCode}"
                    };
                }

                return apiResponse ?? new ApiResponse
                {
                    Success = true,
                    Message = "Báo cáo sự cố thành công"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Lỗi kết nối API",
                    Error = ex.Message
                };
            }
        }

        public async Task<ApiResponse> UpdateOverdueStatusAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync($"{_baseUrl}/api/Borrowings/update-overdue", null);
                var jsonString = await response.Content.ReadAsStringAsync();

                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(
                    jsonString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (!response.IsSuccessStatusCode)
                {
                    return apiResponse ?? new ApiResponse
                    {
                        Success = false,
                        Message = $"Lỗi: {response.StatusCode}"
                    };
                }

                return apiResponse ?? new ApiResponse
                {
                    Success = true,
                    Message = "Cập nhật trạng thái thành công"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Lỗi kết nối API",
                    Error = ex.Message
                };
            }
        }
    }
}
