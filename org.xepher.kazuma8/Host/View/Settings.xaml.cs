using System.Linq;
using System.Windows.Controls;
using Framework.Serializer;
using Host.Utils;
using Microsoft.Phone.Controls;
using Framework.Common;
using Microsoft.Practices.ServiceLocation;

namespace Host.View
{
    public partial class Settings : PhoneApplicationPage
    {
        ISerializer serializer = ServiceLocator.Current.GetInstance<ISerializer>();
        //private bool _isLoaded = false;

        public Settings()
        {
            InitializeComponent();

            SettingsAllLinesCircle.ItemsSource = AppSettings.AllLinesCircleList;
            SettingsAnnouncementCircle.ItemsSource = AppSettings.AnnouncementCircleList;

            UpdateCircle _allLinesCircleSettings = IsolatedStorageHelper.Settings["AllLinesCircle"] as UpdateCircle;
            UpdateCircle _announcementCircleSettings = IsolatedStorageHelper.Settings["AnnouncementCircle"] as UpdateCircle;

            SettingsAllLinesCircle.SelectedItem =
                AppSettings.AllLinesCircleList.FirstOrDefault(s => s.Circle == _allLinesCircleSettings.Circle);
            SettingsAnnouncementCircle.SelectedItem =
                AppSettings.AnnouncementCircleList.FirstOrDefault(s => s.Circle == _announcementCircleSettings.Circle);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            GlobalLoading.Instance.IsLoading = false;
            base.OnBackKeyPress(e);
        }

        private void SettingsAllLinesCircle_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsolatedStorageHelper.AddOrUpdateSettings("AllLinesCircle", e.AddedItems[0]);
        }

        private void SettingsAnnouncementCircle_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsolatedStorageHelper.AddOrUpdateSettings("AnnouncementCircle", e.AddedItems[0]);
        }
    }
}