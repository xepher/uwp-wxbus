using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Host.Resources;
using System.Text;
using Host.Utils;
using Host.ViewModel;
using wuxibus.Model;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Framework.Navigator;
using GalaSoft.MvvmLight.Messaging;

namespace Host.View
{
    public partial class Shell : PhoneApplicationPage
    {
        // Constructor
        public Shell()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/View/Settings.xaml", UriKind.Relative));
        }

        //private async Task<IList<LineEntity>> SearchLine()
        //{
        //    return await SignatureUtil.BeginWebRequest<List<LineEntity>>(requestUrl);
        //}

        private async void txtSearchLine_LostFocus(object sender, RoutedEventArgs e)
        {
            string templateLine = "http://app.wifiwx.com/bus/api.php?a=query_line&k={0}&nonce={1}&secret=640c7088ef7811e2a4e4005056991a1f&version=0.1";
            string requestUrl = SignatureUtil.GetRealRequestUrl(string.Format(templateLine, HttpUtility.UrlEncode(txtSearchLine.Text.Trim()), SignatureUtil.GenerateSeqId()));

            IList<LineEntity> lines = await SignatureUtil.BeginWebRequest<List<LineEntity>>(requestUrl);

            ((ShellViewModel)DataContext).Lines = lines;
        }

        private void grdLineItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //LineEntity line = (LineEntity)(((Grid)sender).DataContext);
            //this.NavigationService.Navigate(new Uri(string.Format("/View/Segment.xaml?routeId={0}&segmentId={1}&segmentName={2}", line.RouteId, line.SegmentId, line.SegmentName), UriKind.Relative));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Messenger.Default.Register<string>(this, "Navigate", uri =>
            {
                if (uri != null)
                {
                    NavigationService.Navigate(new Uri(uri, UriKind.Relative));
                }
            });
            base.OnNavigatedTo(e);
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}