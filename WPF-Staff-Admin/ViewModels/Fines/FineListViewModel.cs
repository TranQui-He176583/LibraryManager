using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPF_Staff_Admin.Helpers;
using WPF_Staff_Admin.Models;
using WPF_Staff_Admin.Services;

namespace WPF_Staff_Admin.ViewModels.Fines
{
    public class FineListViewModel : ViewModelBase
    {
        private readonly IFineService _fineService;
        private readonly IDialogService _dialogService;

        private ObservableCollection<FineDTO> _allFines = new();
        private ObservableCollection<FineDTO> _filteredFines = new();
        private FineDTO? _selectedFine;
        private string _searchText = string.Empty;
        private string _selectedStatusFilter = "Tất cả";
        private bool _isLoading;

        public FineListViewModel(IFineService fineService, IDialogService dialogService)
        {
            _fineService = fineService;
            _dialogService = dialogService;

            RefreshCommand = new RelayCommand(async _ => await LoadFinesAsync());
            PayFineCommand = new RelayCommand(async _ => await PayFineAsync(), _ => CanPayFine());
            
            StatusFilters = new List<string> { "Tất cả", "Chưa thanh toán", "Đã thanh toán" };

            // Initial load
            _ = LoadFinesAsync();
        }

        #region Properties

        public ObservableCollection<FineDTO> Fines
        {
            get => _filteredFines;
            set => SetProperty(ref _filteredFines, value);
        }

        public FineDTO? SelectedFine
        {
            get => _selectedFine;
            set => SetProperty(ref _selectedFine, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    ApplyFilter();
                }
            }
        }

        public List<string> StatusFilters { get; }

        public string SelectedStatusFilter
        {
            get => _selectedStatusFilter;
            set
            {
                if (SetProperty(ref _selectedStatusFilter, value))
                {
                    ApplyFilter();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        #endregion

        #region Commands

        public ICommand RefreshCommand { get; }
        public ICommand PayFineCommand { get; }

        #endregion

        #region Methods

        public async Task LoadFinesAsync()
        {
            IsLoading = true;
            try
            {
                var fines = await _fineService.GetAllFinesAsync();
                _allFines = new ObservableCollection<FineDTO>(fines);
                ApplyFilter();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi tải danh sách phạt: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ApplyFilter()
        {
            var filtered = _allFines.AsEnumerable();

            // Status filter
            if (SelectedStatusFilter == "Chưa thanh toán")
                filtered = filtered.Where(f => !f.IsPaid);
            else if (SelectedStatusFilter == "Đã thanh toán")
                filtered = filtered.Where(f => f.IsPaid);

            // Search filter
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var search = SearchText.ToLower();
                filtered = filtered.Where(f => 
                    (f.MemberName?.ToLower().Contains(search) ?? false) || 
                    (f.BookTitle?.ToLower().Contains(search) ?? false) ||
                    (f.Reason?.ToLower().Contains(search) ?? false));
            }

            Fines = new ObservableCollection<FineDTO>(filtered);
        }

        private bool CanPayFine() => SelectedFine != null && !SelectedFine.IsPaid;

        private async Task PayFineAsync()
        {
            if (SelectedFine == null) return;

            var confirm = _dialogService.ShowConfirmation(
                $"Xác nhận đã thu {SelectedFine.AmountDisplay} từ {SelectedFine.MemberName}?",
                "Xác nhận thanh toán"
            );

            if (confirm)
            {
                IsLoading = true;
                try
                {
                    var result = await _fineService.PayFineAsync(SelectedFine.FineId);
                    if (result)
                    {
                        _dialogService.ShowMessage("Thanh toán thành công!");
                        await LoadFinesAsync();
                    }
                    else
                    {
                        _dialogService.ShowError("Thanh toán thất bại.");
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
        }

        #endregion
    }
}
