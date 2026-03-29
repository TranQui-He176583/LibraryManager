using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPF_Staff_Admin.Helpers;
using WPF_Staff_Admin.Models;
using WPF_Staff_Admin.Services;
using WPF_Staff_Admin.Views.Users;

namespace WPF_Staff_Admin.ViewModels.Users
{
    public class UserListViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private readonly IDialogService _dialogService;
        private readonly IAuthService _authService;

        private ObservableCollection<UserDTO> _users = new();
        private ObservableCollection<UserDTO> _filteredUsers = new();
        private UserDTO? _selectedUser;
        private string _searchText = string.Empty;
        private string _selectedRoleFilter = "Tất cả";
        private bool _isLoading;

        public UserListViewModel(
            IUserService userService,
            IDialogService dialogService,
            IAuthService authService)
        {
            _userService = userService;
            _dialogService = dialogService;
            _authService = authService;

            RefreshCommand = new RelayCommand(async _ => await LoadUsersAsync());
            AddUserCommand = new RelayCommand(_ => AddUser());
            EditUserCommand = new RelayCommand(_ => EditUser(), _ => SelectedUser != null);
            ToggleStatusCommand = new RelayCommand(async _ => await ToggleStatusAsync(), _ => CanManageSelectedUser());
            SearchCommand = new RelayCommand(_ => ApplyFilters());

            _ = LoadUsersAsync();
        }

        public ObservableCollection<UserDTO> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        public ObservableCollection<UserDTO> FilteredUsers
        {
            get => _filteredUsers;
            set => SetProperty(ref _filteredUsers, value);
        }

        public UserDTO? SelectedUser
        {
            get => _selectedUser;
            set
            {
                SetProperty(ref _selectedUser, value);
                OnPropertyChanged(nameof(CanEdit));
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                ApplyFilters();
            }
        }

        public string SelectedRoleFilter
        {
            get => _selectedRoleFilter;
            set
            {
                SetProperty(ref _selectedRoleFilter, value);
                _ = LoadUsersAsync(); // Reload since server-side filtering is preferred
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public bool CanEdit => SelectedUser != null;

        public List<string> RoleFilters
        {
            get
            {
                var filters = new List<string> { "Tất cả", "Member" };
                if (_authService.CurrentUser?.RoleName == "Admin")
                {
                    filters.Add("Staff");
                    filters.Add("Admin");
                }
                return filters;
            }
        }

        public ICommand RefreshCommand { get; }
        public ICommand AddUserCommand { get; }
        public ICommand EditUserCommand { get; }
        public ICommand ToggleStatusCommand { get; }
        public ICommand SearchCommand { get; }

        private async Task LoadUsersAsync()
        {
            try
            {
                IsLoading = true;
                var roleParam = SelectedRoleFilter == "Tất cả" ? null : SelectedRoleFilter;
                var response = await _userService.GetAllUsersAsync(roleParam);

                if (response.Success && response.Data != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Users = new ObservableCollection<UserDTO>(response.Data);
                        ApplyFilters();
                    });
                }
                else
                {
                    _dialogService.ShowError(response.Message ?? "Không thể tải danh sách người dùng");
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ApplyFilters()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredUsers = new ObservableCollection<UserDTO>(Users);
            }
            else
            {
                var search = SearchText.ToLower();
                var filtered = Users.Where(u =>
                    u.FullName.ToLower().Contains(search) ||
                    u.Username.ToLower().Contains(search) ||
                    u.Email.ToLower().Contains(search) ||
                    (u.PhoneNumber != null && u.PhoneNumber.Contains(search))
                ).ToList();
                FilteredUsers = new ObservableCollection<UserDTO>(filtered);
            }
        }

        private void AddUser()
        {
            var viewModel = new UserFormViewModel(_userService, _dialogService, null);
            var window = new UserFormWindow(viewModel)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            if (window.ShowDialog() == true)
            {
                _ = LoadUsersAsync();
            }
        }

        private void EditUser()
        {
            if (SelectedUser == null) return;

            var viewModel = new UserFormViewModel(_userService, _dialogService, SelectedUser);
            var window = new UserFormWindow(viewModel)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            if (window.ShowDialog() == true)
            {
                _ = LoadUsersAsync();
            }
        }

        private async Task ToggleStatusAsync()
        {
            if (SelectedUser == null) return;

            string action = SelectedUser.IsActive ? "khóa" : "mở khóa";
            if (_dialogService.ShowConfirmation($"Bạn có chắc muốn {action} tài khoản '{SelectedUser.Username}'?", "Xác nhận"))
            {
                var response = await _userService.ToggleUserStatusAsync(SelectedUser.UserId);
                if (response.Success)
                {
                    _dialogService.ShowSuccess(response.Message ?? "Thao tác thành công");
                    await LoadUsersAsync();
                }
                else
                {
                    _dialogService.ShowError(response.Message ?? "Thao tác thất bại");
                }
            }
        }

        private bool CanManageSelectedUser()
        {
            if (SelectedUser == null) return false;
            if (SelectedUser.UserId == _authService.CurrentUser?.UserId) return false; // Không tự khóa mình

            // Staff chỉ quản lý Member
            if (_authService.CurrentUser?.RoleName == "Staff")
            {
                return SelectedUser.Role == "Member";
            }

            return true; // Admin quản lý tất cả
        }
    }
}
