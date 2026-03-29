using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using WPF_Staff_Admin.Helpers;
using WPF_Staff_Admin.Models;
using WPF_Staff_Admin.Services;

namespace WPF_Staff_Admin.ViewModels.Categories
{
    public class CategoryFormViewModel : INotifyPropertyChanged
    {
        private readonly ICategoryService _categoryService;
        private readonly IDialogService _dialogService;
        private string _categoryName = string.Empty;
        private string _title = "Thêm Thể loại mới";
        private bool _isSaving;
        private CategoryDTO? _originalCategory;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<bool>? RequestClose;

        public CategoryFormViewModel(ICategoryService categoryService, IDialogService dialogService, CategoryDTO? category = null)
        {
            _categoryService = categoryService;
            _dialogService = dialogService;
            _originalCategory = category;

            if (category != null)
            {
                _categoryName = category.CategoryName;
                _title = "Sửa Thể loại";
            }

            SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => CanSave());
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke(this, false));
        }

        public string CategoryName
        {
            get => _categoryName;
            set
            {
                _categoryName = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public bool IsSaving
        {
            get => _isSaving;
            set
            {
                _isSaving = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private bool CanSave() => !string.IsNullOrWhiteSpace(CategoryName) && !IsSaving;

        private async Task SaveAsync()
        {
            try
            {
                IsSaving = true;
                bool success;
                string message;

                if (_originalCategory == null)
                {
                    var response = await _categoryService.CreateCategoryAsync(new CategoryDTO { CategoryName = CategoryName });
                    success = response.Success;
                    message = response.Message;
                }
                else
                {
                    var response = await _categoryService.UpdateCategoryAsync(_originalCategory.CategoryId, new CategoryDTO { CategoryName = CategoryName });
                    success = response.Success;
                    message = response.Message;
                }

                if (success)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        _dialogService.ShowSuccess(message);
                        RequestClose?.Invoke(this, true);
                    });
                }
                else
                {
                    _dialogService.ShowError(message);
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError(ex.Message);
            }
            finally
            {
                IsSaving = false;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
