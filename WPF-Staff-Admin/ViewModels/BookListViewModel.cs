using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class BookListViewModel : ViewModelBase
    {
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly ICategoryService _categoryService;
        private readonly IDialogService _dialogService;
        private readonly IPublisherService _publisherService;

        private ObservableCollection<BookDTO> _books = new();
        private BookDTO? _selectedBook;
        private bool _isLoading;
        private string _searchQuery = string.Empty;
        private int _currentPage = 1;
        private int _totalPages = 1;
        private int _totalItems = 0;
        private int _pageSize = 20;

        private List<AuthorDTO> _authors = new();
        private List<CategoryDTO> _categories = new();
        private List<string> _languages = new();
        private AuthorDTO? _selectedAuthor;
        private CategoryDTO? _selectedCategory;
        private string? _selectedLanguage;
        private string _sortBy = "title";
        private string _sortOrder = "asc";

        public BookListViewModel(
            IBookService bookService,
            IAuthorService authorService,
            ICategoryService categoryService,
            IPublisherService publisherService,
            IDialogService dialogService)
        {
            _bookService = bookService;
            _authorService = authorService;
            _categoryService = categoryService;
            _dialogService = dialogService;
            _publisherService = publisherService;

            LoadBooksCommand = new RelayCommand(async _ => await LoadBooksAsync());
            SearchCommand = new RelayCommand(async _ => await SearchBooksAsync());
            ClearFiltersCommand = new RelayCommand(_ => ClearFilters());
            ViewDetailsCommand = new RelayCommand<BookDTO>(ViewDetails);
            AddBookCommand = new RelayCommand(_ => AddBook());
            EditBookCommand = new RelayCommand<BookDTO>(EditBook, _ => SelectedBook != null);
            DeleteBookCommand = new RelayCommand<BookDTO>(async book => await DeleteBookAsync(book), _ => SelectedBook != null);
            RefreshCommand = new RelayCommand(async _ => await LoadBooksAsync());
            FirstPageCommand = new RelayCommand(_ => GoToFirstPage(), _ => CurrentPage > 1);
            PreviousPageCommand = new RelayCommand(_ => GoToPreviousPage(), _ => CurrentPage > 1);
            NextPageCommand = new RelayCommand(_ => GoToNextPage(), _ => CurrentPage < TotalPages);
            LastPageCommand = new RelayCommand(_ => GoToLastPage(), _ => CurrentPage < TotalPages);

            _ = InitializeAsync();
        }

        #region Properties

        public ObservableCollection<BookDTO> Books
        {
            get => _books;
            set => SetProperty(ref _books, value);
        }

        public BookDTO? SelectedBook
        {
            get => _selectedBook;
            set
            {
                if (SetProperty(ref _selectedBook, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set => SetProperty(ref _searchQuery, value);
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (SetProperty(ref _currentPage, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            set
            {
                if (SetProperty(ref _totalPages, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }
        public int TotalItems
        {
            get => _totalItems;
            set => SetProperty(ref _totalItems, value);
        }

        public int PageSize
        {
            get => _pageSize;
            set => SetProperty(ref _pageSize, value);
        }

        public List<AuthorDTO> Authors
        {
            get => _authors;
            set => SetProperty(ref _authors, value);
        }

        public List<CategoryDTO> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public List<string> Languages
        {
            get => _languages;
            set => SetProperty(ref _languages, value);
        }

        public AuthorDTO? SelectedAuthor
        {
            get => _selectedAuthor;
            set => SetProperty(ref _selectedAuthor, value);
        }

        public CategoryDTO? SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        public string? SelectedLanguage
        {
            get => _selectedLanguage;
            set => SetProperty(ref _selectedLanguage, value);
        }

        public string PageInfo => $"Trang {CurrentPage} / {TotalPages} (Tổng: {TotalItems} sách)";

        #endregion

        #region Commands

        public ICommand LoadBooksCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        public ICommand ViewDetailsCommand { get; }
        public ICommand AddBookCommand { get; }
        public ICommand EditBookCommand { get; }
        public ICommand DeleteBookCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand FirstPageCommand { get; }
        public ICommand PreviousPageCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand LastPageCommand { get; }

        #endregion

        #region Methods

        private async Task InitializeAsync()
        {
            await LoadFiltersAsync();
            await LoadBooksAsync();
        }

        private async Task LoadFiltersAsync()
        {

            var authorsResponse = await _authorService.GetAllAuthorsAsync();
            if (authorsResponse.Success && authorsResponse.Data != null)
            {
                Authors = authorsResponse.Data;
            }

            var categoriesResponse = await _categoryService.GetAllCategoriesAsync();
            if (categoriesResponse.Success && categoriesResponse.Data != null)
            {
                Categories = categoriesResponse.Data;
            }

            var languagesResponse = await _bookService.GetLanguagesAsync();
            if (languagesResponse.Success && languagesResponse.Data != null)
            {
                Languages = languagesResponse.Data;
            }
        }

        private async Task LoadBooksAsync()
        {
            await SearchBooksAsync();
        }

        private async Task SearchBooksAsync()
        {
            try
            {
                IsLoading = true;

                var request = new SearchBooksRequest
                {
                    SearchQuery = string.IsNullOrWhiteSpace(SearchQuery) ? null : SearchQuery,
                    AuthorId = SelectedAuthor?.AuthorId,
                    CategoryId = SelectedCategory?.CategoryId,
                    Language = SelectedLanguage,
                    SortBy = _sortBy,
                    SortOrder = _sortOrder,
                    Page = CurrentPage,
                    PageSize = PageSize
                };

                var response = await _bookService.SearchBooksAsync(request);

                if (response.Success && response.Data != null)
                {
                    Books = new ObservableCollection<BookDTO>(response.Data.Books);
                    CurrentPage = response.Data.CurrentPage;
                    TotalPages = response.Data.TotalPages;
                    TotalItems = response.Data.TotalItems;
                    OnPropertyChanged(nameof(PageInfo));
                }
                else
                {
                    _dialogService.ShowError(response.Message ?? "Không thể tải danh sách sách");
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
        private void ClearFilters()
        {
            SearchQuery = string.Empty;
            SelectedAuthor = null;
            SelectedCategory = null;
            SelectedLanguage = null;
            CurrentPage = 1;
            _ = SearchBooksAsync();
        }



        private void ViewDetails(BookDTO? book)
        {
            if (book == null) return;

            try
            {
                var detailViewModel = new BookDetailViewModel(
                    _bookService,
                    _authorService,
                    _categoryService,
                    _publisherService,
                    _dialogService,
                    book.BookId
                );

                var detailView = new Views.Books.BookDetailView(detailViewModel)
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen  
                };

                detailView.ShowDialog();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private void AddBook()
        {
            try
            {
                var fileUploadService = App.ServiceProvider.GetRequiredService<IFileUploadService>();

                var formViewModel = new BookFormViewModel(
                    _bookService,
                    _authorService,
                    _categoryService,
                    _publisherService,
                    _dialogService,
                    fileUploadService,     
                    bookId: null,
                    onSaved: async () =>
                    {
                        await LoadBooksAsync();
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

        private void EditBook(BookDTO? book)
        {
            if (book == null) return;

            try
            {
                var fileUploadService = App.ServiceProvider.GetRequiredService<IFileUploadService>();

                var formViewModel = new BookFormViewModel(
                    _bookService,
                    _authorService,
                    _categoryService,
                    _publisherService,
                    _dialogService,
                    fileUploadService,
                    
                    bookId: book.BookId,
                    onSaved: async () =>
                    {
                        await LoadBooksAsync();
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

        private async Task DeleteBookAsync(BookDTO? book)
        {
            if (book == null) return;

            var confirm = _dialogService.ShowConfirmation(
                $"Bạn có chắc muốn xóa sách '{book.Title}'?",
                "Xác nhận xóa"
            );

            if (!confirm) return;

            try
            {
                IsLoading = true;
                var response = await _bookService.DeleteBookAsync(book.BookId);

                if (response.Success)
                {
                    _dialogService.ShowSuccess("Xóa sách thành công!");
                    await LoadBooksAsync();
                }
                else
                {
                    _dialogService.ShowError(response.Message ?? "Không thể xóa sách");
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

        private void GoToFirstPage()
        {
            CurrentPage = 1;
            _ = SearchBooksAsync();
        }

        private void GoToPreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                _ = SearchBooksAsync();
            }
        }

        private void GoToNextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                _ = SearchBooksAsync();
            }
        }

        private void GoToLastPage()
        {
            CurrentPage = TotalPages;
            _ = SearchBooksAsync();
        }

        #endregion
    }
}
