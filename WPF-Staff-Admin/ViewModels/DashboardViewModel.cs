using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WPF_Staff_Admin.Helpers;
using WPF_Staff_Admin.Models;
using WPF_Staff_Admin.Services;

namespace WPF_Staff_Admin.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private readonly IBookService _bookService;
        private readonly IBorrowingService _borrowingService;

        private int _totalBooks;
        private int _availableBooks;
        private int _activeBorrowings;
        private int _overdueBorrowings;
        private bool _isLoading;

        public string UserFullName => SessionManager.Instance.CurrentUser?.FullName ?? "User";

        public int TotalBooks
        {
            get => _totalBooks;
            set => SetProperty(ref _totalBooks, value);
        }

        public int AvailableBooks
        {
            get => _availableBooks;
            set => SetProperty(ref _availableBooks, value);
        }

        public int ActiveBorrowings
        {
            get => _activeBorrowings;
            set => SetProperty(ref _activeBorrowings, value);
        }

        public int OverdueBorrowings
        {
            get => _overdueBorrowings;
            set => SetProperty(ref _overdueBorrowings, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public DashboardViewModel(IBookService bookService, IBorrowingService borrowingService)
        {
            _bookService = bookService;
            _borrowingService = borrowingService;
            
            _ = LoadStatisticsAsync();
        }

        private async Task LoadStatisticsAsync()
        {
            IsLoading = true;
            try
            {
                var booksResponse = await _bookService.GetAllBooksAsync();
                if (booksResponse != null && booksResponse.Success && booksResponse.Data != null)
                {
                    TotalBooks = booksResponse.Data.Sum(b => b.TotalQuantity);
                    AvailableBooks = booksResponse.Data.Sum(b => b.AvailableQuantity);
                }

                var borrowingsResponse = await _borrowingService.GetAllBorrowingsAsync();
                if (borrowingsResponse != null && borrowingsResponse.Any())
                {
                    ActiveBorrowings = borrowingsResponse.Sum(t => t.Details.Count(d => d.Status == "Đang mượn"));
                    OverdueBorrowings = borrowingsResponse.Sum(t => t.Details.Count(d => d.Status == "Quá hạn"));
                }
            }
            catch (Exception ex)
            {
                // Handle or log error
                Console.WriteLine($"Error loading dashboard stats: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
