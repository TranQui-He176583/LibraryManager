using Server.DTOs;

namespace Server.Services
{
    public interface IBorrowingService
    {
        Task<BorrowingTicketResponseDTO> CreateBorrowingAsync(BorrowingCreateDTO borrowingDto);
        Task<BorrowingDetailResponseDTO> ReturnBookByTicketAndBookAsync(int ticketId, int bookId);
        Task<List<BorrowingTicketResponseDTO>> GetMemberBorrowingsAsync(int memberId);
        Task<List<BorrowingTicketResponseDTO>> GetAllBorrowingsAsync();
        Task<List<BorrowingTicketResponseDTO>> GetPendingBorrowingsAsync();
        Task<BorrowingTicketResponseDTO?> GetBorrowingByIdAsync(int ticketId);
        Task<ApiResponse> ApproveBorrowingAsync(ApproveBorrowingDTO dto);
        Task<ApiResponse> RejectBorrowingAsync(RejectBorrowingDTO dto);

        Task UpdateOverdueStatusAsync();
        Task<ApiResponse> ReportLostOrDamagedAsync(ReportIssueDTO dto);
    }
}
