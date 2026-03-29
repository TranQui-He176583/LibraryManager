using System;
using System.Threading.Tasks;
using System.Windows.Input;
using WPF_Staff_Admin.Helpers;
using WPF_Staff_Admin.Models;
using WPF_Staff_Admin.Services;

namespace WPF_Staff_Admin.ViewModels.Authors
{
    public class AuthorFormViewModel : ViewModelBase
    {
        private readonly IAuthorService _authorService;
        private readonly IDialogService _dialogService;
        private readonly int? _authorId;
        private readonly Func<Task> _onSaved;

        private string _authorName = string.Empty;
        private bool _isSaving;
        private string _title = "Thêm Tác giả mới";

        public AuthorFormViewModel(
            IAuthorService authorService,
            IDialogService dialogService,
            int? authorId,
            Func<Task> onSaved)
        {
            _authorService = authorService;
            _dialogService = dialogService;
            _authorId = authorId;
            _onSaved = onSaved;

            SaveCommand = new RelayCommand(async _ => await SaveAsync());
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke(this, false));

            if (_authorId.HasValue)
            {
                Title = "Chỉnh sửa Tác giả";
            }
        }

        public string AuthorName
        {
            get => _authorName;
            set => SetProperty(ref _authorName, value);
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

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler<bool>? RequestClose;

        public void LoadAuthorDetails(AuthorDTO author)
        {
            if (author != null)
            {
                AuthorName = author.AuthorName;
            }
        }

        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(AuthorName))
            {
                _dialogService.ShowError("Tên tác giả không được để trống!");
                return;
            }

            try
            {
                IsSaving = true;
                
                var dto = new AuthorDTO { AuthorName = AuthorName };
                ApiResponse<AuthorDTO> response;

                if (_authorId.HasValue)
                {
                    dto.AuthorId = _authorId.Value;
                    response = await _authorService.UpdateAuthorAsync(_authorId.Value, dto);
                }
                else
                {
                    response = await _authorService.CreateAuthorAsync(dto);
                }

                if (response.Success)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        _dialogService.ShowSuccess(response.Message ?? "Lưu dữ liệu thành công!");
                        _onSaved.Invoke();
                        RequestClose?.Invoke(this, true);
                    });
                }
                else
                {
                    _dialogService.ShowError(response.Message ?? "Không thể lưu dữ liệu.");
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lối: {ex.Message}");
            }
            finally
            {
                IsSaving = false;
            }
        }

    }
}
