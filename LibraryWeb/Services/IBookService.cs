using LibraryWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace LibraryWeb.Services
{
    public interface IBookService
    {
        Task <BookDetailViewModel> GetBookDetailAsync(int id);
        Task<List<BookDetailViewModel>> GetBooksAsync();

        Task<SearchResultViewModel> SearchBooksAsync(SearchFilterViewModel filters);
        Task<List<SelectListItem>> GetAuthorsForDropdownAsync();
        Task<List<SelectListItem>> GetCategoriesForDropdownAsync();
        Task<List<SelectListItem>> GetLanguagesForDropdownAsync();
    }
}
