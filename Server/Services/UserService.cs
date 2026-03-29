using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Server.DTOs;
using Server.Models;
using System;
using System.Diagnostics.Metrics;
namespace Server.Services

{
    public class UserService : IUserService
    {
        private LibraryManagementDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public UserService( LibraryManagementDbContext libraryManagementDbContext, IWebHostEnvironment webHostEnvironment) 
        
        {
            _context = libraryManagementDbContext;
            _environment = webHostEnvironment;
        }

        public async Task<ProfileDTO> GetProfileDTO(int userId)
        {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.Member)
                    .FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return null;
            var profileDTO  = new ProfileDTO
            {

                UserId = userId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                DateOfBirth  = user.DateOfBirth,
                ImageUrl = user.ImageUrl,
                RoleName = user.Role?.RoleName?? "Unknow",
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,

            };
            
            if (user.Role?.RoleName == "Member" && user.Member != null)
            {
                profileDTO.MemberInfo = new MemberInfoDTO
                {
                    MemberId = user.Member.MemberId,
                    IdCardNumber = user.Member.IdcardNumber,
                    MembershipDate = user.Member.MembershipDate,
                    MembershipExpiry = user.Member.MembershipExpiry,
                    MaxBorrowLimit = user.Member.MaxBorrowLimit,
                    SuspendedAt = user.Member.SuspendedAt
                };
            }
            return profileDTO;
        }

        public async Task<ActionResult<ApiResponse>> UpdateProfile(int userId, UpdateProfileDTO dto)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return new ApiResponse
                {
                    Success = false,
                    Message = "Không có user này!"

                };

                var existingEmail = await _context.Users
                    .AnyAsync(u => u.Email == dto.Email && u.UserId != userId);

                if (existingEmail)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Email đã được sử dụng bởi người khác!"
                    };
                }
                var existingPhone = await _context.Users
                    .AnyAsync(u => u.PhoneNumber == dto.PhoneNumber && u.UserId != userId);
                if (existingPhone)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Số điện thoại này đã được sử dụng bởi người khác!"
                    };
                }

                user.FullName = dto.FullName;
                user.Email = dto.Email;
                user.PhoneNumber = dto.PhoneNumber;
                user.Address = dto.Address;
                user.DateOfBirth = dto.DateOfBirth;

                if (!string.IsNullOrEmpty(dto.ImageUrl))
                {
                    user.ImageUrl = dto.ImageUrl;
                }

                user.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new ApiResponse
                {
                    Success = true,
                    Message = "Cập nhật profile thành công!",
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi cập nhật profile!" + ex.ToString()
                };
            }
        }

        public async Task<ActionResult<ApiResponse>> UploadProfileImage(int userId, IFormFile file)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy user!"
                    };
                }

                if (file == null || file.Length == 0)
                {
                     return new ApiResponse
                    {
                        Success = false,
                        Message = "File không hợp lệ!"
                    };
                }
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Chỉ chấp nhận file ảnh: jpg, jpeg, png, gif, webp"
                    };
                }
                if (file.Length > 5 * 1024 * 1024)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Kích thước file không được vượt quá 5MB"
                    };
                }

                if (!string.IsNullOrEmpty(user.ImageUrl))
                {
                    var isDelete =  DeleteOldImage(user.ImageUrl);
                    if (isDelete != null) 
                    {
                        return isDelete;                    
                    }

                }

                var fileName = $"user_{userId}_{Guid.NewGuid()}{extension}";
                var uploadsFolder = Path.Combine( _environment.ContentRootPath, "images", "profiles");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var imageUrl = $"/images/profiles/{fileName}";

                user.ImageUrl = imageUrl;
                user.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                return new ApiResponse
                {
                    Success = true,
                    Message = imageUrl
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = ex.ToString()
                };
            }
        }
        private ApiResponse DeleteOldImage(string imageUrl)
        {
            try
            {
                var relativePath = imageUrl.TrimStart('/');
                var filePath = Path.Combine(_environment.ContentRootPath, relativePath);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                return null;
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = ex.ToString()
                };
            }
        }

        [HttpDelete("{userId}/remove-image")]
        public async Task<ActionResult<ApiResponse>> RemoveProfileImage(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy user!"
                    };
                }

                if (!string.IsNullOrEmpty(user.ImageUrl))
                {
                    DeleteOldImage(user.ImageUrl);
                }
                user.ImageUrl = null;
                user.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                return new ApiResponse
                {
                    Success = true,
                    Message = "Xóa ảnh thành công!",                 
                };
            }
            catch (Exception ex)
            {
                return  new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi xóa ảnh!"+ ex.Message
                };
            }
        }

        [HttpPost("{userId}/change-password")]
        public async Task<ActionResult<ApiResponse>> ChangePassword(int userId, ChangePasswordDTO dto)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy user!"
                    };
                }
                if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                {
                    return new ApiResponse
                    {
                        Success = false,
                        Message = "Mật khẩu hiện tại không đúng!"
                    };
                }
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                user.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new ApiResponse
                {
                    Success = true,
                    Message = "Đổi mật khẩu thành công!",
     
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi đổi mật khẩu!" + ex.Message
                };
            }
        }


        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers(int currentUserId, string currentUserRole, [FromQuery] string? role = null)
        {
            var query = _context.Users.AsQueryable();
            // Phân quyền: Staff chỉ thấy Member. Admin thấy tất cả (hoặc lọc theo role)
            if (currentUserRole == "Staff")
            {
                query = query.Where(u => u.Role.RoleName == "Member");
            }
            else if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(u => u.Role.RoleName == role);
            }

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new UserDTO
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    FullName = u.FullName,
                    DateOfBirth = u.DateOfBirth,
                    Address = u.Address,
                    Role = u.Role.RoleName,
                    IsActive = u.IsActive,
                    MembershipExpiry = u.Member.MembershipExpiry,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .ToListAsync();

            return users;
        }

        public async Task<ActionResult<IEnumerable<UserDTO>>> SearchUsers([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return null;
            }

            var searchTerm = term.ToLower();

            var users = await _context.Users
                .Where(u => u.Role.RoleName == "Member" && (
                    u.Username.ToLower().Contains(searchTerm) ||
                    u.Email.ToLower().Contains(searchTerm) ||
                    u.FullName.ToLower().Contains(searchTerm) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(searchTerm))
                ))
                .Select(u => new UserDTO
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    FullName = u.FullName,
                    DateOfBirth = u.DateOfBirth,
                    Address = u.Address,
                    Role = u.Role.RoleName,
                    IsActive = u.IsActive,
                    MembershipExpiry = u.Member.MembershipExpiry,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .Take(50)
                .ToListAsync();

            return users;
        }

        public async Task<ActionResult<ApiResponseDTO<UserDTO>>> CreateUser([FromBody] CreateUserRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return new ApiResponseDTO<UserDTO>
                {
                    Success = false,
                    Message = "Đã có người khác dùng Username!"
                };
            }

            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return new ApiResponseDTO<UserDTO>
                {
                    Success = false,
                    Message = "Email đã được sử dụng"

                };
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var membershipExpiry = DateOnly.FromDateTime(
                  DateTime.Now.AddMonths(request.MembershipMonths)
               ); 

            var user = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                FullName = request.FullName,
                RoleId = 3,
                DateOfBirth = request.DateOfBirth,
                Address = request.Address,
                IsActive = true,
                //  MembershipExpiry = membershipExpiry,
                CreatedAt = DateTime.Now
            };
            user.Member = new Member();
            user.Member.MembershipExpiry = membershipExpiry;
            user.Role = _context.Roles.Where(r => r.RoleId == user.RoleId).FirstOrDefault();
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var userDto = new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName,
                DateOfBirth = user.DateOfBirth,
                Address = user.Address,
                Role = user.Role.RoleName,
                IsActive = user.IsActive,
                MembershipExpiry = user.Member.MembershipExpiry,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            return new ApiResponseDTO<UserDTO>
            {
                Success = true,
                Message = "Tạo Member thành công!",
                Data = userDto
            };

        }

        public async Task<ActionResult<ApiResponse>> ExtendMembership(int id, [FromBody] ExtendMembershipRequest request)
        {
            var user = await _context.Users
                .Where(u => u.UserId == id)
                .Include(u => u.Member)
                .Include(u => u.Role)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Không tìm thấy người dùng"
                };
            }

            if (user.Role.RoleName != "Member")
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Chỉ có thể gia hạn cho độc giả"
                };
            }

            var todayDateOnly = DateOnly.FromDateTime(DateTime.Today);

            var baseDate = user.Member.MembershipExpiry.HasValue && user.Member.MembershipExpiry.Value > todayDateOnly
                ? user.Member.MembershipExpiry.Value
                : todayDateOnly;

            user.Member.MembershipExpiry = baseDate.AddMonths(request.Months);
            user.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new ApiResponse
            {
                Success = true,
                Message = $"Gia hạn thành công {request.Months} tháng",
                            };
        }



        public async Task<ActionResult<ApiResponse>> UpdateUser(int userId, UserUpdateDTO dto, int currentUserId, string currentUserRole)
        {
            try
            {
                var user = await _context.Users.Include(u => u.Role).Include(u => u.Member).FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null) return new ApiResponse { Success = false, Message = "Không tìm thấy người dùng" };

                // Phân quyền: Staff chỉ được sửa Member
                if (currentUserRole == "Staff" && user.Role.RoleName != "Member")
                {
                    return new ApiResponse { Success = false, Message = "Bạn không có quyền chỉnh sửa tài khoản này" };
                }

                user.FullName = dto.FullName;
                user.Email = dto.Email;
                user.PhoneNumber = dto.PhoneNumber;
                user.Address = dto.Address;
                user.DateOfBirth = dto.DateOfBirth;
                user.UpdatedAt = DateTime.Now;

                if (!string.IsNullOrEmpty(dto.NewPassword))
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                }

                await _context.SaveChangesAsync();
                return new ApiResponse { Success = true, Message = "Cập nhật người dùng thành công" };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = "Lỗi: " + ex.Message };
            }
        }

        public async Task<ActionResult<ApiResponse>> ToggleUserStatus(int userId, int currentUserId, string currentUserRole)
        {
            try
            {
                var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null) return new ApiResponse { Success = false, Message = "Không tìm thấy người dùng" };

                if (userId == currentUserId) return new ApiResponse { Success = false, Message = "Bạn không thể tự khóa tài khoản của mình" };

                // Phân quyền: Staff chỉ được khóa Member
                if (currentUserRole == "Staff" && user.Role.RoleName != "Member")
                {
                    return new ApiResponse { Success = false, Message = "Bạn không có quyền thao tác trên tài khoản này" };
                }

                user.IsActive = !user.IsActive;
                user.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return new ApiResponse { Success = true, Message = (user.IsActive ? "Mở khóa" : "Khóa") + " tài khoản thành công" };
            }
            catch (Exception ex)
            {
                return new ApiResponse { Success = false, Message = "Lỗi: " + ex.Message };
            }
        }
    }
}
