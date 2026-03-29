using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPF_Staff_Admin.Helpers;
using WPF_Staff_Admin.Models;
using WPF_Staff_Admin.Services;
using WPF_Staff_Admin.Views.Authors;
using Microsoft.Extensions.DependencyInjection;

namespace WPF_Staff_Admin.ViewModels.Authors
{
    public class AuthorListViewModel : ViewModelBase
    {
        private readonly IAuthorService _authorService;
        private readonly IDialogService _dialogService;

        private ObservableCollection<AuthorDTO> _authors = new();
        private ObservableCollection<AuthorDTO> _filteredAuthors = new();
        
        private AuthorDTO? _selectedAuthor;
        private bool _isLoading;
        private string _searchQuery = string.Empty;

        public AuthorListViewModel(
            IAuthorService authorService,
            IDialogService dialogService)
        {
            _authorService = authorService;
            _dialogService = dialogService;

            LoadAuthorsCommand = new RelayCommand(async _ => await LoadAuthorsAsync());
            SearchCommand = new RelayCommand(_ => SearchAuthors());
            AddAuthorCommand = new RelayCommand(_ => AddAuthor());
            EditAuthorCommand = new RelayCommand<AuthorDTO>(EditAuthor, _ => SelectedAuthor != null);
            DeleteAuthorCommand = new RelayCommand<AuthorDTO>(async author => await DeleteAuthorAsync(author), _ => SelectedAuthor != null);
            RefreshCommand = new RelayCommand(async _ => await LoadAuthorsAsync());

            _ = InitializeAsync();
        }

        #region Properties

        public ObservableCollection<AuthorDTO> FilteredAuthors
        {
            get => _filteredAuthors;
            set => SetProperty(ref _filteredAuthors, value);
        }

        public AuthorDTO? SelectedAuthor
        {
            get => _selectedAuthor;
            set
            {
                if (SetProperty(ref _selectedAuthor, value))
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
            set
            {
                if (SetProperty(ref _searchQuery, value))
                {
                    SearchAuthors();
                }
            }
        }

        public int TotalItems => FilteredAuthors.Count;
        public string PageInfo => $"Tổng {TotalItems} tác giả";

        #endregion

        #region Commands

        public ICommand LoadAuthorsCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand AddAuthorCommand { get; }
        public ICommand EditAuthorCommand { get; }
        public ICommand DeleteAuthorCommand { get; }
        public ICommand RefreshCommand { get; }

        #endregion

        #region Methods

        private async Task InitializeAsync()
        {
            await LoadAuthorsAsync();
        }

        private async Task LoadAuthorsAsync()
        {
            try
            {
                IsLoading = true;
                var response = await _authorService.GetAllAuthorsAsync();
                
                if (response.Success && response.Data != null)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        _authors = new ObservableCollection<AuthorDTO>(response.Data);
                        SearchAuthors();
                    });
                }
                else
                {
                    _dialogService.ShowError(response.Message ?? "Không thể tải danh sách tác giả");
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

        private void SearchAuthors()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                FilteredAuthors = new ObservableCollection<AuthorDTO>(_authors);
            }
            else
            {
                var lowerQuery = SearchQuery.ToLower();
                var filtered = _authors.Where(a => 
                    (a.AuthorName?.ToLower().Contains(lowerQuery) ?? false));
                
                FilteredAuthors = new ObservableCollection<AuthorDTO>(filtered);
            }
            
            OnPropertyChanged(nameof(TotalItems));
            OnPropertyChanged(nameof(PageInfo));
        }

        private void AddAuthor()
        {
            try
            {
                var formViewModel = new AuthorFormViewModel(
                    _authorService,
                    _dialogService,
                    authorId: null,
                    onSaved: async () => await LoadAuthorsAsync()
                );

                var formView = new AuthorFormWindow(formViewModel)
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

        private void EditAuthor(AuthorDTO? author)
        {
            if (author == null) return;

            try
            {
                var formViewModel = new AuthorFormViewModel(
                    _authorService,
                    _dialogService,
                    authorId: author.AuthorId,
                    onSaved: async () => await LoadAuthorsAsync()
                );
                
                // Set existing data
                formViewModel.LoadAuthorDetails(author);

                var formView = new AuthorFormWindow(formViewModel)
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

        private async Task DeleteAuthorAsync(AuthorDTO? author)
        {
            if (author == null) return;

            var confirm = _dialogService.ShowConfirmation(
                $"Bạn có chắc muốn xóa tác giả '{author.AuthorName}'?",
                "Xác nhận xóa"
            );

            if (!confirm) return;

            try
            {
                IsLoading = true;
                var response = await _authorService.DeleteAuthorAsync(author.AuthorId);

                if (response.Success)
                {
                    _dialogService.ShowSuccess("Xóa tác giả thành công!");
                    await LoadAuthorsAsync();
                }
                else
                {
                    _dialogService.ShowError(response.Message ?? "Không thể xóa tác giả");
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

        #endregion
    }
}
