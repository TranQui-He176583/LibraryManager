using System;
using System.Windows;
using WPF_Staff_Admin.ViewModels.Users;

namespace WPF_Staff_Admin.Views.Users
{
    /// <summary>
    /// Interaction logic for UserFormWindow.xaml
    /// </summary>
    public partial class UserFormWindow : Window
    {
        private UserFormViewModel _viewModel;

        public UserFormWindow(UserFormViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            _viewModel.RequestClose += (s, e) => 
            {
                this.DialogResult = e;
                this.Close();
            };
        }

        private void PassBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.Password = PassBox.Password;
            }
        }

        private void NewPassBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.NewPassword = NewPassBox.Password;
            }
        }
    }
}
