using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Staff_Admin.Models;
namespace WPF_Staff_Admin.Services
{
    public interface IBorrowingService
    {
        Task<List<BorrowingTicketDTO>> GetAllBorrowingsAsync();
        Task<List<BorrowingTicketDTO>> GetBorrowingsByMemberAsync(int memberId);
        Task<List<BorrowingTicketDTO>> GetPendingBorrowingsAsync();
        Task<BorrowingTicketDTO?> GetBorrowingByIdAsync(int ticketId);

        Task<ApiResponse<BorrowingTicketDTO>> CreateBorrowingAsync(CreateBorrowingRequest request);

        Task<ApiResponse> ApproveBorrowingAsync(ApproveBorrowingRequest request);
        Task<ApiResponse> RejectBorrowingAsync(RejectBorrowingRequest request);

        Task<ApiResponse> ReturnBookAsync(ReturnBookRequest request);

        Task<ApiResponse> UpdateOverdueStatusAsync();
    }
}
