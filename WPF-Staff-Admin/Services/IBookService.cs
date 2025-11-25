using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Staff_Admin.Models;

namespace WPF_Staff_Admin.Services
{
    public interface IBookService
    {
        Task<ApiResponse<List<BookDTO>>> GetAllBooksAsync();
        Task<ApiResponse<BookDTO>> GetBookByIdAsync(int bookId);
        Task<ApiResponse<SearchBooksResult>> SearchBooksAsync(SearchBooksRequest request);
        Task<ApiResponse<List<string>>> GetLanguagesAsync();
        Task<ApiResponse<BookDTO>> CreateBookAsync(CreateBookRequest request);
        Task<ApiResponse<BookDTO>> UpdateBookAsync(UpdateBookRequest request);
        Task<ApiResponse> DeleteBookAsync(int bookId);
    }
}
