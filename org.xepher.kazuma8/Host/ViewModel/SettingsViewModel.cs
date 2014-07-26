using Framework.NavigationService;
using GalaSoft.MvvmLight;

namespace Host.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private INavigationService _navigationService;

        public SettingsViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
    }
}
