using LibraryWeb.Models;
namespace LibraryWeb.Services
 
{
    public interface IBorrowingService
    {
        Task<BorrowingTicketViewModel?> CreateBorrowingAsync(CreateBorrowingViewModel model, int userId);
        Task<List<BorrowingTicketViewModel>> GetMyBorrowingsAsync(int userId);
        Task<bool> ReturnBookAsync(int ticketId, int bookId);
    }
}
