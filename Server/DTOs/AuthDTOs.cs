namespace Server.DTOs
{
    public class AuthDTOs
    {
    }
    public class LoginRequestDTO()
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
    public class LoginResponseDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public string? ImageURL { get; set; }
        public string? Token { get; set; }
        public string Message { get; set; } = null!;
    }
    public class RegisterRequestDTO
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
    }
}
