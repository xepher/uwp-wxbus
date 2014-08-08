using Framework.Common;
using GalaSoft.MvvmLight.Messaging;
using Host.Model;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Host.View
{
    public partial class Shell : PhoneApplicationPage
    {
        private bool _isLoadedAllLinesAndNews;

        public Shell()
        {
            InitializeComponent();

            if (!DeviceNetworkInformation.IsNetworkAvailable)
            {
                MessageBox.Show("Network unavailable");
                Application.Current.Terminate();
            }

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            GlobalLoading.Instance.IsLoading = false;
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

            if (!_isLoadedAllLinesAndNews)
            {
                Messenger.Default.Send<string>("", "LoadAllLinesAndNews");
                _isLoadedAllLinesAndNews = true;
            }
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Messenger.Default.Unregister<LineEntity>(this, "Navigate");
            base.OnNavigatedFrom(e);
        }

        private void txtSearchLine_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //Messenger.Default.Send<string>(txtSearchLine.Text, "SearchLine");
            }
        }

        private void txtSearchLine_LostFocus(object sender, RoutedEventArgs e)
        {
            //Messenger.Default.Send<string>(txtSearchLine.Text, "SearchLine");
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
    }
}