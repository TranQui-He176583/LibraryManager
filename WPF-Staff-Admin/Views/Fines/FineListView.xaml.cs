using System.Windows.Controls;
using WPF_Staff_Admin.ViewModels.Fines;

namespace WPF_Staff_Admin.Views.Fines
{
    /// <summary>
    /// Interaction logic for FineListView.xaml
    /// </summary>
    public partial class FineListView : UserControl
    {
        public FineListView(FineListViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
