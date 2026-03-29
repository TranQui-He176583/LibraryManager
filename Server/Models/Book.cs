using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models;

public partial class Book
{
    [Key]
    public int BookId { get; set; }

    [StringLength(20)]
    [Column("ISBN")]
    public string? Isbn { get; set; }

    [Required]
    [StringLength(300)]
    public string Title { get; set; } = null!;

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

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BorrowingDetail> BorrowingDetails { get; set; } = new List<BorrowingDetail>();

    public virtual Publisher? Publisher { get; set; }

    public virtual ICollection<Author> Authors { get; set; } = new List<Author>();

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
