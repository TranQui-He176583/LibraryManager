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
namespace WPF_Staff_Admin.ViewModels.Borrowing
{
    public class BorrowingListViewModel : ViewModelBase
    {
        private readonly IBorrowingService _borrowingService;
        private readonly IDialogService _dialogService;
        private readonly IAuthService _authService;

        private ObservableCollection<BorrowingTicketDTO> _borrowings = new();
        private ObservableCollection<BorrowingTicketDTO> _filteredBorrowings = new();
        private BorrowingTicketDTO? _selectedBorrowing;
        private string _searchText = string.Empty;
        private string _selectedFilter = "Tất cả";
        private bool _isLoading;

        public BorrowingListViewModel(
            IBorrowingService borrowingService,
            IDialogService dialogService,
            IAuthService authService)
        {
            _borrowingService = borrowingService;
            _dialogService = dialogService;
            _authService = authService;

            LoadBorrowingsCommand = new RelayCommand(async _ => await LoadBorrowingsAsync());
            RefreshCommand = new RelayCommand(async _ => await LoadBorrowingsAsync());
            CreateBorrowingCommand = new RelayCommand(_ => CreateBorrowing());
            ViewDetailCommand = new RelayCommand(ViewDetail, _ => SelectedBorrowing != null);
            ApproveCommand = new RelayCommand(async _ => await ApproveAsync(), _ => CanApprove());
            RejectCommand = new RelayCommand(async _ => await RejectAsync(), _ => CanReject());
            ReturnBookCommand = new RelayCommand(async _ => await ReturnBookAsync(), _ => CanReturn());
            ReportLostCommand = new RelayCommand(async _ => await ReportIssueAsync("Lost"), _ => CanReturn());
            ReportDamagedCommand = new RelayCommand(async _ => await ReportIssueAsync("Damaged"), _ => CanReturn());

            Console.WriteLine(">>> BorrowingListViewModel Constructor");
            Console.WriteLine($">>> BorrowingService is null? {_borrowingService == null}");
            Console.WriteLine($">>> DialogService is null? {_dialogService == null}");
            Console.WriteLine($">>> AuthService is null? {_authService == null}");

            _ = LoadBorrowingsAsync();
        }

        public ObservableCollection<BorrowingTicketDTO> Borrowings
        {
            get => _borrowings;
            set => SetProperty(ref _borrowings, value);
        }

        public ObservableCollection<BorrowingTicketDTO> FilteredBorrowings
        {
            get => _filteredBorrowings;
            set => SetProperty(ref _filteredBorrowings, value);
        }

        public BorrowingTicketDTO? SelectedBorrowing
        {
            get => _selectedBorrowing;
            set
            {
                SetProperty(ref _selectedBorrowing, value);
                OnPropertyChanged(nameof(HasSelection));
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                ApplyFilters();
            }
        }

        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                SetProperty(ref _selectedFilter, value);
                ApplyFilters();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public bool HasSelection => SelectedBorrowing != null;

        public List<string> FilterOptions => new()
        {
            "Tất cả",
            "Chờ duyệt",
            "Đang mượn",
            "Quá hạn",
            "Đã trả"
        };

        public ICommand LoadBorrowingsCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand CreateBorrowingCommand { get; }
        public ICommand ViewDetailCommand { get; }
        public ICommand ApproveCommand { get; }
        public ICommand RejectCommand { get; }
        public ICommand ReturnBookCommand { get; }
        public ICommand ReportLostCommand { get; }
        public ICommand ReportDamagedCommand { get; }




        private async Task LoadBorrowingsAsync()
        {
            try
            {
                IsLoading = true;

                var borrowings = await _borrowingService.GetAllBorrowingsAsync();
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Borrowings.Clear();
                    foreach (var borrowing in borrowings)
                    {
                        Borrowings.Add(borrowing);
                    }
                });

                ApplyFilters();

                OnPropertyChanged(nameof(FilteredBorrowings));
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi tải dữ liệu: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ApplyFilters()
        {
            var filtered = Borrowings.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var search = SearchText.ToLower();
                filtered = filtered.Where(b =>
                    b.MemberName.ToLower().Contains(search) ||
                    b.MemberEmail?.ToLower().Contains(search) == true ||
                    b.TicketId.ToString().Contains(search)
                );
            }

            if (SelectedFilter != "Tất cả")
            {
                filtered = filtered.Where(b =>
                    b.Details.Any(d => d.Status == SelectedFilter)
                );
            }

            FilteredBorrowings = new ObservableCollection<BorrowingTicketDTO>(filtered);
        }

        private void CreateBorrowing()
        {
            _dialogService.ShowMessage("Chức năng đang phát triển");
        }

        private void ViewDetail(object? parameter)
        {
            if (SelectedBorrowing == null) return;

            try
            {
                var borrowingService = App.ServiceProvider.GetRequiredService<IBorrowingService>();
                var dialogService = App.ServiceProvider.GetRequiredService<IDialogService>();

                var viewModel = new BorrowingDetailViewModel(
                    borrowingService,
                    dialogService,
                    SelectedBorrowing
                );

                var dialog = new Views.Borrowing.BorrowingDetailDialog(viewModel)
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private bool CanApprove()
        {
            if (SelectedBorrowing == null) return false;
            return SelectedBorrowing.BooksPending > 0;
        }

        private async Task ApproveAsync()
        {
            if (SelectedBorrowing == null) return;

            var pendingDetails = SelectedBorrowing.Details
                .Where(d => d.Status == "Chờ duyệt")
                .ToList();

            if (!pendingDetails.Any())
            {
                _dialogService.ShowMessage("Không có sách nào đang chờ duyệt");
                return;
            }

            var confirm = _dialogService.ShowConfirmation(
                $"Bạn có chắc muốn duyệt {pendingDetails.Count} sách?",
                "Xác nhận duyệt"
            );

            if (!confirm) return;

            try
            {
                IsLoading = true;
                int successCount = 0;
                var errors = new List<string>();

                foreach (var detail in pendingDetails)
                {
                    var request = new ApproveBorrowingRequest
                    {
                        DetailId = detail.DetailId,
                        StaffId = _authService.CurrentUser?.UserId ?? 0
                    };

                    var response = await _borrowingService.ApproveBorrowingAsync(request);

                    if (response.Success)
                    {
                        successCount++;
                    }
                    else
                    {
                        errors.Add($"Sách '{detail.BookTitle}': {response.Message}");
                    }
                }

                if (successCount > 0)
                {
                    _dialogService.ShowSuccess($"Duyệt thành công {successCount} sách!");
                }
                
                if (errors.Any())
                {
                    _dialogService.ShowError("Một số sách không thể duyệt:\n" + string.Join("\n", errors));
                }

                await LoadBorrowingsAsync();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi hệ thống: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanReject()
        {
            if (SelectedBorrowing == null) return false;
            return SelectedBorrowing.BooksPending > 0;
        }

        private async Task RejectAsync()
        {
            if (SelectedBorrowing == null) return;

            var pendingDetails = SelectedBorrowing.Details
                .Where(d => d.Status == "Chờ duyệt")
                .ToList();

            if (!pendingDetails.Any())
            {
                _dialogService.ShowMessage("Không có sách nào đang chờ duyệt");
                return;
            }

            var confirm = _dialogService.ShowConfirmation(
                $"Bạn có chắc muốn từ chối {pendingDetails.Count} sách?",
                "Xác nhận từ chối"
            );

            if (!confirm) return;

            try
            {
                IsLoading = true;
                int successCount = 0;
                var errors = new List<string>();

                foreach (var detail in pendingDetails)
                {
                    var request = new RejectBorrowingRequest
                    {
                        DetailId = detail.DetailId,
                        StaffId = _authService.CurrentUser?.UserId ?? 0,
                        Reason = "Staff từ chối"
                    };

                    var response = await _borrowingService.RejectBorrowingAsync(request);

                    if (response.Success)
                    {
                        successCount++;
                    }
                    else
                    {
                        errors.Add($"Sách '{detail.BookTitle}': {response.Message}");
                    }
                }

                if (successCount > 0)
                {
                    _dialogService.ShowSuccess($"Từ chối thành công {successCount} sách!");
                }
                
                if (errors.Any())
                {
                    _dialogService.ShowError("Một số sách không thể từ chối:\n" + string.Join("\n", errors));
                }

                await LoadBorrowingsAsync();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi hệ thống: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanReturn()
        {
            if (SelectedBorrowing == null) return false;
            return SelectedBorrowing.BooksNotReturned > 0;
        }

        private async Task ReturnBookAsync()
        {
            if (SelectedBorrowing == null) return;

            var booksToReturn = SelectedBorrowing.Details
                .Where(d => d.Status == "Đang mượn" || d.Status == "Quá hạn")
                .ToList();

            if (!booksToReturn.Any())
            {
                _dialogService.ShowMessage("Không có sách nào cần trả trong phiếu này");
                return;
            }

            try
            {
                var borrowingService = App.ServiceProvider.GetRequiredService<IBorrowingService>();
                var dialogService = App.ServiceProvider.GetRequiredService<IDialogService>();

                var viewModel = new ReturnBookViewModel(
                    borrowingService,
                    dialogService,
                    SelectedBorrowing,
                    onSuccess: async () =>
                    {
                        await LoadBorrowingsAsync(); 
                    }
                );

                var dialog = new Views.Borrowing.ReturnBookDialog(viewModel)
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi: {ex.Message}");
            }
        }

        private async Task ReportIssueAsync(string type)
        {
            if (SelectedBorrowing == null) return;

            var booksToReport = SelectedBorrowing.Details
                .Where(d => d.Status == "Đang mượn" || d.Status == "Quá hạn")
                .ToList();

            if (!booksToReport.Any())
            {
                _dialogService.ShowMessage("Không có sách nào đang mượn để báo sự cố.");
                return;
            }

            // For simplicity, if multiple books, we ask for the first one or we could let user select.
            // But usually, common practice is to select a book first. 
            // The current UI shows details on the right. 
            // Let's assume user wants to report for the SELECTED item in the right panel? 
            // Wait, the right panel is an ItemsControl, not a selectable list.
            
            // Re-thinking: Better to open the ReturnBookDialog but with an "Issue" mode, 
            // or just report for all "Đang mượn" books in the ticket (rare case).
            
            // Let's just report for the first one for now as a POC, 
            // but the REAL way is to have a selection in the detail panel.
            
            var detail = booksToReport.First(); 
            string typeName = type == "Lost" ? "mất" : "hỏng";

            var confirm = _dialogService.ShowConfirmation(
                $"Xác nhận báo {typeName} sách '{detail.BookTitle}'?",
                "Xác nhận báo sự cố"
            );

            if (confirm)
            {
                try
                {
                    IsLoading = true;
                    var request = new ReportIssueRequest
                    {
                        DetailId = detail.DetailId,
                        IssueType = type,
                        Notes = $"Báo {typeName} bởi Staff"
                    };

                    var response = await _borrowingService.ReportIssueAsync(request);
                    if (response.Success)
                    {
                        _dialogService.ShowSuccess(response.Message);
                        await LoadBorrowingsAsync();
                    }
                    else
                    {
                        _dialogService.ShowError(response.Message);
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
    }
}
