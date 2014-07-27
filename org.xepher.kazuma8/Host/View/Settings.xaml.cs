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

            SettingsAnnouncementCircle.ItemsSource = AppSettings.AnnouncementCircleList;

            AnnounceUpdateCircle _circleSettings = IsolatedStorageHelper.Settings["AnnouncementCircle"] as AnnounceUpdateCircle;
            SettingsAnnouncementCircle.SelectedItem =
                AppSettings.AnnouncementCircleList.FirstOrDefault(s => s.Circle == _circleSettings.Circle);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            GlobalLoading.Instance.IsLoading = false;
            base.OnBackKeyPress(e);
        }

        private void SettingsAnnouncementCircle_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (_isLoaded)
            //{
            //if (IsolatedStorageHelper.Settings["AnnouncementCircle"].ToString() != e.AddedItems[0].ToString())
            //{
            IsolatedStorageHelper.AddOrUpdateSettings("AnnouncementCircle", e.AddedItems[0]);
            //}
            //}
            //_isLoaded = true;
        }
    }
}