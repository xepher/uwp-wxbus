using System;

namespace Framework.NavigationService
{
    public interface INavigationService
    {
        void Navigate(Uri targetUri);

        void Navigate(Uri targetUri, object parameter);

        void GoBack();
    }
}
