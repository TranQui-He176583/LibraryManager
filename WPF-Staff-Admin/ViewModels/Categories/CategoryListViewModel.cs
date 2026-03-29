using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPF_Staff_Admin.Helpers;
using WPF_Staff_Admin.Models;
using WPF_Staff_Admin.Services;
using WPF_Staff_Admin.Views.Categories;

namespace WPF_Staff_Admin.ViewModels.Categories
{
    public class CategoryListViewModel : INotifyPropertyChanged
    {
        private readonly ICategoryService _categoryService;
        private readonly IDialogService _dialogService;
        private ObservableCollection<CategoryDTO> _categories = new();
        private ObservableCollection<CategoryDTO> _filteredCategories = new();
        private CategoryDTO? _selectedCategory;
        private string _searchQuery = string.Empty;
        private bool _isLoading;
        private string _pageInfo = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public CategoryListViewModel(ICategoryService categoryService, IDialogService dialogService)
        {
            _categoryService = categoryService;
            _dialogService = dialogService;

            RefreshCommand = new RelayCommand(async _ => await LoadCategoriesAsync());
            AddCategoryCommand = new RelayCommand(_ => AddCategory());
            EditCategoryCommand = new RelayCommand(_ => EditCategory(), _ => SelectedCategory != null);
            DeleteCategoryCommand = new RelayCommand(async _ => await DeleteCategoryAsync(), _ => SelectedCategory != null);
            SearchCommand = new RelayCommand(_ => ApplyFilter());

            Task.Run(async () => await LoadCategoriesAsync());
        }

        public ObservableCollection<CategoryDTO> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CategoryDTO> FilteredCategories
        {
            get => _filteredCategories;
            set
            {
                _filteredCategories = value;
                OnPropertyChanged();
            }
        }

        public CategoryDTO? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
                if (string.IsNullOrWhiteSpace(value))
                {
                    ApplyFilter();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public string PageInfo
        {
            get => _pageInfo;
            set
            {
                _pageInfo = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshCommand { get; }
        public ICommand AddCategoryCommand { get; }
        public ICommand EditCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }
        public ICommand SearchCommand { get; }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                IsLoading = true;
                var response = await _categoryService.GetAllCategoriesAsync();
                if (response.Success && response.Data != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Categories = new ObservableCollection<CategoryDTO>(response.Data);
                        ApplyFilter();
                    });
                }
                else
                {
                    _dialogService.ShowError(response.Message);
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                FilteredCategories = new ObservableCollection<CategoryDTO>(Categories);
            }
            else
            {
                var lowerSearch = SearchQuery.ToLower();
                var filtered = Categories.Where(c => c.CategoryName.ToLower().Contains(lowerSearch)).ToList();
                FilteredCategories = new ObservableCollection<CategoryDTO>(filtered);
            }
            PageInfo = $"Tổng số: {FilteredCategories.Count} thể loại";
        }

        private void AddCategory()
        {
            var formViewModel = new CategoryFormViewModel(_categoryService, _dialogService);
            var formWindow = new CategoryFormWindow(formViewModel)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            if (formWindow.ShowDialog() == true)
            {
                Task.Run(async () => await LoadCategoriesAsync());
            }
        }

        private void EditCategory()
        {
            if (SelectedCategory == null) return;

            var formViewModel = new CategoryFormViewModel(_categoryService, _dialogService, SelectedCategory);
            var formWindow = new CategoryFormWindow(formViewModel)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            if (formWindow.ShowDialog() == true)
            {
                Task.Run(async () => await LoadCategoriesAsync());
            }
        }

        private async Task DeleteCategoryAsync()
        {
            if (SelectedCategory == null) return;

            if (_dialogService.ShowConfirmation($"Bạn có chắc chắn muốn xóa thể loại '{SelectedCategory.CategoryName}'?", "Xác nhận xóa"))
            {
                try
                {
                    IsLoading = true;
                    var response = await _categoryService.DeleteCategoryAsync(SelectedCategory.CategoryId);
                    if (response.Success)
                    {
                        _dialogService.ShowSuccess("Xóa thể loại thành công");
                        await LoadCategoriesAsync();
                    }
                    else
                    {
                        _dialogService.ShowError(response.Message);
                    }
                }
                catch (Exception ex)
                {
                    _dialogService.ShowError(ex.Message);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
