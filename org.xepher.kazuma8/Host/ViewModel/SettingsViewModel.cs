using System.Collections.Generic;
using Framework.NavigationService;
using GalaSoft.MvvmLight;
using Host.Utils;

namespace Host.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private INavigationService _navigationService;

        private const string LocalStorageSwitchPropertyName = "LocalStorageSwitch";

        private bool _localStorageSwitch;

        public bool LocalStorageSwitch
        {
            get
            {
                return _localStorageSwitch;
            }
            set
            {
                Set(LocalStorageSwitchPropertyName, ref _localStorageSwitch, value);
                IsolatedStorageHelper.AddOrUpdateSettings(Constants.SETTINGS_IS_LOCALSTORAGE_ENABLED, value);
            }
        }

        public SettingsViewModel(INavigationService navigationService)
            : this()
        {
            _navigationService = navigationService;
        }

        public SettingsViewModel()
        {
            _localStorageSwitch = (bool)IsolatedStorageHelper.Settings[Constants.SETTINGS_IS_LOCALSTORAGE_ENABLED];
        }
    }
}
