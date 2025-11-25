using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Staff_Admin.Helpers
{
    public class SessionManager
    {
        private static SessionManager? _instance;
        private static readonly object _lock = new object();

        public static SessionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new SessionManager();
                    }
                }
                return _instance;
            }
        }
        public UserSession? CurrentUser { get; private set; }
        public bool IsLoggedIn => CurrentUser != null;
        public bool IsAdmin => CurrentUser?.RoleId == 1;
        public bool IsStaff => CurrentUser?.RoleId == 2;

        private SessionManager() { }

        public void Login(UserSession user)
        {
            CurrentUser = user;
        }

        public void Logout()
        {
            CurrentUser = null;
        }

        public void UpdateUser(UserSession user)
        {
            CurrentUser = user;
        }
    }

    public class UserSession
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? Token { get; set; }
    }


}
