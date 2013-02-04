using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using org.xepher.lang;
using org.xepher.model;
using org.xepher.wuxibus.misc;

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

            InitializeUIComponent();

            AdHelper.InitializeAds(ContentPanel, this.ApplicationBar);
        }

        private void InitializeUIComponent()
        {
            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;

            List<Line> lines = (Application.Current as App).DAHelper.GetAllLine();

            linesList.ItemsSource = lines;
        }

        private void ApplicationBarLocalization()
        {
            ApplicationBar.Buttons.Clear();
            ApplicationBar.MenuItems.Clear();

            // add buttons
            ApplicationBarIconButton searchButton = new ApplicationBarIconButton()
                                                         {
                                                             Text = AppResource.ApplicationBarIconButtonSearch,
                                                             IconUri =
                                                                 new Uri(
                                                                 "Assets/Icons/dark/appbar.feature.search.rest.png",
                                                                 UriKind.Relative)
                                                         };
            searchButton.Click += new EventHandler(ApplicationBarIconButtonSearch_Click);

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

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBoxResult.Cancel == MessageBox.Show(AppResource.MsgExitApplication, AppResource.TitleExitApplication, MessageBoxButton.OKCancel))
            {
                e.Cancel = true;
            }
        }

        private void linesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isListBoxDataBinded)
            {
                Line selectedLine = linesList.SelectedItem as Line;
                (Application.Current as App).SelectedLine = selectedLine;
                NavigationService.Navigate(new Uri("/StationsPage.xaml", UriKind.Relative));
            }
        }

        private void ApplicationBarIconButtonSearch_Click(object sender, EventArgs e)
        {
            //if (string.IsNullOrEmpty((Application.Current as App).ViewState))
            //{
            //    MessageBox.Show(AppResource.MsgRefreshToken);
            //    return;
            //}
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
            linesList.SelectedIndex = -1;
            _isListBoxDataBinded = false;
        }
    }
}