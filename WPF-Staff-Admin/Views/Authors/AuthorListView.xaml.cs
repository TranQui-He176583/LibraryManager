using System.Windows.Controls;
using WPF_Staff_Admin.ViewModels.Authors;

namespace WPF_Staff_Admin.Views.Authors
{
    public partial class AuthorListView : UserControl
    {
        public AuthorListView(AuthorListViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
