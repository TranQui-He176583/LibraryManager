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
                var authors = await _context.Books
                    .SelectMany(b => b.Authors)
                    .Distinct()
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
                    Success = true,
                    Message = "Failed" + ex.Message
                };

            }
        }
    }
}
