using Microsoft.AspNetCore.Mvc;
using Server.DTOs;

namespace Server.Services
{
    public interface IUserService
    {
        Task<ProfileDTO> GetProfileDTO(int userId);
        Task<ActionResult<ApiResponse>> UpdateProfile(int userId, UpdateProfileDTO dto);
        Task<ActionResult<ApiResponse>> UploadProfileImage(int userId, IFormFile file);
        Task<ActionResult<ApiResponse>> RemoveProfileImage(int userId);
        Task<ActionResult<ApiResponse>> ChangePassword(int userId, ChangePasswordDTO dto);
        Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers([FromQuery] string? role = null);
        Task<ActionResult<IEnumerable<UserDTO>>> SearchUsers([FromQuery] string term);
        Task<ActionResult<ApiResponseDTO<UserDTO>>> CreateUser([FromBody] CreateUserRequest request);
        Task<ActionResult<ApiResponse>> ExtendMembership(int id, [FromBody] ExtendMembershipRequest request);

    }
}
