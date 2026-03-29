using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPF_Staff_Admin.Helpers;
using WPF_Staff_Admin.Models;
using WPF_Staff_Admin.Services;

namespace WPF_Staff_Admin.ViewModels.Users
{
    public class UserFormViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private readonly IDialogService _dialogService;
        private readonly UserDTO? _originalUser;

        private string _username = string.Empty;
        private string _password = string.Empty; // For new users
        private string _fullName = string.Empty;
        private string _email = string.Empty;
        private string? _phoneNumber;
        private string? _address;
        private DateOnly? _dateOfBirth;
        private string _role = "Member";
        private string? _newPassword; // For existing users
        private bool _isSaving;
        private string _title = "Thêm người dùng mới";

        public UserFormViewModel(
            IUserService userService,
            IDialogService dialogService,
            UserDTO? user = null)
        {
            _userService = userService;
            _dialogService = dialogService;
            _originalUser = user;

            SaveCommand = new RelayCommand(async _ => await SaveAsync());
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke(this, false));

            if (_originalUser != null)
            {
                Title = "Chỉnh sửa người dùng";
                Username = _originalUser.Username;
                FullName = _originalUser.FullName;
                Email = _originalUser.Email;
                PhoneNumber = _originalUser.PhoneNumber;
                Address = _originalUser.Address;
                DateOfBirth = _originalUser.DateOfBirth;
                Role = _originalUser.Role;
            }
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string FullName
        {
            get => _fullName;
            set => SetProperty(ref _fullName, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string? PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        public string? Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        public DateOnly? DateOfBirth
        {
            get => _dateOfBirth;
            set => SetProperty(ref _dateOfBirth, value);
        }

        public string Role
        {
            get => _role;
            set => SetProperty(ref _role, value);
        }

        public string? NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        public bool IsSaving
        {
            get => _isSaving;
            set => SetProperty(ref _isSaving, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public bool IsEditMode => _originalUser != null;
        public bool IsCreateMode => !IsEditMode;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler<bool>? RequestClose;

        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email))
            {
                _dialogService.ShowError("Họ tên và Email không được để trống!");
                return;
            }

            if (IsCreateMode && string.IsNullOrWhiteSpace(Password))
            {
                _dialogService.ShowError("Vui lòng nhập mật khẩu cho người dùng mới!");
                return;
            }

            try
            {
                IsSaving = true;
                ApiResponse response;

                if (IsEditMode)
                {
                    var updateDto = new UserUpdateDTO
                    {
                        FullName = FullName,
                        Email = Email,
                        PhoneNumber = PhoneNumber,
                        Address = Address,
                        DateOfBirth = DateOfBirth,
                        NewPassword = NewPassword
                    };
                    response = await _userService.UpdateUserAsync(_originalUser!.UserId, updateDto);
                }
                else
                {
                    var createRequest = new CreateUserRequest
                    {
                        Username = Username,
                        Password = Password,
                        FullName = FullName,
                        Email = Email,
                        PhoneNumber = PhoneNumber,
                        Address = Address,
                        DateOfBirth = DateOfBirth
                    };
                    var createResponse = await _userService.CreateUserAsync(createRequest);
                    response = new ApiResponse { Success = createResponse.Success, Message = createResponse.Message };
                }

                if (response.Success)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _dialogService.ShowSuccess(response.Message ?? "Lưu thành công!");
                        RequestClose?.Invoke(this, true);
                    });
                }
                else
                {
                    _dialogService.ShowError(response.Message ?? "Có lỗi xảy ra");
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi: {ex.Message}");
            }
            finally
            {
                IsSaving = false;
            }
        }
    }
}
