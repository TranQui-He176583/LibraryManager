using Microsoft.EntityFrameworkCore;
using Server.DTOs;
using Server.Models;

namespace Server.Services
{
    public class FineService : IFineService
    {
        private readonly LibraryManagementDbContext _context;

        public FineService(LibraryManagementDbContext context)
        {
            _context = context;
        }

        public async Task<List<FineDTO>> GetAllFinesAsync()
        {
            var fines = await _context.Fines
                .Include(f => f.BorrowDetail)
                    .ThenInclude(bd => bd.Ticket)
                        .ThenInclude(t => t.Member)
                .Include(f => f.BorrowDetail)
                    .ThenInclude(bd => bd.Book)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return fines.Select(MapToDTO).ToList();
        }

        public async Task<List<FineDTO>> GetFinesByMemberAsync(int memberId)
        {
            var fines = await _context.Fines
                .Include(f => f.BorrowDetail)
                    .ThenInclude(bd => bd.Ticket)
                .Where(f => f.BorrowDetail.Ticket.MemberId == memberId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return fines.Select(MapToDTO).ToList();
        }

        public async Task<FineDTO?> GetFineByIdAsync(int fineId)
        {
            var fine = await _context.Fines
                .Include(f => f.BorrowDetail)
                .FirstOrDefaultAsync(f => f.FineId == fineId);

            return fine != null ? MapToDTO(fine) : null;
        }

        public async Task<bool> MarkAsPaidAsync(int fineId)
        {
            var fine = await _context.Fines.FindAsync(fineId);
            if (fine == null) return false;

            fine.IsPaid = true;
            fine.PaidDate = DateTime.Now;
            
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<FineDTO> CreateFineAsync(FineDTO dto)
        {
            var fine = new Fine
            {
                BorrowDetailId = dto.BorrowDetailId,
                Amount = dto.Amount,
                Reason = dto.Reason,
                IsPaid = dto.IsPaid,
                PaidDate = dto.PaidDate,
                CreatedAt = DateTime.Now,
                Notes = dto.Notes
            };

            _context.Fines.Add(fine);
            await _context.SaveChangesAsync();
            
            return MapToDTO(fine);
        }

        private static FineDTO MapToDTO(Fine f)
        {
            return new FineDTO
            {
                FineId = f.FineId,
                BorrowDetailId = f.BorrowDetailId,
                Amount = f.Amount,
                Reason = f.Reason,
                IsPaid = f.IsPaid,
                PaidDate = f.PaidDate,
                CreatedAt = f.CreatedAt,
                Notes = f.Notes,
                MemberName = f.BorrowDetail?.Ticket?.Member?.FullName,
                BookTitle = f.BorrowDetail?.Book?.Title
            };
        }
    }
}
