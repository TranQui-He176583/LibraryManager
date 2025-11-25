namespace Server.DTOs
{
    public class BorrowingCreateDTO
    {
        public int MemberId { get; set; }
        public int LibrarianId { get; set; }
        public List<int> BookIds { get; set; } = new();
        public int BorrowDays { get; set; } = 14;
        public string? Notes { get; set; }
    }
    public class BorrowingTicketResponseDTO
    {
        public int TicketId { get; set; }
        public string MemberName { get; set; } = null!;
        public string? MemberEmail { get; set; }
        public string LibrarianName { get; set; } = null!;
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public string? Notes { get; set; }
        public List<BorrowingDetailResponseDTO> Details { get; set; } = new();
        public int TotalBooks => Details.Count;
        public int BooksPending => Details.Count(d => d.Status == "Chờ duyệt");
        public int BooksNotReturned => Details.Count(d => d.Status != "Đã trả");
        public decimal TotalFines => Details.Sum(d => d.TotalFineAmount);
    }

    public class BorrowingDetailResponseDTO
    {
        public int DetailId { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = null!;
        public string? BookIsbn { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; } = null!;
        public string? Authors { get; set; }
        public decimal? BookPrice { get; set; }
        public int DaysOverdue { get; set; }
        public List<FineDTO> Fines { get; set; } = new();
        public string? BookImageUrl { get; set; }
        public DateTime BorrowDate { get; set; }  
        public DateTime DueDate { get; set; }     
        public decimal TotalFineAmount => Fines.Sum(f => f.Amount);
    }
    public class ReturnBookDTO
    {
        public int TicketId { get; set; }  
        public int BookId { get; set; }
        public DateTime? ReturnDate { get; set; }  
        public string? Notes { get; set; }       
    }
    public class FineDTO
    {
        public int FineId { get; set; }
        public int BorrowDetailId { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public DateTime? PaidDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Notes { get; set; }
    }
    public class ApproveBorrowingDTO
    {
        public int DetailId { get; set; }
        public int StaffId { get; set; }
    }

    public class RejectBorrowingDTO
    {
        public int DetailId { get; set; }
        public int StaffId { get; set; }
        public string? Reason { get; set; }
    }

}
