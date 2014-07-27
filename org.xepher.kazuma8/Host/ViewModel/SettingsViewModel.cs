using System.Collections.Generic;
using Framework.NavigationService;
using GalaSoft.MvvmLight;
using Host.Utils;

namespace Host.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        private INavigationService _navigationService;

        //public readonly IList<AnnounceUpdateCircle> AnnouncementCircle = AppSettings.AnnouncementCircleList;
        
        public SettingsViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
    }
}
