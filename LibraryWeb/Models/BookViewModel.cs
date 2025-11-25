namespace LibraryWeb.Models
{
    public class BookDetailViewModel
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
        public List<AuthorViewModel>? Authors { get; set; }
        public string? Description { get; set; }
        public int? PageCount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<CategoryViewModel>? Categories { get; set; }
        public string? Publisher { get; set; }
    }
    public class AuthorViewModel
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = null!;
    }

    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
    }
}
