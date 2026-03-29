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
using WPF_Staff_Admin.Views.Publishers;

namespace WPF_Staff_Admin.ViewModels.Publishers
{
    public class PublisherListViewModel : INotifyPropertyChanged
    {
        private readonly IPublisherService _publisherService;
        private readonly IDialogService _dialogService;
        private ObservableCollection<PublisherDTO> _publishers = new();
        private ObservableCollection<PublisherDTO> _filteredPublishers = new();
        private PublisherDTO? _selectedPublisher;
        private string _searchQuery = string.Empty;
        private bool _isLoading;
        private string _pageInfo = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public PublisherListViewModel(IPublisherService publisherService, IDialogService dialogService)
        {
            _publisherService = publisherService;
            _dialogService = dialogService;

            RefreshCommand = new RelayCommand(async _ => await LoadPublishersAsync());
            AddPublisherCommand = new RelayCommand(_ => AddPublisher());
            EditPublisherCommand = new RelayCommand(_ => EditPublisher(), _ => SelectedPublisher != null);
            DeletePublisherCommand = new RelayCommand(async _ => await DeletePublisherAsync(), _ => SelectedPublisher != null);
            SearchCommand = new RelayCommand(_ => ApplyFilter());

            Task.Run(async () => await LoadPublishersAsync());
        }

        public ObservableCollection<PublisherDTO> Publishers
        {
            get => _publishers;
            set
            {
                _publishers = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PublisherDTO> FilteredPublishers
        {
            get => _filteredPublishers;
            set
            {
                _filteredPublishers = value;
                OnPropertyChanged();
            }
        }

        public PublisherDTO? SelectedPublisher
        {
            get => _selectedPublisher;
            set
            {
                _selectedPublisher = value;
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
        public ICommand AddPublisherCommand { get; }
        public ICommand EditPublisherCommand { get; }
        public ICommand DeletePublisherCommand { get; }
        public ICommand SearchCommand { get; }

        private async Task LoadPublishersAsync()
        {
            try
            {
                IsLoading = true;
                var response = await _publisherService.GetAllPublishersAsync();
                if (response.Success && response.Data != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Publishers = new ObservableCollection<PublisherDTO>(response.Data);
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
                FilteredPublishers = new ObservableCollection<PublisherDTO>(Publishers);
            }
            else
            {
                var lowerSearch = SearchQuery.ToLower();
                var filtered = Publishers.Where(p => 
                    p.PublisherName.ToLower().Contains(lowerSearch) || 
                    (p.Website != null && p.Website.ToLower().Contains(lowerSearch))
                ).ToList();
                FilteredPublishers = new ObservableCollection<PublisherDTO>(filtered);
            }
            PageInfo = $"Tổng số: {FilteredPublishers.Count} nhà xuất bản";
        }

        private void AddPublisher()
        {
            var formViewModel = new PublisherFormViewModel(_publisherService, _dialogService);
            var formWindow = new PublisherFormWindow(formViewModel)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            if (formWindow.ShowDialog() == true)
            {
                Task.Run(async () => await LoadPublishersAsync());
            }
        }

        private void EditPublisher()
        {
            if (SelectedPublisher == null) return;

            var formViewModel = new PublisherFormViewModel(_publisherService, _dialogService, SelectedPublisher);
            var formWindow = new PublisherFormWindow(formViewModel)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            if (formWindow.ShowDialog() == true)
            {
                Task.Run(async () => await LoadPublishersAsync());
            }
        }

        private async Task DeletePublisherAsync()
        {
            if (SelectedPublisher == null) return;

            if (_dialogService.ShowConfirmation($"Bạn có chắc chắn muốn xóa nhà xuất bản '{SelectedPublisher.PublisherName}'?", "Xác nhận xóa"))
            {
                try
                {
                    IsLoading = true;
                    var response = await _publisherService.DeletePublisherAsync(SelectedPublisher.PublisherId);
                    if (response.Success)
                    {
                        _dialogService.ShowSuccess("Xóa nhà xuất bản thành công");
                        await LoadPublishersAsync();
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
