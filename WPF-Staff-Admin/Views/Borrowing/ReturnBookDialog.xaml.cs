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
using WPF_Staff_Admin.ViewModels.Borrowing;

namespace WPF_Staff_Admin.Views.Borrowing
{
    public partial class ReturnBookDialog : Window
    {
        public ReturnBookDialog(ReturnBookViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}