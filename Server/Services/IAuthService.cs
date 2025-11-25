using Server.DTOs;

namespace Server.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginDto);
        Task<bool> RegisterAsync(RegisterRequestDTO registerDto);
    }
}
