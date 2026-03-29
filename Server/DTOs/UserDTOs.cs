using System.ComponentModel.DataAnnotations;

namespace Server.DTOs
{
    public class ProfileDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? ImageUrl { get; set; }
        public string RoleName { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public MemberInfoDTO? MemberInfo { get; set; }
    }
    public class MemberInfoDTO
    {
        public int MemberId { get; set; }
        public string? IdCardNumber { get; set; }
        public DateOnly MembershipDate { get; set; }
        public DateOnly? MembershipExpiry { get; set; }
        public int MaxBorrowLimit { get; set; }
        public DateTime? SuspendedAt { get; set; }
    }

    public class BorrowingStatsDTO
    {
        public int TotalBorrowed { get; set; }
        public int CurrentBorrowing { get; set; }
        public int TotalReturned { get; set; }
        public int OverdueBooks { get; set; }
        public decimal TotalFines { get; set; }
        public decimal UnpaidFines { get; set; }
    }
    public class UpdateProfileDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(150)]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = null!;

        [Phone]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(300)]
        public string? Address { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? ImageUrl { get; set; }
    }
    public class UserDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateOnly? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string Role { get; set; } = "Reader";
        public bool IsActive { get; set; }
        public DateOnly? MembershipExpiry { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateUserRequest
    {
        [Required(ErrorMessage = "Username là bắt buộc")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username phải từ 3-50 ký tự")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password là bắt buộc")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password phải từ 6-100 ký tự")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        public string FullName { get; set; } = string.Empty;

        public DateOnly? DateOfBirth { get; set; }
        public string? Address { get; set; }

        public int MembershipMonths { get; set; } = 12;
    }
    public class ExtendMembershipRequest
    {
        [Required]
        [Range(1, 24, ErrorMessage = "Số tháng gia hạn phải từ 1-24")]
        public int Months { get; set; } = 12;
    }

    public class ChangePasswordDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; } = null!;
    }

    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password phải từ 6-100 ký tự")]
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
    public class ApiResponseDTO<T> {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }

    public class UserUpdateDTO
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? NewPassword { get; set; }
    }
}
