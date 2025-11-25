using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Staff_Admin.Models;

namespace WPF_Staff_Admin.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest loginRequest);
        Task LogoutAsync();
        bool IsAuthenticated { get; }
        UserDTO? CurrentUser { get; }
        string? Token { get; }
    }

}
