using System.Windows.Controls;
using WPF_Staff_Admin.ViewModels.Borrowing;

namespace WPF_Staff_Admin.Views.Borrowing
{
    public partial class BorrowingListView : UserControl
    {
        public BorrowingListView(BorrowingListViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
