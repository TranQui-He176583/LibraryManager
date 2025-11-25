using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPF_Staff_Admin.Helpers
{
    public interface IDialogService
    {
        void ShowMessage(string message, string title = "Thông báo");
        void ShowError(string message, string title = "Lỗi");
        bool ShowConfirmation(string message, string title = "Xác nhận");
        void ShowSuccess(string message, string title = "Thành công");
    }

    public class DialogService : IDialogService
    {
        public void ShowMessage(string message, string title = "Thông báo")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowError(string message, string title = "Lỗi")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public bool ShowConfirmation(string message, string title = "Xác nhận")
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        public void ShowSuccess(string message, string title = "Thành công")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
