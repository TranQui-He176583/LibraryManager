using System.Windows;
using WPF_Staff_Admin.ViewModels.Publishers;

namespace WPF_Staff_Admin.Views.Publishers
{
    public partial class PublisherFormWindow : Window
    {
        public PublisherFormWindow(PublisherFormViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.RequestClose += (s, e) =>
            {
                this.DialogResult = e;
                this.Close();
            };
        }
    }
}
