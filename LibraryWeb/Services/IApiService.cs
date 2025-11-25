using LibraryWeb.Models;
using static LibraryWeb.Models.ProfileViewModel;

namespace LibraryWeb.Services
{
    public interface IApiService
    {
   
        Task<LoginResponseViewModel?> LoginAsync(LoginViewModel loginModel);
        Task<ProfileViewModel?> GetProfileAsync(int userId);
        Task<bool> UpdateProfileAsync(int userId, UpdateProfileViewModel model);
        Task<uploadImageRespone> UpdateProfileWithImageAsync(int userId, UpdateProfileViewModel model);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordViewModel model);
        Task<BorrowingStatsViewModel?> GetBorrowingStatsAsync(int userId);

        Task<uploadImageRespone?> UploadProfileImageAsync(int userId, IFormFile imageFile);
        Task<bool> RemoveProfileImageAsync(int userId);

    }
}
