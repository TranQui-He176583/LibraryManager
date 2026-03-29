using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using WPF_Staff_Admin.Helpers;
using WPF_Staff_Admin.Models;
using WPF_Staff_Admin.Services;

namespace WPF_Staff_Admin.ViewModels.Publishers
{
    public class PublisherFormViewModel : INotifyPropertyChanged
    {
        private readonly IPublisherService _publisherService;
        private readonly IDialogService _dialogService;
        private string _publisherName = string.Empty;
        private string _web = string.Empty;
        private string _title = "Thêm Nhà xuất bản mới";
        private bool _isSaving;
        private PublisherDTO? _originalPublisher;

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<bool>? RequestClose;

        public PublisherFormViewModel(IPublisherService publisherService, IDialogService dialogService, PublisherDTO? publisher = null)
        {
            _publisherService = publisherService;
            _dialogService = dialogService;
            _originalPublisher = publisher;

            if (publisher != null)
            {
                _publisherName = publisher.PublisherName;
                _web = publisher.Website ?? string.Empty;
                _title = "Sửa Nhà xuất bản";
            }

            SaveCommand = new RelayCommand(async _ => await SaveAsync(), _ => CanSave());
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke(this, false));
        }

        public string PublisherName
        {
            get => _publisherName;
            set
            {
                _publisherName = value;
                OnPropertyChanged();
            }
        }

        public string Web
        {
            get => _web;
            set
            {
                _web = value;
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

        private bool CanSave() => !string.IsNullOrWhiteSpace(PublisherName) && !IsSaving;

        private async Task SaveAsync()
        {
            try
            {
                IsSaving = true;
                bool success;
                string message;

                var dto = new PublisherDTO
                {
                    PublisherName = PublisherName,
                    Website = Web
                };

                if (_originalPublisher == null)
                {
                    var response = await _publisherService.CreatePublisherAsync(dto);
                    success = response.Success;
                    message = response.Message;
                }
                else
                {
                    var response = await _publisherService.UpdatePublisherAsync(_originalPublisher.PublisherId, dto);
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
