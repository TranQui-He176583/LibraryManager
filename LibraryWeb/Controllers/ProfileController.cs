using LibraryWeb.Models;
using LibraryWeb.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryWeb.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IApiService _apiService;

        public ProfileController(IApiService apiService)
        {
            _apiService = apiService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập!";
                return RedirectToAction("Login", "Auth");
            }

            var profile = await _apiService.GetProfileAsync(userId.Value);
            if (profile == null)
            {
                TempData["ErrorMessage"] = "Không thể tải thông tin profile!";
                return RedirectToAction("Index", "Home");
            }

            if (profile.RoleName == "Member")
            {
                profile.Stats = await _apiService.GetBorrowingStatsAsync(userId.Value);
            }

            return View(profile);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập!";
                return RedirectToAction("Login", "Auth");
            }

            var profile = await _apiService.GetProfileAsync(userId.Value);
            if (profile == null)
            {
                TempData["ErrorMessage"] = "Không thể tải thông tin profile!";
                return RedirectToAction("Index");
            }

            var model = new UpdateProfileViewModel
            {
                FullName = profile.FullName,
                Email = profile.Email,
                PhoneNumber = profile.PhoneNumber,
                Address = profile.Address,
                DateOfBirth = profile.DateOfBirth,
                CurrentImageUrl = profile.ImageUrl
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateProfileViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Phiên đăng nhập đã hết hạn!";
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
                return View(model);

            if (model.ProfileImage != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(model.ProfileImage.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("ProfileImage", "Chỉ chấp nhận file ảnh: jpg, jpeg, png, gif, webp");
                    return View(model);
                }

                if (model.ProfileImage.Length > 5 * 1024 * 1024) 
                {
                    ModelState.AddModelError("ProfileImage", "Kích thước ảnh không được vượt quá 5MB");
                    return View(model);
                }
            }


            var success = await _apiService.UpdateProfileWithImageAsync(userId.Value, model);

            if (success.Success)
            {
                HttpContext.Session.SetString("FullName", model.FullName);
                HttpContext.Session.SetString("Email", model.Email);

                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = success.Message;
            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập!";
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Phiên đăng nhập đã hết hạn!";
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
                return View(model);

            var success = await _apiService.ChangePasswordAsync(userId.Value, model);

            if (success)
            {
                TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Mật khẩu hiện tại không đúng hoặc có lỗi xảy ra!");
            return View(model);
        }
    }
}
