using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Input;
using WPF_Staff_Admin.Helpers;
using WPF_Staff_Admin.Services;
using WPF_Staff_Admin.ViewModels.Borrowing;
using WPF_Staff_Admin.Views;
using WPF_Staff_Admin.Views.Borrowing;

namespace WPF_Staff_Admin.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IAuthService _authService;

        private string _currentViewTitle = "Dashboard";
        private object? _currentView;

        public MainViewModel(INavigationService navigationService, IDialogService dialogService, IAuthService authService)
        {
            _navigationService = navigationService;
            _dialogService = dialogService;
            _authService = authService;

            NavigateToDashboardCommand = new RelayCommand(_ => NavigateToDashboard());
            NavigateToBooksCommand = new RelayCommand(_ => NavigateToBooks());
            NavigateToUsersCommand = new RelayCommand(_ => NavigateToUsers(), _ => CanAccessUsers());
            NavigateToBorrowingsCommand = new RelayCommand(_ => NavigateToBorrowings());
            NavigateToAuthorsCommand = new RelayCommand(_ => NavigateToAuthors());
            NavigateToCategoriesCommand = new RelayCommand(_ => NavigateToCategories());
            NavigateToPublishersCommand = new RelayCommand(_ => NavigateToPublishers());
            NavigateToFinesCommand = new RelayCommand(_ => NavigateToFines());
            NavigateToReportsCommand = new RelayCommand(_ => NavigateToReports());
            LogoutCommand = new RelayCommand(_ => Logout());
            
        }

        #region Properties

        public string CurrentViewTitle
        {
            get => _currentViewTitle;
            set => SetProperty(ref _currentViewTitle, value);
        }

        public object? CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public string UserFullName => SessionManager.Instance.CurrentUser?.FullName ?? "User";
        public string UserRole => SessionManager.Instance.CurrentUser?.RoleName ?? "";
        public bool IsAdmin => SessionManager.Instance.IsAdmin;
        public bool IsStaff => SessionManager.Instance.IsStaff;

        #endregion

        #region Commands

        public ICommand NavigateToDashboardCommand { get; }
        public ICommand NavigateToBooksCommand { get; }
        public ICommand NavigateToUsersCommand { get; }
        public ICommand NavigateToBorrowingsCommand { get; }
        public ICommand NavigateToAuthorsCommand { get; }
        public ICommand NavigateToCategoriesCommand { get; }
        public ICommand NavigateToPublishersCommand { get; }
        public ICommand NavigateToFinesCommand { get; }
        public ICommand NavigateToReportsCommand { get; }
        public ICommand LogoutCommand { get; }

        #endregion

        #region Navigation Methods

        public void NavigateToDashboard()
        {
            CurrentViewTitle = "Dashboard";

            var bookService = App.ServiceProvider.GetRequiredService<IBookService>();
            var borrowingService = App.ServiceProvider.GetRequiredService<IBorrowingService>();
            var dashboardViewModel = new DashboardViewModel(bookService, borrowingService);

            CurrentView = new DashboardView(dashboardViewModel);
        }

        private void NavigateToBooks()
        {
            CurrentViewTitle = "Quản lý Sách";

            var bookService = App.ServiceProvider.GetRequiredService<IBookService>();
            var authorService = App.ServiceProvider.GetRequiredService<IAuthorService>();
            var categoryService = App.ServiceProvider.GetRequiredService<ICategoryService>();
            var publisherService = App.ServiceProvider.GetRequiredService<IPublisherService>();
            var dialogService = App.ServiceProvider.GetRequiredService<IDialogService>();
            var authService = App.ServiceProvider.GetRequiredService<IAuthService>();

            var bookListViewModel = new BookListViewModel(
                bookService,
                authorService,
                categoryService,
                publisherService,
                dialogService
            );

            var bookListView = new Views.Books.BookListView(bookListViewModel);

            CurrentView = bookListView;
        }

        private void NavigateToUsers()
        {
            CurrentViewTitle = "Quản lý Người dùng";
            CurrentView = CreatePlaceholderView("User Management - Đang phát triển");
        }

        private void NavigateToBorrowings()
        {
            CurrentViewTitle = "Quản lý Mượn/Trả Sách";

            var borrowingService = App.ServiceProvider.GetRequiredService<IBorrowingService>();
            var dialogService = App.ServiceProvider.GetRequiredService<IDialogService>();
            var authService = App.ServiceProvider.GetRequiredService<IAuthService>();

            var borrowingListViewModel = new BorrowingListViewModel(
                borrowingService,
                dialogService,
                authService
            );

            var borrowingListView = new BorrowingListView(borrowingListViewModel);
            CurrentView = borrowingListView;
        }

        private void NavigateToAuthors()
        {
            CurrentViewTitle = "Quản lý Tác giả";

            var authorService = App.ServiceProvider.GetRequiredService<IAuthorService>();
            var dialogService = App.ServiceProvider.GetRequiredService<IDialogService>();

            var authorListViewModel = new WPF_Staff_Admin.ViewModels.Authors.AuthorListViewModel(
                authorService,
                dialogService
            );

            var authorListView = new WPF_Staff_Admin.Views.Authors.AuthorListView(authorListViewModel);
            CurrentView = authorListView;
        }

        private void NavigateToCategories()
        {
            CurrentViewTitle = "Quản lý Thể loại";

            var categoryService = App.ServiceProvider.GetRequiredService<ICategoryService>();
            var dialogService = App.ServiceProvider.GetRequiredService<IDialogService>();

            var categoryListViewModel = new WPF_Staff_Admin.ViewModels.Categories.CategoryListViewModel(
                categoryService,
                dialogService
            );

            var categoryListView = new WPF_Staff_Admin.Views.Categories.CategoryListView(categoryListViewModel);
            CurrentView = categoryListView;
        }

        private void NavigateToPublishers()
        {
            CurrentViewTitle = "Quản lý Nhà xuất bản";

            var publisherService = App.ServiceProvider.GetRequiredService<IPublisherService>();
            var dialogService = App.ServiceProvider.GetRequiredService<IDialogService>();

            var publisherListViewModel = new WPF_Staff_Admin.ViewModels.Publishers.PublisherListViewModel(
                publisherService,
                dialogService
            );

            var publisherListView = new WPF_Staff_Admin.Views.Publishers.PublisherListView(publisherListViewModel);
            CurrentView = publisherListView;
        }

        private void NavigateToFines()
        {
            CurrentViewTitle = "Quản lý Phạt";
            CurrentView = CreatePlaceholderView("Fine Management - Đang phát triển");
        }

        private void NavigateToReports()
        {
            CurrentViewTitle = "Báo cáo & Thống kê";
            CurrentView = CreatePlaceholderView("Reports - Đang phát triển");
        }

        private object CreatePlaceholderView(string message)
        {
            return new System.Windows.Controls.TextBlock
            {
                Text = message,
                FontSize = 24,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Foreground = System.Windows.Media.Brushes.Gray
            };
        }

        #endregion

        #region Permission Methods

        private bool CanAccessUsers()
        {
            return SessionManager.Instance.IsAdmin;
        }

        #endregion

        #region Other Methods

        private void Logout()
        {
            var confirm = _dialogService.ShowConfirmation(
                "Bạn có chắc muốn đăng xuất?",
                "Xác nhận đăng xuất"
            );

            if (confirm)
            {
                SessionManager.Instance.Logout();

                var loginWindow = App.ServiceProvider.GetRequiredService<LoginWindow>();
                loginWindow.Show();

                Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(w => w.GetType() == typeof(MainWindow))
                    ?.Close();
            }
        }

        #endregion
    }
}
