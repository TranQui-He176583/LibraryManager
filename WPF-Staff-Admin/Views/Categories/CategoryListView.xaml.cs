using System.Windows.Controls;
using WPF_Staff_Admin.ViewModels.Categories;

namespace WPF_Staff_Admin.Views.Categories
{
    public partial class CategoryListView : UserControl
    {
        public CategoryListView(CategoryListViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
