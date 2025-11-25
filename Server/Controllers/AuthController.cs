using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Services;

namespace Server.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginDto)
        {
            try
            {
                var result = await _authService.LoginAsync(loginDto);

                if (result == null)
                {
                    return Unauthorized(new
                    {
                        Success = false,
                        Message = "Username hoặc password không đúng"
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Message = result.Message,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Lỗi khi đăng nhập",
                    Error = ex.Message
                });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerDto)
        {
            try
            {
                var result = await _authService.RegisterAsync(registerDto);

                if (!result)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Đăng ký thất bại. Username hoặc Email đã tồn tại"
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Đăng ký thành công!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Lỗi khi đăng ký",
                    Error = ex.Message
                });
            }
        }

    }
}
