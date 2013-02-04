using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.Linq;
using Microsoft.Phone.Shell;
using org.xepher.lang;
using org.xepher.model;

namespace org.xepher.wuxibus
{
    public partial class StationsPage : PhoneApplicationPage
    {
        // 处理重新绑定ListBox数据源时会触发SelectionChanged事件的问题
        private bool _isListBoxDataBinded = false;

        private Line Line { get; set; }
        private List<Segment> Segments { get; set; }
        private Segment SelectedSegment { get; set; }
        private int SegmentIndex = 0;

        private bool _isAddedDirectionButton = false;
        private ApplicationBarIconButton btnDirection;

        public StationsPage()
        {
            InitializeComponent();

            ApplicationBarLocalization();

            InitializeUIComponent();
        }

        private void InitializeUIComponent()
        {
            Line = (Application.Current as App).SelectedLine;
            Segments = (Application.Current as App).DAHelper.GetSegment(Line.line_id);
            (Application.Current as App).SelectedSegment = SelectedSegment = Segments.First();
            List<Station> lstStation = (Application.Current as App).DAHelper.GetStation(Line.line_id,
                                                                                        SelectedSegment.segment_id);
            stationList.ItemsSource = lstStation;
            (Application.Current as App).StationSplitNumber = lstStation.Max(s => s.station_num);

            LineInfo.Text = Line.line_info;
            PageTitle.Text = SelectedSegment.segment_name;

            if (!_isAddedDirectionButton)
            {
                if (Segments.Count > 1)
                {
                    AddDirectionButton();
                    _isAddedDirectionButton = true;
                }
            }
        }

        private void ApplicationBarLocalization()
        {
            ApplicationBar.Buttons.Clear();
            ApplicationBar.MenuItems.Clear();

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

        private void AddDirectionButton()
        {
            btnDirection = new ApplicationBarIconButton()
                               {
                                   IconUri =
                                       new Uri(
                                       "Assets/Icons/dark/appbar.next.rest.png",
                                       UriKind.Relative),
                                   IsEnabled = true,
                                   Text = AppResource.ApplicationBarIconButtonDirection
                               };

            btnDirection.Click += new EventHandler(ApplicationBarIconButtonDirection_Click);

            ApplicationBar.Buttons.Add(btnDirection);
        }

        private void stationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isListBoxDataBinded)
            {
                (Application.Current as App).SelectedStation = stationList.SelectedItem as Station;
                NavigationService.Navigate(new Uri("/BusesPage.xaml", UriKind.Relative));
            }
        }

        private void ApplicationBarIconButtonDirection_Click(object sender, EventArgs e)
        {
            if (++SegmentIndex >= Segments.Count()) SegmentIndex = 0;
            (Application.Current as App).SelectedSegment = SelectedSegment = Segments[SegmentIndex];
            stationList.ItemsSource = (Application.Current as App).DAHelper.GetStation(Line.line_id,
                                                                                        SelectedSegment.segment_id);

            PageTitle.Text = SelectedSegment.segment_name;
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
            stationList.SelectedIndex = -1;
            _isListBoxDataBinded = false;
        }
    }
}