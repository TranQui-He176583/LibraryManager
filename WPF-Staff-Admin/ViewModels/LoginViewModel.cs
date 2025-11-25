using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPF_Staff_Admin.Helpers;
using WPF_Staff_Admin.Models;
using WPF_Staff_Admin.Services;
using WPF_Staff_Admin.Views;
namespace WPF_Staff_Admin.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly IDialogService _dialogService;

        private string _username = string.Empty;
        private string _password = string.Empty;
        private bool _isLoading;
        private string _errorMessage = string.Empty;

        public LoginViewModel(IAuthService authService, IDialogService dialogService)
        {
            _authService = authService;
            _dialogService = dialogService;

            LoginCommand = new RelayCommand(async _ => await LoginAsync(), _ => CanLogin());
        }

        #region Properties

        public string Username
        {
            get => _username;
            set
            {
                if (SetProperty(ref _username, value))
                {
                    ErrorMessage = string.Empty;
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                {
                    ErrorMessage = string.Empty;
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (SetProperty(ref _isLoading, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        #endregion

        #region Commands

        public ICommand LoginCommand { get; }

        #endregion

        #region Methods

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Username)
                && !string.IsNullOrWhiteSpace(Password)
                && !IsLoading;
        }

        private async Task LoginAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var request = new LoginRequest
                {
                    Username = Username,
                    Password = Password
                };

                var response = await _authService.LoginAsync(request);

                if (response.Success && response.Data != null)
                {
                    var loginData = response.Data;

                    if (loginData.roleName != "Admin" && loginData.roleName != "Librarian")
                    {
                        ErrorMessage = "Bạn không có quyền truy cập hệ thống này!";
                        _dialogService.ShowError("Chỉ Admin và Staff mới có thể đăng nhập vào hệ thống này!");
                        return;
                    }

                    var userSession = new UserSession
                    {
                        UserId = loginData.UserId,
                        Username = loginData.UserName,
                        FullName = loginData.FullName,
                        Email = loginData.Email,
                        RoleId = loginData.RoleId,
                        RoleName = loginData.roleName,
                        ImageUrl = loginData.ImageURL,
                        Token = loginData.Token
                    };

                    SessionManager.Instance.Login(userSession);

                    var mainWindow = App.ServiceProvider.GetService(typeof(MainWindow)) as MainWindow;
                    if (mainWindow != null)
                    {
                        mainWindow.Show();

                        Application.Current.Windows
                            .OfType<Window>()
                            .FirstOrDefault(w => w.GetType() == typeof(LoginWindow))
                            ?.Close();
                    }
                }
                else
                {
                    ErrorMessage = "Đăng nhập thất bại!";
                    _dialogService.ShowError("Tài Khoản hoặc mật khẩu sai!");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Lỗi kết nối đến server!";
                _dialogService.ShowError($"Lỗi Login: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion
    }
}
