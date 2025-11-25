using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF_Staff_Admin.ViewModels;

namespace WPF_Staff_Admin.Views.Books
{
    public partial class BookFormView : Window
    {
        public BookFormView(BookFormViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
