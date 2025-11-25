using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Staff_Admin.Models
{
    public class BookDTO
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Isbn { get; set; }
        public int? PublisherId { get; set; }
        public string? PublisherName { get; set; }
        public int? PublicationYear { get; set; }
        public int TotalQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public decimal? Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? Language { get; set; }
        public string? Location { get; set; }
        public List<AuthorDTO>? Authors { get; set; }
        public string? Description { get; set; }
        public int? PageCount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<CategoryDTO>? Categories { get; set; }
        public string? Publisher { get; set; }

        public string AuthorsDisplay => Authors != null && Authors.Any()
            ? string.Join(", ", Authors.Select(a => a.AuthorName))
            : "N/A";

        public string CategoriesDisplay => Categories != null && Categories.Any()
            ? string.Join(", ", Categories.Select(c => c.CategoryName))
            : "N/A";

        public string QuantityDisplay => $"{AvailableQuantity}/{TotalQuantity}";
    }

    public class AuthorDTO
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
    }

    public class CategoryDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    public class PublisherDTO
    {
        public int PublisherId { get; set; }
        public string PublisherName { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
    }

    public class SearchBooksRequest
    {
        public string? SearchQuery { get; set; }
        public int? AuthorId { get; set; }
        public int? CategoryId { get; set; }
        public string? Language { get; set; }
        public string SortBy { get; set; } = "title";
        public string SortOrder { get; set; } = "asc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class SearchBooksResult
    {
        public List<BookDTO> Books { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; }
    }

    public class CreateBookRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Isbn { get; set; }
        public int? PublisherId { get; set; }
        public int? PublishedYear { get; set; }
        public int? PageCount { get; set; }
        public string? Language { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int TotalQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public decimal? Price { get; set; }
        public string? Location { get; set; }
        public List<int>? AuthorIds { get; set; }
        public List<int>? CategoryIds { get; set; }
    }

    public class UpdateBookRequest : CreateBookRequest
    {
        public int BookId { get; set; }
    }
}
