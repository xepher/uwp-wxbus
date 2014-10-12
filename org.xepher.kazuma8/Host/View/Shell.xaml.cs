using System.Windows.Controls;
using Coding4Fun.Toolkit.Controls;
using Framework.Common;
using GalaSoft.MvvmLight.Messaging;
using Host.Model;
using Host.Utils;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using System;
using System.Windows;
using System.Windows.Navigation;

namespace Host.View
{
    public partial class Shell : PhoneApplicationPage
    {
        private bool _isLoadedLinesAndNews;
        private bool _isExit;

        public Shell()
        {
            InitializeComponent();

            if (!DeviceNetworkInformation.IsNetworkAvailable)
            {
                MessageBox.Show(Constants.MSG_NETWORK_UNAVAILABLE);
                Application.Current.Terminate();
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            GlobalLoading.Instance.IsLoading = false;

            if (!_isExit)
            {
                _isExit = true;
                var toast = new ToastPrompt { Message = "再按一次退出程序" };
                toast.Completed += (o, ex) => { _isExit = false; };
                toast.Show();
                e.Cancel = true;
            }
            else
            {
                NavigationService.RemoveBackEntry();
            }

            base.OnBackKeyPress(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Messenger.Default.Register<LineEntity>(this, "Navigate", s =>
            {
                if (null != s)
                {
                    NavigationService.Navigate(new Uri("/View/Segment.xaml", UriKind.Relative));
                }
            });
            Messenger.Default.Register<string>(this, "NavigateSettings", s =>
            {
                if (null != s)
                {
                    NavigationService.Navigate(new Uri("/View/Settings.xaml", UriKind.Relative));
                }
            });

            string routeId = string.Empty;
            string routeName = string.Empty;
            bool routeIdResult = NavigationContext.QueryString.TryGetValue("routeId", out routeId);
            bool routeNameResult = NavigationContext.QueryString.TryGetValue("routeName", out routeName);
            if (routeIdResult && routeNameResult)
            {
                if (!string.IsNullOrEmpty(routeId) && !string.IsNullOrEmpty(routeName))
                {
                    NavigationContext.QueryString.Clear();
                    Messenger.Default.Send<LineEntity>(new LineEntity { RouteId = routeId, RouteName = routeName }, "Navigate");
                }
            }
            else
            {
                if (!_isLoadedLinesAndNews)
                {
                    Messenger.Default.Send<string>("", "LoadLinesAndNews");
                    _isLoadedLinesAndNews = true;
                }
            }
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Messenger.Default.Unregister<LineEntity>(this, "Navigate");
            base.OnNavigatedFrom(e);
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

        private void AppMenuSettings_OnClick(object sender, EventArgs e)
        {
            Messenger.Default.Send<string>("", "NavigateSettings");
        }

        private void txtSearchLine_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Messenger.Default.Send<string>(((TextBox)sender).Text, "FilterLines");
        }
    }
}