using System;
using System.Windows.Controls;

namespace Framework.NavigationService
{
    public sealed class NavigationService : INavigationService
    {
        private readonly Frame _frame;

        public NavigationService(Frame frame)
        {
            _frame = frame;
        }

        public void Navigate(Uri targetUri)
        {
            _frame.Navigate(targetUri);
        }

        public void Navigate(Uri targetUri, object parameter)
        {
            _frame.Navigate(targetUri);
        }

        public void GoBack()
        {
            _frame.GoBack();
        }
    }
}
