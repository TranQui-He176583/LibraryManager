using Server.DTOs;

namespace Server.Services
{
    public interface IAuthorService
    {
        Task<ApiResponseDTO<List<AuthorDTO>>> getAllAuthor();
    }
}
