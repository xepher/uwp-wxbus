using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.Linq;
using Microsoft.Phone.Shell;
using org.xepher.common;
using org.xepher.lang;
using org.xepher.model;

namespace org.xepher.wuxibus
{
    public partial class StationsPage : PhoneApplicationPage
    {
        // 处理重新绑定ListBox数据源时会触发SelectionChanged事件的问题
        private bool _isListBoxDataBinded = false;

        private Route Route { get; set; }
        private List<Direction> Directions { get; set; }
        private bool _isAddedDirectionButton = false;
        private int _stationCount;
        private ApplicationBarIconButton btnDirection;
        private Direction _direction;

        public StationsPage()
        {
            InitializeComponent();

            ApplicationBarLocalization();

            Route = (Application.Current as App).SelectedRoute;
            PageTitle.Text = Route.Name;

            Object obj = IsolatedStorage.ReadFromFile(string.Format("Data\\{0}.data", Route.Value), typeof(List<Direction>));

            if (obj == null)
            {
                ApplicationBar.IsMenuEnabled = false;
                foreach (ApplicationBarIconButton button in ApplicationBar.Buttons)
                {
                    button.IsEnabled = false;
                }

                Downloader.LoadStations(StationsRequestCallback, (Application.Current as App).Container);
            }
            else
            {
                Directions = (List<Direction>)obj;
                stationsList.ItemsSource = Directions.First(d => d.IsSelected).Stations;
                if (!_isAddedDirectionButton)
                {
                    if (Directions.Count > 1)
                    {
                        AddDirectionButton();
                        _isAddedDirectionButton = true;
                    }
                }
                txtInformation.Text = Directions.First(d => d.IsSelected).Name;
            }
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

            ApplicationBar.Buttons.Add(refreshButton);

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

        private void StationsRequestCallback(IAsyncResult ar)
        {
            HttpWebRequest request = (HttpWebRequest)ar.AsyncState;

            const string formatString = "__EVENTTARGET=ddlRoute&__EVENTARGUMENT=&__LASTFOCUS=&__VIEWSTATE={0}&txtRandom=&ddlRoute={1}&hidSngserialIDValue=&hidSngserialIDValueList=&hidJudgeFlg=&hidsngserialID=&hiddualserialID=&hidX=&hidY=";
            string postString = string.Format(formatString, (Application.Current as App).ViewState, Route.Value);
            byte[] postData = Encoding.UTF8.GetBytes(postString);

            using (Stream postStream = request.EndGetRequestStream(ar))
            {
                postStream.Write(postData, 0, postData.Length);
            }
            request.BeginGetResponse(StationsResponseCallback, request);
        }

        private void StationsResponseCallback(IAsyncResult ar)
        {
            HttpWebRequest request = (HttpWebRequest)ar.AsyncState;
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(ar);

            Downloader.GetRandomming(iar => { }, (Application.Current as App).Container);

            string result;

            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                result = reader.ReadToEnd();

                // viewstate save
                (Application.Current as App).ViewState = Common.GetViewState(result);

                Dispatcher.BeginInvoke(() =>
                                           {
                                               _isListBoxDataBinded = true;

                                               // Resolve Stations
                                               Directions = Common.ResolveStations(result);
                                               Direction newDirection = Directions.First(d => d.IsSelected);

                                               stationsList.ItemsSource = newDirection.Stations;

                                               if (!_isAddedDirectionButton)
                                               {
                                                   if (Directions.Count > 1)
                                                   {
                                                       AddDirectionButton();
                                                       _isAddedDirectionButton = true;
                                                   }
                                               }
                                               txtInformation.Text = Directions.First(d => d.IsSelected).Name;

                                               stationsList.SelectedIndex = -1;
                                               _isListBoxDataBinded = false;

                                               // todo: Async save Stations information
                                               IsolatedStorage.SaveToFile(Directions, string.Format("Data\\{0}.data", Route.Value));

                                               ApplicationBar.IsMenuEnabled = true;
                                               foreach (ApplicationBarIconButton button in ApplicationBar.Buttons)
                                               {
                                                   button.IsEnabled = true;
                                               }

                                               GlobalLoading.Instance.IsLoading = false;
                                           });
            }
        }

        private void StationDirectionRequestCallback(IAsyncResult ar)
        {
            HttpWebRequest request = (HttpWebRequest)ar.AsyncState;

            const string formatString = "__EVENTTARGET=ddlSegment&__EVENTARGUMENT=&__LASTFOCUS=&__VIEWSTATE={0}&txtRandom=&ddlRoute={1}&ddlSegment={2}&hidSngserialIDValue=&hidSngserialIDValueList=&hidJudgeFlg=&hidsngserialID=&hiddualserialID=&hidX=&hidY=";
            string postString = string.Format(formatString, (Application.Current as App).ViewState, Route.Value,
                                              _direction.Value);

            // add rpt$ctl00$hidSngserialID,rpt$ctl00$hidDualserialID etc
            StringBuilder sb = new StringBuilder(postString);
            for (int i = 0; i < _stationCount; i++)
            {
                sb.Append(string.Format("&rpt$ctl{0}$hidSngserialID={1}&rpt$ctl{0}$hidDualserialID={1}",
                                        i < 10 ? "0" + i.ToString() : i.ToString(), i + 1));
            }

            byte[] postData = Encoding.UTF8.GetBytes(sb.ToString());

            using (Stream postStream = request.EndGetRequestStream(ar))
            {
                postStream.Write(postData, 0, postData.Length);
            }
            request.BeginGetResponse(StationDirectionResponseCallback, request);
        }

        private void StationDirectionResponseCallback(IAsyncResult ar)
        {
            HttpWebRequest request = (HttpWebRequest)ar.AsyncState;
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(ar);

            Downloader.GetRandomming(iar => { }, (Application.Current as App).Container);

            string result;

            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                result = reader.ReadToEnd();

                // viewstate save
                (Application.Current as App).ViewState = Common.GetViewState(result);

                Dispatcher.BeginInvoke(() =>
                                           {
                                               _isListBoxDataBinded = true;

                                               // Resolve Stations
                                               Direction newDirection =
                                                   Common.ResolveStations(result).First(d => d.IsSelected);
                                               Direction matchedDirection =
                                                   Directions.First(d => d.Value == newDirection.Value);
                                               if (matchedDirection.Stations == null)
                                               {
                                                   matchedDirection.Stations = newDirection.Stations;

                                                   // 设置回默认的线路顺序
                                                   _direction = Directions.First(d => !d.IsSelected);

                                                   _direction.IsSelected = true;
                                                   matchedDirection.IsSelected = false;
                                               }

                                               stationsList.ItemsSource = newDirection.Stations;

                                               if (!_isAddedDirectionButton)
                                               {
                                                   if (Directions.Count > 1)
                                                   {
                                                       AddDirectionButton();
                                                       _isAddedDirectionButton = true;
                                                   }
                                               }
                                               txtInformation.Text = Directions.First(d => d.IsSelected).Name;

                                               stationsList.SelectedIndex = -1;
                                               _isListBoxDataBinded = false;

                                               // todo: Async save Stations information
                                               IsolatedStorage.SaveToFile(Directions, string.Format("Data\\{0}.data", Route.Value));

                                               ApplicationBar.IsMenuEnabled = true;
                                               foreach (ApplicationBarIconButton button in ApplicationBar.Buttons)
                                               {
                                                   button.IsEnabled = true;
                                               }

                                               GlobalLoading.Instance.IsLoading = false;
                                           });
            }
        }

        private void stationsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isListBoxDataBinded)
            {
            }
        }

        // 下载车辆信息
        private void BusInformationDownloaded(object sender, UploadStringCompletedEventArgs e)
        {
            // Resolve Busses
            List<Bus> busses = Common.ResolveBusses(e.Result);

            GlobalLoading.Instance.IsLoading = false;
        }

        private void ApplicationBarIconButtonRefresh_Click(object sender, EventArgs e)
        {
            if (MessageBoxResult.OK ==
                MessageBox.Show(string.Format(AppResource.MsgRefreshStations, Route.Name), AppResource.TitleRefreshStations, MessageBoxButton.OKCancel))
            {

                ApplicationBar.IsMenuEnabled = false;
                foreach (ApplicationBarIconButton button in ApplicationBar.Buttons)
                {
                    button.IsEnabled = false;
                }

                Downloader.LoadStations(StationsRequestCallback, (Application.Current as App).Container);
            }
        }

        private void ApplicationBarIconButtonDirection_Click(object sender, EventArgs e)
        {
            // 前面添加direction按钮的时候已经检查过是否只有单条线路，所以这里不检查
            _direction = Directions.First(d => !d.IsSelected);
            Direction direSelected = Directions.First(d => d.IsSelected);

            _direction.IsSelected = true;
            direSelected.IsSelected = false;

            // 如果为null表示该route的此方向站点信息并没有收录过，需要访问网络获取
            if (_direction.Stations == null)
            {
                _stationCount = direSelected.Stations.Count;

                foreach (ApplicationBarIconButton button in ApplicationBar.Buttons)
                {
                    button.IsEnabled = false;
                }
                ApplicationBar.IsMenuEnabled = false;

                Downloader.LoadStations(StationDirectionRequestCallback, (Application.Current as App).Container);
            }
            else
            {
                stationsList.ItemsSource = _direction.Stations;
                txtInformation.Text = _direction.Name;
            }
        }

        private void ApplicationBarMenuItemAbout_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarMenuItemSettings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingPage.xaml", UriKind.Relative));
        }
    }
}