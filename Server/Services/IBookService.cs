using Microsoft.AspNetCore.Mvc;
using Server.DTOs;

namespace Server.Services
{
    public interface IBookService
    {
        Task<BookDetailDTO> ReturnBookDetail(int bookId);
        Task <List<BookDetailDTO>> GetAllBook();
        Task<ActionResult<ApiResponseDTO<SearchBooksResultDTO>>> SearchBooks([FromQuery] SearchBooksRequestDTO request);
        Task<ActionResult<ApiResponseDTO<List<string>>>> GetLanguages();
        Task<ActionResult<ApiResponseDTO<List<PublisherDTO>>>> GetPublishers();
        Task<ActionResult<ApiResponseDTO<PublisherDTO>>> GetPublisher(int id);
        Task<ActionResult<ApiResponseDTO<BookDetailDTO>>> CreateBook([FromBody] CreateBookRequest request);
        Task<ActionResult<ApiResponseDTO<BookDetailDTO>>> UpdateBook(int id, [FromBody] UpdateBookRequest request);
        Task<ActionResult<ApiResponseDTO<string>>> UploadBookImage(IFormFile file);
        ActionResult<ApiResponse> DeleteBookImage([FromQuery] int bookId);

        ActionResult<ApiResponse> DeleteBook([FromQuery] int bookId);
    }
}
