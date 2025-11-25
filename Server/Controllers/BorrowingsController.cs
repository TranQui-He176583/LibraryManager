using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Services;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BorrowingsController : Controller
    {

        private readonly IBorrowingService _borrowingService;

        public BorrowingsController(IBorrowingService borrowingService)
        {
            _borrowingService = borrowingService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateBorrowing([FromBody] BorrowingCreateDTO borrowingDto)
        {
            try
            {
                var result = await _borrowingService.CreateBorrowingAsync(borrowingDto);

                return Ok(new
                {
                    Success = true,
                    Message = "Tạo phiếu mượn thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost("return")]
        public async Task<IActionResult> ReturnBookSimple([FromBody] ReturnBookDTO returnDto)
        {
            try
            {
                var result = await _borrowingService.ReturnBookByTicketAndBookAsync(
                    returnDto.TicketId,
                    returnDto.BookId
                    );

                return Ok(new
                {
                    Success = true,
                    Message = "Trả sách thành công",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("member/{memberId}")]
        public async Task<IActionResult> GetMemberBorrowings(int memberId)
        {
            try
            {
                var result = await _borrowingService.GetMemberBorrowingsAsync(memberId);

                return Ok(new
                {
                    Success = true,
                    Message = $"Tìm thấy {result.Count} phiếu mượn",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<BorrowingTicketResponseDTO>>> GetAllBorrowings()
        {
            try
            {
                var borrowings = await _borrowingService.GetAllBorrowingsAsync();
                return Ok(borrowings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách mượn sách" }+ ex.Message);
            }
        }

        [HttpGet("pending")]
        public async Task<ActionResult<List<BorrowingTicketResponseDTO>>> GetPendingBorrowings()
        {
            try
            {
                var borrowings = await _borrowingService.GetPendingBorrowingsAsync();
                return Ok(borrowings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách chờ duyệt"+ ex.Message });
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<BorrowingTicketResponseDTO>> GetBorrowingById(int id)
        {
            try
            {
                var borrowing = await _borrowingService.GetBorrowingByIdAsync(id);

                if (borrowing == null)
                {
                    return NotFound(new { message = "Không tìm thấy phiếu mượn" });
                }

                return Ok(borrowing);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi lấy thông tin mượn sách"+ ex.Message });
            }
        }

        [HttpPost("approve")]
        public async Task<ActionResult> ApproveBorrowing([FromBody] ApproveBorrowingDTO dto)
        {
            try
            {
                var apiResponse = await _borrowingService.ApproveBorrowingAsync(dto);

                if (apiResponse == null)
                {
                    return BadRequest(new { message ="Respone null" });
                }

                return Ok(new { apiResponse.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi duyệt mượn sách" } + ex.Message);
            }
        }

        [HttpPost("reject")]
        public async Task<ActionResult> RejectBorrowing([FromBody] RejectBorrowingDTO dto)
        {
            try
            {
                var apiResponse = await _borrowingService.RejectBorrowingAsync(dto);

                if (apiResponse == null)
                {
                    return BadRequest(new { message = "Service sai" });
                }

                return Ok(new { message = apiResponse.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi từ chối mượn sách" + ex.Message });
            }
        }

        [HttpPost("update-overdue")]
        public async Task<ActionResult> UpdateOverdueStatus()
        {
            try
            {
                await _borrowingService.UpdateOverdueStatusAsync();
                return Ok(new { message = "Cập nhật trạng thái quá hạn thành công" });
            }
            catch (Exception ex)
            {               
                return StatusCode(500, new { message = "Lỗi khi cập nhật trạng thái" + ex.Message });
            }
        }


    }
}