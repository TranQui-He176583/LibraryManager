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
using System.IO;
namespace WPF_Staff_Admin.ViewModels
{
    public class BookFormViewModel : ViewModelBase
    {
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly ICategoryService _categoryService;
        private readonly IDialogService _dialogService;
        private readonly IPublisherService _publisherService;
        private readonly Action? _onSaved;
        private readonly IFileUploadService _fileUploadService;
        private string? _selectedImagePath;
        private bool _isUploadingImage;

        private bool _isEditMode;
        private int? _bookId;
        private bool _isLoading;
        private bool _isSaving;

        private string _title = string.Empty;
        private string _isbn = string.Empty;
        private int? _publisherId;
        private int? _publicationYear;
        private int? _pageCount;
        private string? _language;
        private string _description = string.Empty;
        private string _imageUrl = string.Empty;
        private int _totalQuantity;
        private int _availableQuantity;
        private decimal? _price;
        private string _location = string.Empty;

        private List<PublisherDTO> _publishers = new();
        private List<string> _languages = new();
        private ObservableCollection<SelectableAuthorDTO> _authors = new();
        private ObservableCollection<SelectableCategoryDTO> _categories = new();

        public BookFormViewModel(
            IBookService bookService,
            IAuthorService authorService,
            ICategoryService categoryService,
            IPublisherService publisherService,
            IDialogService dialogService,
            IFileUploadService fileUploadService,
            int? bookId = null,
            Action? onSaved = null)
        {
            _bookService = bookService;
            _authorService = authorService;
            _categoryService = categoryService;
            _publisherService = publisherService;
            _dialogService = dialogService;
            _fileUploadService = fileUploadService;
            _bookId = bookId;
            _onSaved = onSaved;
            _isEditMode = bookId.HasValue;

            SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => CanSave());
            CancelCommand = new RelayCommand(_ => Cancel());
            BrowseImageCommand = new RelayCommand(_ => BrowseImage());
            ClearImageCommand = new RelayCommand(_ => ClearImage());

            _ = InitializeAsync();
        }

        #region Properties

        public string FormTitle => _isEditMode ? "Chỉnh sửa sách" : "Thêm sách mới";

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public bool IsSaving
        {
            get => _isSaving;
            set
            {
                if (SetProperty(ref _isSaving, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }
        public string Title
        {
            get => _title;
            set
            {
                if (SetProperty(ref _title, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string Isbn
        {
            get => _isbn;
            set => SetProperty(ref _isbn, value);
        }

        public int? PublisherId
        {
            get => _publisherId;
            set => SetProperty(ref _publisherId, value);
        }

        public int? PublicationYear
        {
            get => _publicationYear;
            set => SetProperty(ref _publicationYear, value);
        }

        public int? PageCount
        {
            get => _pageCount;
            set => SetProperty(ref _pageCount, value);
        }

        public string? Language
        {
            get => _language;
            set => SetProperty(ref _language, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string ImageUrl
        {
            get => _imageUrl;
            set => SetProperty(ref _imageUrl, value);
        }

        public int TotalQuantity
        {
            get => _totalQuantity;
            set
            {
                if (SetProperty(ref _totalQuantity, value))
                {
                    if (AvailableQuantity > value)
                    {
                        AvailableQuantity = value;
                    }
                }
            }
        }

        public int AvailableQuantity
        {
            get => _availableQuantity;
            set => SetProperty(ref _availableQuantity, value);
        }

        public decimal? Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        public string Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        public List<PublisherDTO> Publishers
        {
            get => _publishers;
            set => SetProperty(ref _publishers, value);
        }

        public List<string> Languages
        {
            get => _languages;
            set => SetProperty(ref _languages, value);
        }

        public ObservableCollection<SelectableAuthorDTO> Authors
        {
            get => _authors;
            set => SetProperty(ref _authors, value);
        }

        public ObservableCollection<SelectableCategoryDTO> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }
        public string? SelectedImagePath
        {
            get => _selectedImagePath;
            set => SetProperty(ref _selectedImagePath, value);
        }

        public bool IsUploadingImage
        {
            get => _isUploadingImage;
            set => SetProperty(ref _isUploadingImage, value);
        }

        public bool HasSelectedImage => !string.IsNullOrWhiteSpace(SelectedImagePath);

        #endregion

        #region Commands

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ICommand BrowseImageCommand { get; }
        public ICommand ClearImageCommand { get; }
        #endregion

        #region Methods

        private async Task InitializeAsync()
        {
            try
            {
                IsLoading = true;

                await LoadMasterDataAsync();

                if (_isEditMode && _bookId.HasValue)
                {
                    await LoadBookDataAsync(_bookId.Value);
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khởi tạo form: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
        private void BrowseImage()
        {
            try
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Title = "Chọn ảnh sách",
                    Filter = "Image files (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif|All files (*.*)|*.*",
                    Multiselect = false
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    SelectedImagePath = openFileDialog.FileName;
                    OnPropertyChanged(nameof(HasSelectedImage));
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi chọn file: {ex.Message}");
            }
        }

        private void ClearImage()
        {
            SelectedImagePath = null;
            ImageUrl = string.Empty;
            OnPropertyChanged(nameof(HasSelectedImage));
        }

        private async Task LoadMasterDataAsync()
        {
            var publishersResponse = await _publisherService.GetAllPublishersAsync();
            if (publishersResponse.Success && publishersResponse.Data != null)
            {
                Publishers = publishersResponse.Data;
            }

            var languagesResponse = await _bookService.GetLanguagesAsync();
            if (languagesResponse.Success && languagesResponse.Data != null)
            {
                Languages = languagesResponse.Data;
            }

            var authorsResponse = await _authorService.GetAllAuthorsAsync();
            if (authorsResponse.Success && authorsResponse.Data != null)
            {
                Authors = new ObservableCollection<SelectableAuthorDTO>(
                    authorsResponse.Data.Select(a => new SelectableAuthorDTO
                    {
                        AuthorId = a.AuthorId,
                        AuthorName = a.AuthorName,
                        IsSelected = false
                    })
                );
            }

            var categoriesResponse = await _categoryService.GetAllCategoriesAsync();
            if (categoriesResponse.Success && categoriesResponse.Data != null)
            {
                Categories = new ObservableCollection<SelectableCategoryDTO>(
                    categoriesResponse.Data.Select(c => new SelectableCategoryDTO
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName,
                        IsSelected = false
                    })
                );
            }
        }

        private async Task LoadBookDataAsync(int bookId)
        {
            var response = await _bookService.GetBookByIdAsync(bookId);

            if (response.Success && response.Data != null)
            {
                var book = response.Data;

                Title = book.Title;
                Isbn = book.Isbn ?? string.Empty;
                PublisherId = book.PublisherId;
                PublicationYear = book.PublicationYear;
                PageCount = book.PageCount;
                Language = book.Language;
                Description = book.Description ?? string.Empty;
                ImageUrl = book.ImageUrl ?? string.Empty;
                TotalQuantity = book.TotalQuantity;
                AvailableQuantity = book.AvailableQuantity;
                Price = book.Price;
                Location = book.Location ?? string.Empty;

                if (book.Authors != null)
                {
                    foreach (var author in Authors)
                    {
                        author.IsSelected = book.Authors.Any(a => a.AuthorId == author.AuthorId);
                    }
                }

                if (book.Categories != null)
                {
                    foreach (var category in Categories)
                    {
                        category.IsSelected = book.Categories.Any(c => c.CategoryId == category.CategoryId);
                    }
                }
            }
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Title)
                && TotalQuantity > 0
                && AvailableQuantity >= 0
                && AvailableQuantity <= TotalQuantity
                && !IsSaving;
        }

        private async Task SaveAsync()
        {
            try
            {
                IsSaving = true;

                if (!string.IsNullOrWhiteSpace(SelectedImagePath) && File.Exists(SelectedImagePath))
                {
                    IsUploadingImage = true;

                    var uploadResponse = await _fileUploadService.UploadBookImageAsync(SelectedImagePath);

                    if (uploadResponse.Success && uploadResponse.Data != null)
                    {
                        ImageUrl = uploadResponse.Data;
                        await _fileUploadService.DeleteBookImageAsync(_bookId);
                        await _fileUploadService.DeleteBookImageAsync(_bookId);
                    }
                    else
                    {
                        _dialogService.ShowError($"Upload ảnh thất bại: {uploadResponse.Message}");
                        return; 
                    }

                    IsUploadingImage = false;
                }

                var selectedAuthorIds = Authors
                    .Where(a => a.IsSelected)
                    .Select(a => a.AuthorId)
                    .ToList();
                var selectedCategoryIds = Categories
                    .Where(c => c.IsSelected)
                    .Select(c => c.CategoryId)
                    .ToList();

                if (_isEditMode && _bookId.HasValue)
                {
                    var updateRequest = new UpdateBookRequest
                    {
                        BookId = _bookId.Value,
                        Title = Title,
                        Isbn = string.IsNullOrWhiteSpace(Isbn) ? null : Isbn,
                        PublisherId = PublisherId,
                        PublishedYear = PublicationYear,
                        PageCount = PageCount,
                        Language = Language,
                        Description = string.IsNullOrWhiteSpace(Description) ? null : Description,
                        ImageUrl = string.IsNullOrWhiteSpace(ImageUrl) ? null : ImageUrl,
                        TotalQuantity = TotalQuantity,
                        AvailableQuantity = AvailableQuantity,
                        Price = Price,
                        Location = string.IsNullOrWhiteSpace(Location) ? null : Location,
                        AuthorIds = selectedAuthorIds.Any() ? selectedAuthorIds : null,
                        CategoryIds = selectedCategoryIds.Any() ? selectedCategoryIds : null
                    };

                    var response = await _bookService.UpdateBookAsync(updateRequest);

                    if (response.Success)
                    {
                        _dialogService.ShowSuccess("Cập nhật sách thành công!");
                        _onSaved?.Invoke();
                        CloseWindow();
                    }
                    else
                    {
                        _dialogService.ShowError(response.Message ?? "Không thể cập nhật sách");
                    }
                }
                else
                {
                    var createRequest = new CreateBookRequest
                    {
                        Title = Title,
                        Isbn = string.IsNullOrWhiteSpace(Isbn) ? null : Isbn,
                        PublisherId = PublisherId,
                        PublishedYear = PublicationYear,
                        PageCount = PageCount,
                        Language = Language,
                        Description = string.IsNullOrWhiteSpace(Description) ? null : Description,
                        ImageUrl = string.IsNullOrWhiteSpace(ImageUrl) ? null : ImageUrl,
                        TotalQuantity = TotalQuantity,
                        AvailableQuantity = AvailableQuantity,
                        Price = Price,
                        Location = string.IsNullOrWhiteSpace(Location) ? null : Location,
                        AuthorIds = selectedAuthorIds.Any() ? selectedAuthorIds : null,
                        CategoryIds = selectedCategoryIds.Any() ? selectedCategoryIds : null
                    };

                    var response = await _bookService.CreateBookAsync(createRequest);

                    if (response.Success)
                    {
                        _dialogService.ShowSuccess("Thêm sách mới thành công!");
                        _onSaved?.Invoke();
                        CloseWindow();
                    }
                    else
                    {
                        _dialogService.ShowError(response.Message ?? "Không thể thêm sách");
                    }
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi: {ex.Message}");
            }
            finally
            {
                IsSaving = false;
                IsUploadingImage = false;
            }
        }

        private void Cancel()
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this)
                ?.Close();
        }

        #endregion
    }

    public class SelectableAuthorDTO : ViewModelBase
    {
        private bool _isSelected;

        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }

    public class SelectableCategoryDTO : ViewModelBase
    {
        private bool _isSelected;

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
