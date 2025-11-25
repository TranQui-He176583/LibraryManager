using LibraryWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace LibraryWeb.Services
{
    public class BookService : IBookService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public BookService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            var baseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
            _apiBaseUrl = $"{baseUrl}/api";
        }

        public async Task<List<BookDetailViewModel>> GetBooksAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Books");

                if (!response.IsSuccessStatusCode)
                    return new List<BookDetailViewModel>();

                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<BookDetailViewModel>>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return apiResponse?.Data ?? new List<BookDetailViewModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi GetBooks: {ex.Message}");
                return new List<BookDetailViewModel>();
            }
        }

        public async Task<BookDetailViewModel?> GetBookDetailAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Books/{id}");
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<BookDetailViewModel>>(
                    json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                Console.WriteLine(id);
                Console.WriteLine(apiResponse?.Data?.BookId);

                Console.WriteLine($"{_apiBaseUrl}/Books/{id}");
                Console.WriteLine();
                return apiResponse?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }


        public async Task<SearchResultViewModel> SearchBooksAsync(SearchFilterViewModel filters)
        {
            try
            {
                var queryParams = new List<string>();

                if (!string.IsNullOrEmpty(filters.SearchQuery))
                    queryParams.Add($"searchQuery={Uri.EscapeDataString(filters.SearchQuery)}");

                if (filters.AuthorId.HasValue)
                    queryParams.Add($"authorId={filters.AuthorId}");

                if (filters.CategoryId.HasValue)
                    queryParams.Add($"categoryId={filters.CategoryId}");

                if (!string.IsNullOrEmpty(filters.Language))
                    queryParams.Add($"language={Uri.EscapeDataString(filters.Language)}");

                if (!string.IsNullOrEmpty(filters.SortBy))
                    queryParams.Add($"sortBy={filters.SortBy}");

                if (!string.IsNullOrEmpty(filters.SortOrder))
                    queryParams.Add($"sortOrder={filters.SortOrder}");

                queryParams.Add($"page={filters.Page}");
                queryParams.Add($"pageSize={filters.PageSize}");

                var queryString = string.Join("&", queryParams);
                var url = $"{_apiBaseUrl}/Books/search?{queryString}";

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return new SearchResultViewModel();
                }

                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<SearchResultDTO>>(
                    json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (apiResponse?.Data == null)
                {
                    return new SearchResultViewModel();
                }

                var result = new SearchResultViewModel
                {
                    Books = apiResponse.Data.Books.Select(b => new BookDetailViewModel
                    {
                        BookId = b.BookId,
                        Title = b.Title,
                        Isbn = b.Isbn,
                        PublisherName = b.PublisherName,
                        PublicationYear = b.PublicationYear,
                        TotalQuantity = b.TotalQuantity,
                        AvailableQuantity = b.AvailableQuantity,
                        Price = b.Price,
                        ImageUrl = b.ImageUrl,
                        Language = b.Language,
                        Location = b.Location
                    }).ToList(),

                    Filters = filters,

                    Pagination = new PaginationViewModel
                    {
                        CurrentPage = apiResponse.Data.CurrentPage,
                        TotalPages = apiResponse.Data.TotalPages,
                        TotalItems = apiResponse.Data.TotalItems,
                        PageSize = apiResponse.Data.PageSize
                    }
                };

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi SearchBooks: {ex.Message}");
                return new SearchResultViewModel();
            }
        }

        public async Task<List<SelectListItem>> GetAuthorsForDropdownAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Author");
                if (!response.IsSuccessStatusCode)
                    return new List<SelectListItem>();

                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<AuthorViewModel>>>(
                    json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (apiResponse?.Data == null)
                    return new List<SelectListItem>();

                var items = apiResponse.Data.Select(a => new SelectListItem
                {
                    Value = a.AuthorId.ToString(),
                    Text = a.AuthorName
                }).ToList();
                foreach (var i in items)
                {
                    Console.WriteLine("zz"+i.Text+" id = "+i.Value);
                }
                items.Insert(0, new SelectListItem { Value = "", Text = "-- Tất cả tác giả --" });

                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi GetAuthors: {ex.Message}");
                return new List<SelectListItem>();
            }
        }

        public async Task<List<SelectListItem>> GetCategoriesForDropdownAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Category");
                if (!response.IsSuccessStatusCode)
                    return new List<SelectListItem>();

                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<CategoryViewModel>>>(
                    json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (apiResponse?.Data == null)
                    return new List<SelectListItem>();

                var items = apiResponse.Data.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList();

                items.Insert(0, new SelectListItem { Value = "", Text = "-- Tất cả thể loại --" });
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi GetCategories: {ex.Message}");
                return new List<SelectListItem>();
            }
        }

        public async Task<List<SelectListItem>> GetLanguagesForDropdownAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/Books/languages");
                if (!response.IsSuccessStatusCode)
                    return new List<SelectListItem>();

                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<string>>>(
                    json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (apiResponse?.Data == null)
                    return new List<SelectListItem>();

                var items = apiResponse.Data.Select(l => new SelectListItem
                {
                    Value = l,
                    Text = l
                }).ToList();

                foreach (var i in items)
                {
                    Console.WriteLine("zz" + i.Text + " id = " + i.Value);
                }

                items.Insert(0, new SelectListItem { Value = "", Text = "-- Tất cả ngôn ngữ --" });
                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi GetLanguages: {ex.Message}");
                return new List<SelectListItem>();
            }
        }

    }
}
