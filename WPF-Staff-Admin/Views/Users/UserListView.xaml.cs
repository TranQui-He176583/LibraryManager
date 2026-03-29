using System.Windows.Controls;

namespace WPF_Staff_Admin.Views.Users
{
    /// <summary>
    /// Interaction logic for UserListView.xaml
    /// </summary>
    public partial class UserListView : UserControl
    {
        public UserListView()
        {
            InitializeComponent();
        }

        public UserListView(object viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}
