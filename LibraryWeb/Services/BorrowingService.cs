using LibraryWeb.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace LibraryWeb.Services
{
    public class BorrowingService : IBorrowingService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public BorrowingService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            var baseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
            _apiBaseUrl = $"{baseUrl}/api";
        }

        public async Task<BorrowingTicketViewModel?> CreateBorrowingAsync(CreateBorrowingViewModel model, int userId)
        {
            try
            {
                var borrowingDto = new
                {
                    memberId = userId,
                    librarianId = 2,
                    bookIds = new[] { model.BookId },
                    dueDate = DateTime.Now.AddDays(14).ToString("yyyy-MM-dd"),
                    notes = model.Notes
                };

                var json = JsonSerializer.Serialize(borrowingDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/Borrowings", content);

                if (!response.IsSuccessStatusCode) return null;

                var responseJson = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<BorrowingTicketViewModel>>(
                    responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return apiResponse?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi CreateBorrowing: {ex.Message}");
                return null;
            }
        }

        public async Task<List<BorrowingTicketViewModel>> GetMyBorrowingsAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Borrowings/member/{userId}");
                if (!response.IsSuccessStatusCode) return new List<BorrowingTicketViewModel>();

                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<BorrowingTicketViewModel>>>(
                    json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return apiResponse?.Data ?? new List<BorrowingTicketViewModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi GetMyBorrowings: {ex.Message}");
                return new List<BorrowingTicketViewModel>();
            }
        }

        public async Task<bool> ReturnBookAsync(int ticketId, int bookId)
        {
            try
            {
                var returnDto = new { ticketId, bookId };
                var json = JsonSerializer.Serialize(returnDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/Borrowings/return", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi ReturnBook: {ex.Message}");
                return false;
            }
        }
    }
}
