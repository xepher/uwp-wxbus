using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using org.xepher.common;
using org.xepher.lang;
using org.xepher.model;

namespace org.xepher.wuxibus
{
    public partial class MainPage : PhoneApplicationPage
    {
        // 处理重新绑定ListBox数据源时会触发SelectionChanged事件
        private bool _isListBoxDataBinded = false;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            ApplicationBarLocalization();
            
            //Object obj = IsolatedStorage.ReadFromFile(string.Format("Data\\Routes.data"), typeof(List<Route>));

            //if (obj == null)
            //{
                ApplicationBar.IsMenuEnabled = false;
                foreach (ApplicationBarIconButton button in ApplicationBar.Buttons)
                {
                    button.IsEnabled = false;
                }

                Downloader.LoadRoutes(RoutesResponseCallback, (Application.Current as App).Container);
            //}
            //else
            //{
            //    routesList.ItemsSource = obj as List<Route>;
            //}
        }

        private void ApplicationBarLocalization()
        {
            ApplicationBar.Buttons.Clear();
            ApplicationBar.MenuItems.Clear();

            // add buttons
            ApplicationBarIconButton refreshButton = new ApplicationBarIconButton()
                                                         {
                                                             Text = AppResource.ApplicationBarIconButtonRefresh,
                                                             IconUri =
                                                                 new Uri("Assets/Icons/dark/appbar.refresh.rest.png",
                                                                         UriKind.Relative),
                                                         };
            refreshButton.Click += new EventHandler(ApplicationBarIconButtonRefresh_Click);

            ApplicationBarIconButton searchButton = new ApplicationBarIconButton()
                                                         {
                                                             Text = AppResource.ApplicationBarIconButtonSearch,
                                                             IconUri =
                                                                 new Uri(
                                                                 "Assets/Icons/dark/appbar.feature.search.rest.png",
                                                                 UriKind.Relative)
                                                         };
            searchButton.Click += new EventHandler(ApplicationBarIconButtonSearch_Click);

            ApplicationBar.Buttons.Add(refreshButton);
            ApplicationBar.Buttons.Add(searchButton);

            // add menuitems
            ApplicationBarMenuItem settingsMenuItem = new ApplicationBarMenuItem()
            {
                Text = AppResource.ApplicationBarMenuItemSettings
            };
            settingsMenuItem.Click += new EventHandler(ApplicationBarMenuItemSettings_Click);

            ApplicationBarMenuItem aboutMenuItem = new ApplicationBarMenuItem()
                                                      {
                                                          Text = AppResource.ApplicationBarMenuItemAbout
                                                      };
            aboutMenuItem.Click += new EventHandler(ApplicationBarMenuItemAbout_Click);

            ApplicationBar.MenuItems.Add(settingsMenuItem);
            ApplicationBar.MenuItems.Add(aboutMenuItem);
        }

        private void RoutesResponseCallback(IAsyncResult ar)
        {
            HttpWebRequest request = (HttpWebRequest)ar.AsyncState;
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(ar);

            Downloader.GetRandomming(iar => { }, (Application.Current as App).Container);

            string result;

            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                (Application.Current as App).RawDefaultHtml = result = reader.ReadToEnd();
                Dispatcher.BeginInvoke(() =>
                                           {
                                               // viewstate save
                                               (Application.Current as App).ViewState = Common.GetViewState(result);

                                               _isListBoxDataBinded = true;

                                               // Resolve Routes
                                               // todo:比较 (Application.Current as App).Routes 与 Common.ResolveRoutes(e.Result) 是否一样
                                               // 不一样需要将Common.ResolveRoutes(e.Result)写入
                                               routesList.ItemsSource =
                                                   (Application.Current as App).Routes = Common.ResolveRoutes(result);

                                               routesList.SelectedIndex = -1;
                                               _isListBoxDataBinded = false;

                                               // todo: Async save Routes information
                                               IsolatedStorage.SaveToFile((Application.Current as App).Routes,
                                                                          "Data\\Routes.data");

                                               ApplicationBar.IsMenuEnabled = true;
                                               foreach (ApplicationBarIconButton button in ApplicationBar.Buttons)
                                               {
                                                   button.IsEnabled = true;
                                               }

                                               GlobalLoading.Instance.IsLoading = false;
                                           });
            }
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBoxResult.Cancel == MessageBox.Show(AppResource.MsgExitApplication, AppResource.TitleExitApplication, MessageBoxButton.OKCancel))
            {
                e.Cancel = true;
            }
        }

        private void routesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isListBoxDataBinded)
            {
                Route selectedRoute = routesList.SelectedItem as Route;
                if (string.IsNullOrEmpty((Application.Current as App).ViewState))
                {
                    object obj = IsolatedStorage.ReadFromFile(string.Format("Data\\{0}.data", selectedRoute.Value),
                                                              typeof(List<Direction>));
                    if (obj == null)
                    {
                        MessageBox.Show(AppResource.MsgRefreshToken);
                        return;
                    }
                }
                (Application.Current as App).SelectedRoute = selectedRoute;
                NavigationService.Navigate(new Uri("/StationsPage.xaml", UriKind.Relative));
            }
        }

        // refresh routes, download routes information
        private void ApplicationBarIconButtonRefresh_Click(object sender, EventArgs e)
        {
            if (MessageBoxResult.OK ==
                MessageBox.Show(AppResource.MsgRefreshRoutes, AppResource.TitleRefreshRoutes, MessageBoxButton.OKCancel))
            {
                ApplicationBar.IsMenuEnabled = false;
                foreach (ApplicationBarIconButton button in ApplicationBar.Buttons)
                {
                    button.IsEnabled = false;
                }

                Downloader.LoadRoutes(RoutesResponseCallback, (Application.Current as App).Container);
            }
        }

        // navigate to search page
        private void ApplicationBarIconButtonSearch_Click(object sender, EventArgs e)
        {
            // todo: if you search uncached route, there will throw an exception, because the token is null
            if (string.IsNullOrEmpty((Application.Current as App).ViewState))
            {
                MessageBox.Show(AppResource.MsgRefreshToken);
                return;
            }
            NavigationService.Navigate(new Uri("/SearchPage.xaml?search=route", UriKind.Relative));
        }

        private void ApplicationBarMenuItemAbout_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarMenuItemSettings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingPage.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            _isListBoxDataBinded = true;
            routesList.SelectedIndex = -1;
            _isListBoxDataBinded = false;
        }
    }
}