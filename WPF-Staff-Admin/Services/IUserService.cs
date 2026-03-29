using System.Collections.Generic;
using System.Threading.Tasks;
using WPF_Staff_Admin.Models;

namespace WPF_Staff_Admin.Services
{
    public interface IUserService
    {
        Task<ApiResponse<IEnumerable<UserDTO>>> GetAllUsersAsync(string? role = null);
        Task<ApiResponse<IEnumerable<UserDTO>>> SearchUsersAsync(string term);
        Task<ApiResponse<UserDTO>> CreateUserAsync(CreateUserRequest request);
        Task<ApiResponse> UpdateUserAsync(int userId, UserUpdateDTO dto);
        Task<ApiResponse> ToggleUserStatusAsync(int userId);
        Task<ApiResponse> ExtendMembershipAsync(int userId, int months);
    }
}
