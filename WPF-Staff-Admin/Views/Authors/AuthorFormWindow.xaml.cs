using System;
using System.Windows;
using WPF_Staff_Admin.ViewModels.Authors;

namespace WPF_Staff_Admin.Views.Authors
{
    public partial class AuthorFormWindow : Window
    {
        public AuthorFormWindow(AuthorFormViewModel viewModel)
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
