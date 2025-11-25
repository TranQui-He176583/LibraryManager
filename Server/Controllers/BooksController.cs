using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.DTOs;
using Server.Models;
using Server.Services;
namespace Server.Controllers
{

    [Route("api/[controller]")]
    [ApiController]

    public class BooksController : Controller
    {
        private readonly LibraryManagementDbContext _context;
        private readonly IBookService _bookService;

        public BooksController(LibraryManagementDbContext context, IBookService bookService)
        {
            _context = context;
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            try
            {
                var books = await _bookService.GetAllBook();

                return Ok(new
                {
                    Success = true,
                    Message = $"Tìm thấy {books.Count} cuốn sách",
                    Data = books
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Lỗi khi lấy danh sách sách",
                    Error = ex.Message
                });
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDetailDTO>> GetBookDetail(int id)
        {
            try
            {
                var book = await _bookService.ReturnBookDetail(id);

                if (book == null)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = $"Không tìm thấy sách với ID {id}"
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Lấy thông tin sách thành công",
                    Data = book
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Lỗi khi lấy thông tin sách",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<ApiResponseDTO<SearchBooksResultDTO>>> SearchBooks([FromQuery] SearchBooksRequestDTO request)
        {
            return await _bookService.SearchBooks(request);
        }


        [HttpGet("languages")]
        public async Task<ActionResult<ApiResponseDTO<List<string>>>> GetLanguages()
        {
            return await _bookService.GetLanguages();
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDTO<BookDetailDTO>>> CreateBook([FromBody] CreateBookRequest request)
        {
            return await _bookService.CreateBook(request);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDTO<BookDetailDTO>>> UpdateBook(int id, [FromBody] UpdateBookRequest request)
        {
            return await _bookService.UpdateBook(id, request);
        }

        [HttpPost("book-image")]
        public async Task<ActionResult<ApiResponseDTO<string>>> UploadBookImage(IFormFile file)
        {
            return await _bookService.UploadBookImage(file);
        }

        [HttpDelete("book-image")]
        public ActionResult<ApiResponse> DeleteBookImage([FromQuery] int bookId)
        {
            return _bookService.DeleteBookImage(bookId);
        }

        [HttpDelete("Delete")]
        public ActionResult<ApiResponse> DeleteBook([FromQuery] int bookId)
        {
            return _bookService.DeleteBook(bookId);
        }
    }
}
