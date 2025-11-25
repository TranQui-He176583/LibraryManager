using System.ComponentModel.DataAnnotations;

namespace LibraryWeb.Models
{
        public class ProfileViewModel
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
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public MemberInfoViewModel? MemberInfo { get; set; }
            public BorrowingStatsViewModel? Stats { get; set; }
        }

        public class MemberInfoViewModel
        {
            public int MemberId { get; set; }
            public string? IdCardNumber { get; set; }
            public DateTime MembershipDate { get; set; }
            public DateTime? MembershipExpiry { get; set; }
            public int MaxBorrowLimit { get; set; }
            public bool IsSuspended { get; set; }
            public DateTime? SuspendedAt { get; set; }
        }

        public class BorrowingStatsViewModel
        {
            public int TotalBorrowed { get; set; }
            public int CurrentBorrowing { get; set; }
            public int TotalReturned { get; set; }
            public int OverdueBooks { get; set; }
            public decimal TotalFines { get; set; }
            public decimal UnpaidFines { get; set; }
        }

        public class ChangePasswordViewModel
        {
            [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu hiện tại")]
            public string CurrentPassword { get; set; } = null!;

            [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu mới")]
            public string NewPassword { get; set; } = null!;

            [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu mới")]
            [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
            [DataType(DataType.Password)]
            [Display(Name = "Xác nhận mật khẩu mới")]
            public string ConfirmPassword { get; set; } = null!;
        }

        public class UpdateProfileViewModel
        {
            [Required(ErrorMessage = "Vui lòng nhập họ tên")]
            [StringLength(150, ErrorMessage = "Họ tên không được vượt quá 150 ký tự")]
            [Display(Name = "Họ và tên")]
            public string FullName { get; set; } = null!;

            [Required(ErrorMessage = "Vui lòng nhập email")]
            [EmailAddress(ErrorMessage = "Email không hợp lệ")]
            [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
            public string Email { get; set; } = null!;

            [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
            [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
            [Display(Name = "Số điện thoại")]
            public string? PhoneNumber { get; set; }

            [StringLength(300, ErrorMessage = "Địa chỉ không được vượt quá 300 ký tự")]
            [Display(Name = "Địa chỉ")]
            public string? Address { get; set; }

            [DataType(DataType.Date)]
            [Display(Name = "Ngày sinh")]
            public DateOnly? DateOfBirth { get; set; }

            [Display(Name = "Ảnh đại diện")]
            public IFormFile? ProfileImage { get; set; }

            public string? CurrentImageUrl { get; set; }

            public bool RemoveImage { get; set; }
        }
    }
