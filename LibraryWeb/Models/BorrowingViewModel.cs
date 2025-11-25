using System.ComponentModel.DataAnnotations;

namespace LibraryWeb.Models
{
    public class CreateBorrowingViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn sách")]
        public int BookId { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string? Notes { get; set; }

        public string? BookTitle { get; set; }
        public string? BookIsbn { get; set; }
        public decimal? BookPrice { get; set; }
    }

    public class BorrowingTicketViewModel
    {
        public int TicketId { get; set; }
        public int LibrarianId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public string? Notes { get; set; }
        public string? MemberName { get; set; }
        public string? MemberEmail { get; set; }
        public List<BorrowingDetailViewModel> Details { get; set; } = new();
    }
    public class BorrowingDetailViewModel
    {
        public int DetailId { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = null!;
        public string? BookIsbn { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; } = null!;
        public string? Authors    { get; set; }
        public int DaysOverdue { get; set; }
        public decimal? BookPrice { get; set; }
    }
}
