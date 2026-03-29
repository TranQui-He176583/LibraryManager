using System.Windows;
using WPF_Staff_Admin.ViewModels.Categories;

namespace WPF_Staff_Admin.Views.Categories
{
    public partial class CategoryFormWindow : Window
    {
        public CategoryFormWindow(CategoryFormViewModel viewModel)
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
