using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF_Staff_Admin.ViewModels;

namespace WPF_Staff_Admin.Views.Books
{

    public partial class BookListView : UserControl
    {
        public BookListView()
        {
            InitializeComponent();
        }

        public BookListView(BookListViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var dataGrid = sender as DataGrid;
                if (dataGrid == null) return;

                var dep = (DependencyObject)e.OriginalSource;

                while (dep != null && dep is not DataGridRow && dep is not DataGridColumnHeader)
                {
                    dep = System.Windows.Media.VisualTreeHelper.GetParent(dep);
                }

                if (dep is DataGridColumnHeader) return;

                if (dep is not DataGridRow) return;

                if (DataContext is BookListViewModel viewModel && viewModel.SelectedBook != null)
                {
                    viewModel.ViewDetailsCommand.Execute(viewModel.SelectedBook);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở chi tiết sách: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
