using Microsoft.Extensions.DependencyInjection;
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
namespace WPF_Staff_Admin.ViewModels
{
    public class BookDetailViewModel : ViewModelBase
    {
        private readonly IBookService _bookService;
        private readonly IDialogService _dialogService;
        private readonly IAuthorService _authorService;
        private readonly ICategoryService _categoryService;
        private readonly IPublisherService _publisherService;
        private readonly Action? _onClose;

        private BookDTO? _book;
        private bool _isLoading;

        public BookDetailViewModel(
        IBookService bookService,
         IAuthorService authorService,
        ICategoryService categoryService,
        IPublisherService publisherService,
        IDialogService dialogService,
         int bookId,
        Action? onClose = null)
        {
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
            _authorService = authorService ?? throw new ArgumentNullException(nameof(authorService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _publisherService = publisherService ?? throw new ArgumentNullException(nameof(publisherService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _onClose = onClose;

            CloseCommand = new RelayCommand(_ => Close());
            EditCommand = new RelayCommand(_ => Edit());
            DeleteCommand = new RelayCommand(async _ => await DeleteAsync());

            LoadBookDetails(bookId);
        }

        #region Properties

        public BookDTO? Book
        {
            get => _book;
            set
            {
                if (SetProperty(ref _book, value))
                {
                    OnPropertyChanged(nameof(AuthorsDisplay));
                    OnPropertyChanged(nameof(CategoriesDisplay));
                    OnPropertyChanged(nameof(QuantityInfo));
                    OnPropertyChanged(nameof(PriceDisplay));
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string AuthorsDisplay => Book?.AuthorsDisplay ?? "N/A";
        public string CategoriesDisplay => Book?.CategoriesDisplay ?? "N/A";
        public string QuantityInfo => Book != null
            ? $"Còn lại: {Book.AvailableQuantity} / Tổng: {Book.TotalQuantity}"
            : "N/A";
        public string PriceDisplay => Book?.Price != null
            ? $"{Book.Price:N0} đ"
            : "N/A";

        #endregion

        #region Commands

        public ICommand CloseCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        #endregion

        #region Methods

        private async void LoadBookDetails(int bookId)
        {
            try
            {
                IsLoading = true;

                var response = await _bookService.GetBookByIdAsync(bookId);

                if (response.Success && response.Data != null)
                {
                    Book = response.Data;
                }
                else
                {
                    _dialogService.ShowError(response.Message ?? "Không thể tải thông tin sách");
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi tải thông tin sách: {ex.Message}\n\nChi tiết: {ex.StackTrace}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void Close()
        {
            try
            {
                _onClose?.Invoke();

                Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(w => w.DataContext == this)
                    ?.Close();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi đóng cửa sổ: {ex.Message}");
            }
        }

        private void Edit()
        {
            if (Book == null) return;
            var fileUploadService = App.ServiceProvider.GetRequiredService<IFileUploadService>();
            try
            {
                var formViewModel = new BookFormViewModel(
                    _bookService,
                    _authorService,
                    _categoryService,
                    _publisherService,
                    _dialogService,
                    fileUploadService,
                    bookId: Book.BookId,
                    onSaved: () =>
                    {
                        LoadBookDetails(Book.BookId);
                    }
                );

                var formView = new Views.Books.BookFormView(formViewModel)
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                formView.ShowDialog();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private async Task DeleteAsync()
        {
            if (Book == null) return;

            var confirm = _dialogService.ShowConfirmation(
                $"Bạn có chắc muốn xóa sách '{Book.Title}'?",
                "Xác nhận xóa"
            );

            if (!confirm) return;

            try
            {
                IsLoading = true;
                var response = await _bookService.DeleteBookAsync(Book.BookId);

                if (response.Success)
                {
                    _dialogService.ShowSuccess("Xóa sách thành công!");
                    Close();
                }
                else
                {
                    _dialogService.ShowError(response.Message ?? "Không thể xóa sách");
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi xóa sách: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion
    }
}
