using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Staff_Admin.Models;

namespace WPF_Staff_Admin.Services
{
   public class BookService : IBookService
    {
        private readonly IApiService _apiService;

        public BookService(IApiService apiService)
        {
            _apiService = apiService;
        }
        public async Task<ApiResponse<List<BookDTO>>> GetAllBooksAsync()
        {
            return await _apiService.GetAsync<List<BookDTO>>("Books");
        }

        public async Task<ApiResponse<BookDTO>> GetBookByIdAsync(int bookId)
        {
            return await _apiService.GetAsync<BookDTO>($"Books/{bookId}");
        }

        public async Task<ApiResponse<SearchBooksResult>> SearchBooksAsync(SearchBooksRequest request)
        {
            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(request.SearchQuery))
                queryParams.Add($"searchQuery={Uri.EscapeDataString(request.SearchQuery)}");

            if (request.AuthorId.HasValue)
                queryParams.Add($"authorId={request.AuthorId}");

            if (request.CategoryId.HasValue)
                queryParams.Add($"categoryId={request.CategoryId}");

            if (!string.IsNullOrEmpty(request.Language))
                queryParams.Add($"language={Uri.EscapeDataString(request.Language)}");

            queryParams.Add($"sortBy={request.SortBy}");
            queryParams.Add($"sortOrder={request.SortOrder}");
            queryParams.Add($"page={request.Page}");
            queryParams.Add($"pageSize={request.PageSize}");

            var queryString = string.Join("&", queryParams);
            return await _apiService.GetAsync<SearchBooksResult>($"Books/search?{queryString}");
        }

        public async Task<ApiResponse<List<string>>> GetLanguagesAsync()
        {
            return await _apiService.GetAsync<List<string>>("Books/languages");
        }

        public async Task<ApiResponse<BookDTO>> CreateBookAsync(CreateBookRequest request)
        {
            return await _apiService.PostAsync<BookDTO>("Books", request);
        }

        public async Task<ApiResponse<BookDTO>> UpdateBookAsync(UpdateBookRequest request)
        {
            return await _apiService.PutAsync<BookDTO>($"Books/{request.BookId}", request);
        }

        public async Task<ApiResponse> DeleteBookAsync(int bookId)
        {
            return await _apiService.DeleteAsync($"Books/Delete?bookId={bookId}");
        }


    }
}
