using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.DTOs;
using Server.Models;
using Server.Services;
namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> getUserProfile(int userId)
        {
            try
            {
                var result = await _userService.GetProfileDTO(userId);
                return Ok(new
                {
                    Success = true,
                    Message = "Get Profile complete",
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


        [HttpPut("{userId}")]
        public async Task<ActionResult<ApiResponse>> UpdateProfile(int userId, UpdateProfileDTO dto)
        {
            return await _userService.UpdateProfile(userId, dto);
        }

        [HttpPost("{userId}/upload-image")]
        public async Task<ActionResult<ApiResponse>> UploadProfileImage(int userId, IFormFile file)
        {
            return await _userService.UploadProfileImage(userId, file);
        }

        [HttpDelete("{userId}/remove-image")]
        public async Task<ActionResult<ApiResponse>> RemoveProfileImage(int userId)
        {
            return await _userService.RemoveProfileImage(userId);
        }

        [HttpPost("{userId}/change-password")]
        public async Task<ActionResult<ApiResponse>> ChangePassword(int userId, ChangePasswordDTO dto)
        {
            return await _userService.ChangePassword(userId, dto);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers([FromQuery] string? role = null)
        {
            return await _userService.GetAllUsers(role);
        }
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> SearchUsers([FromQuery] string term)
        {
            return await _userService.SearchUsers(term);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDTO<UserDTO>>> CreateUser([FromBody] CreateUserRequest request)
        {
            return await _userService.CreateUser(request);
        }
        [HttpPut("{id}/extend-membership")]
        public async Task<ActionResult<ApiResponse>> ExtendMembership(int id, [FromBody] ExtendMembershipRequest request)
        {
            return await _userService.ExtendMembership(id, request);
        }
    }
}
