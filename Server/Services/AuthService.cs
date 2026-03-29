
using Server.Models;
using Server.DTOs;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace Server.Services
{
    
    public class AuthService : IAuthService
    {
        private readonly LibraryManagementDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(LibraryManagementDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginDto)
        {
            
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);
          
            if (user == null)
                return null;
           
            if (!user.IsActive)
                return null;
            
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);

            if (!isPasswordValid)
                return null;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Role, user.Role.RoleName)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials);

            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponseDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                RoleId = user.RoleId,
                RoleName = user.Role.RoleName,
                ImageURL = user.ImageUrl,
                Token = jwtToken,
                Message = "Đăng nhập thành công!"
            };
        }
        public async Task<bool> RegisterAsync(RegisterRequestDTO registerDto)
        {
           
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == registerDto.Username);

            if (existingUser != null)
                return false; 
           
            var existingEmail = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == registerDto.Email);

            if (existingEmail != null)
                return false;

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

      
            var memberRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == "Member");

            if (memberRole == null)
                return false; 

        
            var newUser = new User
            {
                Username = registerDto.Username,
                PasswordHash = passwordHash,
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                RoleId = memberRole.RoleId,
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
