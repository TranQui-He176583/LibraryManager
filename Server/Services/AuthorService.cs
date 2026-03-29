using Microsoft.EntityFrameworkCore;
using Server.DTOs;
using Server.Models;
namespace Server.Services
{
    public class AuthorService : IAuthorService
    {
        public LibraryManagementDbContext _context;
        public AuthorService (LibraryManagementDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponseDTO<List<AuthorDTO>>> getAllAuthor()
        {
            try
            {
                var authors = await _context.Authors
                    .OrderBy(a => a.AuthorName)
                    .ToListAsync();
                List<AuthorDTO> result = new List<AuthorDTO>();
                foreach (Author author in authors )
                {
                    result.Add(new AuthorDTO {
                        AuthorId =  author.AuthorId,
                        AuthorName = author.AuthorName,
                    });
                }
                return new ApiResponseDTO<List<AuthorDTO>>
                {
                    Success = true,
                    Message = "Complete!",
                    Data = result
                };
            }
            catch (Exception ex) {
                return new ApiResponseDTO<List<AuthorDTO>>
                {
                    Success = false, // Changed from true to false for exception!
                    Message = "Failed: " + ex.Message
                };
            }
        }

        public async Task<ApiResponseDTO<AuthorDTO>> CreateAuthor(AuthorDTO request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.AuthorName))
                    return new ApiResponseDTO<AuthorDTO> { Success = false, Message = "Tên tác giả không được để trống" };

                var author = new Author { AuthorName = request.AuthorName };
                _context.Authors.Add(author);
                await _context.SaveChangesAsync();

                return new ApiResponseDTO<AuthorDTO>
                {
                    Success = true,
                    Message = "Thêm tác giả thành công",
                    Data = new AuthorDTO { AuthorId = author.AuthorId, AuthorName = author.AuthorName }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<AuthorDTO> { Success = false, Message = "Lỗi: " + ex.Message };
            }
        }

        public async Task<ApiResponseDTO<AuthorDTO>> UpdateAuthor(int id, AuthorDTO request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.AuthorName))
                    return new ApiResponseDTO<AuthorDTO> { Success = false, Message = "Tên tác giả không được để trống" };

                var author = await _context.Authors.FindAsync(id);
                if (author == null)
                    return new ApiResponseDTO<AuthorDTO> { Success = false, Message = "Không tìm thấy tác giả" };

                author.AuthorName = request.AuthorName;
                await _context.SaveChangesAsync();

                return new ApiResponseDTO<AuthorDTO>
                {
                    Success = true,
                    Message = "Cập nhật tác giả thành công",
                    Data = new AuthorDTO { AuthorId = author.AuthorId, AuthorName = author.AuthorName }
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<AuthorDTO> { Success = false, Message = "Lỗi: " + ex.Message };
            }
        }

        public async Task<ApiResponseDTO<bool>> DeleteAuthor(int id)
        {
            try
            {
                var author = await _context.Authors
                    .Include(a => a.Books)
                    .FirstOrDefaultAsync(a => a.AuthorId == id);

                if (author == null)
                    return new ApiResponseDTO<bool> { Success = false, Message = "Không tìm thấy tác giả" };

                if (author.Books != null && author.Books.Any())
                    return new ApiResponseDTO<bool> { Success = false, Message = "Không thể xóa tác giả này vì đã có sách thuộc tác giả này trong hệ thống." };

                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();

                return new ApiResponseDTO<bool>
                {
                    Success = true,
                    Message = "Xóa tác giả thành công",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDTO<bool> { Success = false, Message = "Lỗi: " + ex.Message };
            }
        }
    }
}
