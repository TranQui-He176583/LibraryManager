using Server.DTOs;
using Server.Models;

namespace Server.DTOs
{
    public class BookDetailDTO ()
    {
        public int BookId { get; set; }
        public string Title { get; set; } = null!;
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
    }

    public class AuthorDTO
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = null!;
    }

    public class CategoryDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
    }

    public class SearchBooksRequestDTO
    {
        public string? SearchQuery { get; set; }   
        public int? AuthorId { get; set; }         
        public int? CategoryId { get; set; }        
        public string? Language { get; set; }       

        public string SortBy { get; set; } = "title";
        public string SortOrder { get; set; } = "asc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}
public class SearchBooksResultDTO
{
    public List<BookDetailDTO> Books { get; set; } = new();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public int PageSize { get; set; }
}
public class PublisherDTO
{
    public int PublisherId { get; set; }
    public string PublisherName { get; set; } = string.Empty;
    public string? Web { get; set; }
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
public class UpdateBookRequest
{
    public int BookId { get; set; }
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