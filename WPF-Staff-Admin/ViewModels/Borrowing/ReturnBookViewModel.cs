using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPF_Staff_Admin.Helpers;
using WPF_Staff_Admin.Models;
using WPF_Staff_Admin.Services;

namespace WPF_Staff_Admin.ViewModels.Borrowing
{
    public class ReturnBookViewModel : ViewModelBase
    {
        private readonly IBorrowingService _borrowingService;
        private readonly IDialogService _dialogService;
        private readonly Action? _onSuccess;

        private BorrowingTicketDTO _ticket;
        private ObservableCollection<BorrowingDetailDTO> _returningBooks = new();
        private DateTime _returnDate = DateTime.Now;
        private string? _notes;
        private bool _isReturning;

        public ReturnBookViewModel(
            IBorrowingService borrowingService,
            IDialogService dialogService,
            BorrowingTicketDTO ticket,
            Action? onSuccess = null)
        {
            _borrowingService = borrowingService;
            _dialogService = dialogService;
            _ticket = ticket;
            _onSuccess = onSuccess;

            LoadReturningBooks();

            // Commands
            ReturnCommand = new RelayCommand(async _ => await ReturnAsync(), _ => CanReturn());
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        public BorrowingTicketDTO Ticket
        {
            get => _ticket;
            set => SetProperty(ref _ticket, value);
        }

        public ObservableCollection<BorrowingDetailDTO> ReturningBooks
        {
            get => _returningBooks;
            set => SetProperty(ref _returningBooks, value);
        }

        public DateTime ReturnDate
        {
            get => _returnDate;
            set => SetProperty(ref _returnDate, value);
        }

        public string? Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        public bool IsReturning
        {
            get => _isReturning;
            set => SetProperty(ref _isReturning, value);
        }

        public int TotalSelectedBooks => ReturningBooks?.Count(b => b.IsSelected) ?? 0;


        public ICommand ReturnCommand { get; }
        public ICommand CancelCommand { get; }

        private void LoadReturningBooks()
        {
            var booksToReturn = Ticket.Details
                .Where(d => d.Status == "Đang mượn" || d.Status == "Quá hạn")
                .ToList();

            ReturningBooks.Clear();

            foreach (var book in booksToReturn)
            {
                book.IsSelected = false;
                ReturningBooks.Add(book);
            }

            Console.WriteLine($"Loaded {ReturningBooks.Count} books that can be returned");
        }

        private bool CanReturn()
        {
            return TotalSelectedBooks > 0 && !IsReturning;
        }

        private async Task ReturnAsync()
        {
            try
            {
                var selectedBooks = ReturningBooks.Where(b => b.IsSelected).ToList();

                if (!selectedBooks.Any())
                {
                    _dialogService.ShowMessage("Vui lòng chọn ít nhất 1 cuốn sách để trả");
                    return;
                }

                var confirm = _dialogService.ShowConfirmation(
                    $"Xác nhận trả {selectedBooks.Count} cuốn sách?",
                    "Xác nhận trả sách"
                );

                if (!confirm) return;

                IsReturning = true;

                int successCount = 0;

                foreach (var book in selectedBooks)
                {
                    var request = new ReturnBookRequest
                    {
                        TicketId = Ticket.TicketId,
                        BookId = book.BookId,
                        ReturnDate = ReturnDate,
                        Notes = Notes
                    };


                    var response = await _borrowingService.ReturnBookAsync(request);

                    if (response.Success)
                    {
                        successCount++;
                 
                    }
                    else
                    {
                   
                        _dialogService.ShowError($"Lỗi trả sách '{book.BookTitle}': {response.Message}");
                        break;
                    }
                }

                if (successCount > 0)
                {
                    _dialogService.ShowSuccess($"Đã trả {successCount} cuốn sách thành công!");
                    _onSuccess?.Invoke();
                    CloseDialog();
                }
            }
            catch (Exception ex)
            {
                
                _dialogService.ShowError($"Lỗi: {ex.Message}");
            }
            finally
            {
                IsReturning = false;
            }
        }

        private void Cancel()
        {
            CloseDialog();
        }

        private void CloseDialog()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}