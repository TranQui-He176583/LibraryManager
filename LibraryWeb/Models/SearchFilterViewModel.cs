using Microsoft.AspNetCore.Mvc.Rendering;
namespace LibraryWeb.Models
{
    public class SearchFilterViewModel
    {
        public string? SearchQuery { get; set; }
        public int? AuthorId { get; set; }
        public int? CategoryId { get; set; }
        public string? Language { get; set; }
        public string? SortBy { get; set; } = "title";
        public string? SortOrder { get; set; } = "asc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public List<SelectListItem>? Authors { get; set; }
        public List<SelectListItem>? Categories { get; set; }
        public List<SelectListItem>? Languages { get; set; }
    }

    public class SearchResultViewModel
    {
        public List<BookDetailViewModel> Books { get; set; } = new();
        public SearchFilterViewModel Filters { get; set; } = new();
        public PaginationViewModel Pagination { get; set; } = new();

    }
    public class PaginationViewModel
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
    }
    public class SearchResultDTO
    {
        public List<BookDetailViewModel> Books { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; }
    }
}
