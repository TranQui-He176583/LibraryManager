using System;
using System.Collections.Generic;
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
    public class BorrowingDetailViewModel : ViewModelBase
    {
        private readonly IBorrowingService _borrowingService;
        private readonly IDialogService _dialogService;
        private BorrowingTicketDTO _ticket;

        public BorrowingDetailViewModel(
            IBorrowingService borrowingService,
            IDialogService dialogService,
            BorrowingTicketDTO ticket)
        {
            _borrowingService = borrowingService;
            _dialogService = dialogService;
            _ticket = ticket;

            CloseCommand = new RelayCommand(_ => Close());
        }

        public BorrowingTicketDTO Ticket
        {
            get => _ticket;
            set => SetProperty(ref _ticket, value);
        }

        public ICommand CloseCommand { get; }
        private void Close()
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
