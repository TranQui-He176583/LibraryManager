using Microsoft.EntityFrameworkCore;
using Server.DTOs;
using Server.Models;
namespace Server.Services
{
    public class BorrowingService : IBorrowingService
    {
        private readonly LibraryManagementDbContext _context;

        public BorrowingService(LibraryManagementDbContext context)
        {
            _context = context;
        }

        public async Task<BorrowingTicketResponseDTO> CreateBorrowingAsync(BorrowingCreateDTO borrowingDto)
        {
           
            var member = await _context.Members
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.UserId == borrowingDto.MemberId);

            if (member == null)
                throw new Exception("Không tìm thấy thành viên");

            if (member.User == null || !member.User.IsActive)
                throw new Exception("Tài khoản không hoạt động");

            
            if (member.MembershipExpiry.HasValue &&
                member.MembershipExpiry < DateOnly.FromDateTime(DateTime.Now))
                throw new Exception("Thẻ thành viên đã hết hạn");

            
            if (member.SuspendedAt.HasValue)
                throw new Exception("Tài khoản đang bị khóa");

           
            var currentBorrowCount = await _context.BorrowingDetails
                .Include(bd => bd.Ticket)
                .CountAsync(bd => bd.Ticket.MemberId == borrowingDto.MemberId &&
                                  bd.Status == "Đang mượn");

            if (currentBorrowCount + borrowingDto.BookIds.Count > member.MaxBorrowLimit)
                throw new Exception($"Vượt quá giới hạn mượn sách ({member.MaxBorrowLimit} cuốn)");

            
            foreach (var bookId in borrowingDto.BookIds)
            {
                var book = await _context.Books.FindAsync(bookId);
                if (book == null)
                    throw new Exception($"Không tìm thấy sách ID {bookId}");

                if (book.AvailableQuantity <= 0)
                    throw new Exception($"Sách '{book.Title}' hiện không có sẵn");
            }

           
            var ticket = new BorrowingTicket
            {
                MemberId = borrowingDto.MemberId,
                LibrarianId = borrowingDto.LibrarianId,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(borrowingDto.BorrowDays),
                Notes = borrowingDto.Notes
            };

            _context.BorrowingTickets.Add(ticket);
            await _context.SaveChangesAsync();

            
            foreach (var bookId in borrowingDto.BookIds)
            {
                var detail = new BorrowingDetail
                {
                    TicketId = ticket.TicketId,
                    BookId = bookId,
                    Status = "Chờ duyệt"
                };

                _context.BorrowingDetails.Add(detail);

               
                var book = await _context.Books.FindAsync(bookId);
                if (book != null)
                {
                    book.AvailableQuantity--;
                    book.UpdatedAt = DateTime.Now;
                }
            }

            await _context.SaveChangesAsync();

           
            return await GetBorrowingByIdAsync(ticket.TicketId);
        }

        public async Task<BorrowingDetailResponseDTO> ReturnBookByTicketAndBookAsync(int ticketId, int bookId)
        {
            
            var detail = await _context.BorrowingDetails
                .Include(bd => bd.Ticket)
                .Include(bd => bd.Book)
                .Where(bd => bd.Ticket.TicketId == ticketId &&
                             bd.BookId == bookId &&
                             bd.Status == "Đang mượn")
                .OrderBy(bd => bd.Ticket.BorrowDate) 
                .FirstOrDefaultAsync();

            if (detail == null)
                throw new Exception($"User không đang mượn sách này");

          
            detail.ReturnDate = DateTime.Now;
            detail.Status = "Đã trả";

            if (detail.Book != null)
            {
                detail.Book.AvailableQuantity++;
                detail.Book.UpdatedAt = DateTime.Now;
            }

            // Create fine if overdue
            if (detail.ReturnDate > detail.Ticket.DueDate)
            {
                var daysOverdue = (detail.ReturnDate.Value - detail.Ticket.DueDate).Days;
                if (daysOverdue > 0)
                {
                    var fine = new Fine
                    {
                        BorrowDetailId = detail.DetailId,
                        Amount = daysOverdue * 5000,
                        Reason = "Trả muộn",
                        CreatedAt = DateTime.Now,
                        IsPaid = false
                    };
                    _context.Fines.Add(fine);
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var innerMsg = ex.InnerException != null ? " Inner: " + ex.InnerException.Message : "";
                throw new Exception($"Lỗi lưu dữ liệu: {ex.Message}{innerMsg}");
            }

            return MapToDetailDTO(detail);
        }

        public async Task<ApiResponse> ReportLostOrDamagedAsync(ReportIssueDTO dto)
        {
            try
            {
                var detail = await _context.BorrowingDetails
                    .Include(bd => bd.Book)
                    .Include(bd => bd.Ticket)
                    .FirstOrDefaultAsync(bd => bd.DetailId == dto.DetailId);

                if (detail == null)
                    return new ApiResponse { Success = false, Message = "Không tìm thấy thông tin mượn sách" };

                if (detail.Status == "Đã trả" || detail.Status == "Mất" || detail.Status == "Hỏng")
                    return new ApiResponse { Success = false, Message = "Sách này đã được xử lý" };

                detail.Status = dto.IssueType == "Lost" ? "Mất" : "Hỏng";
                detail.ReturnDate = DateTime.Now;

                // Create fine equal to book price
                var fine = new Fine
                {
                    BorrowDetailId = detail.DetailId,
                    Amount = detail.Book.Price ?? 0,
                    Reason = dto.IssueType == "Lost" ? "Mất sách" : "Làm hỏng sách",
                    CreatedAt = DateTime.Now,
                    IsPaid = false,
                    Notes = dto.Notes
                };

                _context.Fines.Add(fine);
                
                // Note: We don't increment AvailableQuantity because the book is lost/damaged
                
                await _context.SaveChangesAsync();
                return new ApiResponse { Success = true, Message = $"Đã ghi nhận sách bị {detail.Status.ToLower()} và tạo phiếu phạt." };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = "Lỗi: " + ex.Message };
            }
        }
 
        public async Task<List<BorrowingTicketResponseDTO>> GetMemberBorrowingsAsync(int memberId)
        {
            var tickets = await _context.BorrowingTickets
                .Include(bt => bt.Member)
                .Include(bt => bt.Librarian)
                .Include(bt => bt.BorrowingDetails)
                    .ThenInclude(bd => bd.Book)
                        .ThenInclude(b => b.Authors)
                .Where(bt => bt.MemberId == memberId)
                .OrderByDescending(bt => bt.BorrowDate)
                .ToListAsync();

            return tickets.Select(MapToTicketDTO).ToList();
        }


        public async Task<BorrowingTicketResponseDTO?> GetBorrowingByIdAsync(int ticketId)
        {
            var ticket = await _context.BorrowingTickets
                .Include(bt => bt.Member)
                .Include(bt => bt.Librarian)
                .Include(bt => bt.BorrowingDetails)
                    .ThenInclude(bd => bd.Book)
                        .ThenInclude(b => b.Authors)
                .FirstOrDefaultAsync(bt => bt.TicketId == ticketId);

            return MapToTicketDTO(ticket!);
        }

        private BorrowingTicketResponseDTO MapToTicketDTO(BorrowingTicket ticket)
        {
            return new BorrowingTicketResponseDTO
            {
                TicketId = ticket.TicketId,
                MemberName = ticket.Member?.FullName ?? "Unknown",
                MemberEmail = ticket.Member?.Email,
                LibrarianName = ticket.Librarian?.FullName ?? "Unknown",
                BorrowDate = ticket.BorrowDate,
                DueDate = ticket.DueDate,
                Notes = ticket.Notes,
                Details = ticket.BorrowingDetails?.Select(MapToDetailDTO).ToList() ?? new()
            };
        }

        private BorrowingDetailResponseDTO MapToDetailDTO(BorrowingDetail detail)
        {
            var daysOverdue = 0;
            //Console.WriteLine(DateTime.Now);
            //Console.WriteLine(detail.Ticket.DueDate);
            //Console.WriteLine((DateTime.Now - detail.Ticket.DueDate).Days);
            if (detail.ReturnDate > detail.Ticket.DueDate)
            {
                daysOverdue = (DateTime.Now - detail.Ticket.DueDate).Days;
            }

            var authorsString = detail.Book?.Authors != null && detail.Book.Authors.Any()
                ? string.Join(", ", detail.Book.Authors.Select(a => a.AuthorName))
                : null;

            return new BorrowingDetailResponseDTO
            {
                DetailId = detail.DetailId,
                BookId = detail.BookId,
                BookTitle = detail.Book?.Title ?? "Unknown",
                BookIsbn = detail.Book?.Isbn,
                ReturnDate = detail.ReturnDate,
                Status = detail.Status,
                Authors = authorsString,
                BookPrice = detail.Book?.Price,
                DaysOverdue = daysOverdue,
                Fines = detail.Fines.Select(f => new FineDTO
                {
                    FineId = f.FineId,
                    BorrowDetailId = f.BorrowDetailId,
                    Amount = f.Amount,
                    Reason = f.Reason,
                    IsPaid = f.IsPaid,
                    PaidDate = f.PaidDate,
                    CreatedAt = f.CreatedAt,
                    Notes = f.Notes
                }).ToList()
            };
        }
        public async Task<List<BorrowingTicketResponseDTO>> GetAllBorrowingsAsync()
        {
            var tickets = await _context.BorrowingTickets
                .Include(t => t.Member)
                .Include(t => t.Librarian)
                .Include(t => t.BorrowingDetails)
                    .ThenInclude(d => d.Book)
                        .ThenInclude(b => b.Authors)
                .Include(t => t.BorrowingDetails)
                    .ThenInclude(d => d.Fines)
                .OrderByDescending(t => t.BorrowDate)
                .ToListAsync();

            return tickets.Select(MapToResponseDTO).ToList();
        }

        public async Task<List<BorrowingTicketResponseDTO>> GetPendingBorrowingsAsync()
        {
            var tickets = await _context.BorrowingTickets
                .Include(t => t.Member)
                .Include(t => t.Librarian)
                .Include(t => t.BorrowingDetails)
                    .ThenInclude(d => d.Book)
                            .ThenInclude(ba => ba.Authors)
                .Include(t => t.BorrowingDetails)
                    .ThenInclude(d => d.Fines)
                .Where(t => t.BorrowingDetails.Any(d => d.Status == "Chờ duyệt"))
                .OrderByDescending(t => t.BorrowDate)
                .ToListAsync();

            return tickets.Select(MapToResponseDTO).ToList();
        }
        public async Task<ApiResponse> ApproveBorrowingAsync(ApproveBorrowingDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var detail = await _context.BorrowingDetails
                    .Include(d => d.Book)
                    .FirstOrDefaultAsync(d => d.DetailId == dto.DetailId);

                if (detail == null)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy chi tiết mượn sách"

                    };
                }

                if (detail.Status != "Chờ duyệt")
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Chi tiết này không ở trạng thái chờ duyệt"

                    };
                }

                if (detail.Book.AvailableQuantity < 1)
                {

                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Sách không còn sẵn"

                    };
                }

                detail.Status = "Đang mượn";

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new ApiResponse
                {
                    Success = true,
                    Message = "Duyệt mượn sách thành công"

                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiResponse
                {
                    Success = false,
                    Message = "Lỗi: "+ ex.Message  

                };
            }
        }


        public async Task<ApiResponse> RejectBorrowingAsync(RejectBorrowingDTO dto)
        {
            try
            {
                var detail = await _context.BorrowingDetails
                    .FirstOrDefaultAsync(d => d.DetailId == dto.DetailId);

                if (detail == null)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy chi tiết mượn sách"
                    };
                }

                if (detail.Status != "Chờ duyệt")
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Chi tiết này không ở trạng thái chờ duyệt"
                    };
                }

                // _context.BorrowingDetails.Remove(detail);
                detail.Status = "Từ chối";
                var book = _context.Books
                    .Where(b => b.BookId == detail.BookId)
                    .FirstOrDefault();
                if (book != null)
                {
                    book.AvailableQuantity++;
                }               
                 await _context.SaveChangesAsync();
                return new ApiResponse
                {
                    Success = true,
                    Message = "Từ chối mượn sách thành công"
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Lỗi: " + ex.Message
                };
            }
        }

        public async Task UpdateOverdueStatusAsync()
        {
            try
            {
                var now = DateTime.Now;
                var overdueDetails = await _context.BorrowingDetails
                    .Include(d => d.Ticket)
                    .Where(d => d.Status == "Đang mượn"
                            && d.Ticket.DueDate < now)
                    .ToListAsync();

                foreach (var detail in overdueDetails)
                {
                    detail.Status = "Quá hạn";
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.Message);
            }
        }
        private BorrowingTicketResponseDTO MapToResponseDTO(BorrowingTicket ticket)
        {
            return new BorrowingTicketResponseDTO
            {
                TicketId = ticket.TicketId,
                MemberName = ticket.Member.FullName,
                MemberEmail = ticket.Member.Email,
                LibrarianName = ticket.Librarian.FullName,
                BorrowDate = ticket.BorrowDate,
                DueDate = ticket.DueDate,
                Notes = ticket.Notes,
                Details = ticket.BorrowingDetails.Select(MapToDetailDTO).ToList()
            };
        }

        

    }
}
