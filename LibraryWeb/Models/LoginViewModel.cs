using System.ComponentModel.DataAnnotations;

namespace LibraryWeb.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }

    public class LoginResponseViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string RoleName { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}
