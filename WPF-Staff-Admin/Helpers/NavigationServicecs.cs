using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WPF_Staff_Admin.Helpers
{
    public interface INavigationService
    {
        void NavigateTo(UserControl view);
        void GoBack();
        bool CanGoBack { get; }
    }

    public class NavigationService : INavigationService
    {
        private readonly Stack<UserControl> _navigationStack = new();
        private ContentControl? _contentControl;
        public bool CanGoBack => _navigationStack.Count > 1;
        public void Initialize(ContentControl contentControl)
        {
            _contentControl = contentControl;
        }

        public void NavigateTo(UserControl view)
        {
            if (_contentControl == null)
                throw new InvalidOperationException("NavigationService chưa được khởi tạo");

            _navigationStack.Push(view);
            _contentControl.Content = view;
        }

        public void GoBack()
        {
            if (!CanGoBack || _contentControl == null)
                return;

            _navigationStack.Pop(); 
            var previousView = _navigationStack.Peek();
            _contentControl.Content = previousView;
        }

        public void ClearHistory()
        {
            _navigationStack.Clear();
        }
    }
}
