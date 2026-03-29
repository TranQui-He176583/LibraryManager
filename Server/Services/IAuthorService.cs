using Server.DTOs;

namespace Server.Services
{
    public interface IAuthorService
    {
        Task<ApiResponseDTO<List<AuthorDTO>>> getAllAuthor();
        Task<ApiResponseDTO<AuthorDTO>> CreateAuthor(AuthorDTO request);
        Task<ApiResponseDTO<AuthorDTO>> UpdateAuthor(int id, AuthorDTO request);
        Task<ApiResponseDTO<bool>> DeleteAuthor(int id);
    }
}
