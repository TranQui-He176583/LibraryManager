using System.Windows.Controls;
using WPF_Staff_Admin.ViewModels.Publishers;

namespace WPF_Staff_Admin.Views.Publishers
{
    public partial class PublisherListView : UserControl
    {
        public PublisherListView(PublisherListViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
