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
            if (!ModelState.IsValid) 
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "Vui lòng nhập đầy đủ thông tin" });
                return View(model);
            }

            var result = await _apiService.LoginAsync(model);
            if (result == null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "Tên đăng nhập hoặc mật khẩu không đúng!" });
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng!");
                return View(model);
            }

            // Lưu session
            HttpContext.Session.SetInt32("UserId", result.UserId);
            HttpContext.Session.SetString("Username", result.Username);
            HttpContext.Session.SetString("FullName", result.FullName);
            HttpContext.Session.SetString("RoleName", result.RoleName);
            
            if (!string.IsNullOrEmpty(result.Token))
                HttpContext.Session.SetString("Token", result.Token);
                
            if (!string.IsNullOrEmpty(result.Email))
                HttpContext.Session.SetString("Email", result.Email);

            TempData["SuccessMessage"] = $"Chào mừng {result.FullName}!";
            string redirectUrl = result.RoleName == "Member" ? Url.Action("Index", "Books") : Url.Action("Index", "Home");
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true, redirectUrl, token = result.Token });

            return Redirect(redirectUrl);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Đã đăng xuất thành công!";
            return RedirectToAction("Login");
        }
    }
}