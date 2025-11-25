using LibraryWeb.Models;
using LibraryWeb.Services;
using Microsoft.AspNetCore.Mvc;
namespace LibraryWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly IApiService _apiService;

        public AuthController(IApiService apiService)
        {
            _apiService = apiService;
        }
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _apiService.LoginAsync(model);
            if (result == null)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng!");
                return View(model);
            }

            // Lưu session
            HttpContext.Session.SetInt32("UserId", result.UserId);
            HttpContext.Session.SetString("Username", result.Username);
            HttpContext.Session.SetString("FullName", result.FullName);
            HttpContext.Session.SetString("RoleName", result.RoleName);
            
            if (!string.IsNullOrEmpty(result.Email))
                HttpContext.Session.SetString("Email", result.Email);

            TempData["SuccessMessage"] = $"Chào mừng {result.FullName}!";
            return result.RoleName == "Member"
                ? RedirectToAction("Index", "Books")
                : RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Đã đăng xuất thành công!";
            return RedirectToAction("Login");
        }
    }
}