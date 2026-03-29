using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Services;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinesController : ControllerBase
    {
        private readonly IFineService _fineService;
        private readonly IBorrowingService _borrowingService;

        public FinesController(IFineService fineService, IBorrowingService borrowingService)
        {
            _fineService = fineService;
            _borrowingService = borrowingService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDTO<List<FineDTO>>>> GetFines()
        {
            var fines = await _fineService.GetAllFinesAsync();
            return Ok(new ApiResponseDTO<List<FineDTO>>
            {
                Success = true,
                Message = $"Tìm thấy {fines.Count} phiếu phạt",
                Data = fines
            });
        }

        [HttpGet("member/{memberId}")]
        public async Task<ActionResult<ApiResponseDTO<List<FineDTO>>>> GetMemberFines(int memberId)
        {
            var fines = await _fineService.GetFinesByMemberAsync(memberId);
            return Ok(new ApiResponseDTO<List<FineDTO>>
            {
                Success = true,
                Message = $"Tìm thấy {fines.Count} phiếu phạt",
                Data = fines
            });
        }

        [HttpPost("{id}/pay")]
        public async Task<IActionResult> PayFine(int id)
        {
            var result = await _fineService.MarkAsPaidAsync(id);
            if (!result) return NotFound(new ApiResponse { Success = false, Message = "Không tìm thấy phiếu phạt" });
            return Ok(new ApiResponse { Success = true, Message = "Thanh toán thành công" });
        }

        [HttpPost("report-issue")]
        public async Task<ActionResult<ApiResponse>> ReportIssue([FromBody] ReportIssueDTO dto)
        {
            var result = await _borrowingService.ReportLostOrDamagedAsync(dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}
