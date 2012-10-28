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

        private Direction _direction;

        public StationsPage()
        {
            InitializeComponent();

            Route = (Application.Current as App).SelectedRoute;
            PageTitle.Text = Route.Name;

            Object obj = IsolatedStorage.ReadFromFile(string.Format("Data\\{0}.data", Route.Value), typeof(List<Direction>));

            if (obj == null)
            {
                LoadStations();
            }
            else
            {
                Directions = (List<Direction>)obj;
                stationsList.ItemsSource = Directions.First(d => d.IsSelected).Stations;
                if (!_isAddedDirectionButton)
                {
                    if (Directions.Count > 1)
                    {
                        ApplicationBarIconButton button = new ApplicationBarIconButton()
                                                              {
                                                                  IconUri =
                                                                      new Uri(
                                                                      "Assets/Icons/dark/appbar.next.rest.png",
                                                                      UriKind.Relative),
                                                                  IsEnabled = true,
                                                                  Text = "Direction"
                                                              };

                        button.Click += new EventHandler(ApplicationBarIconButtonDirection_Click);

                        ApplicationBar.Buttons.Add(button);
                        _isAddedDirectionButton = true;
                    }
                }
                txtInformation.Text = Directions.First(d => d.IsSelected).Name;
            }
        }

        private void LoadStations()
        {
            // GET /bustravelguide/ for all routes
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://218.90.160.85:9090/bustravelguide/default.aspx");
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:16.0) Gecko/20100101 Firefox/16.0 (wbs wp 1.0)";
            request.Headers["Accept-Encoding"] = "gzip, deflate";
            request.Headers["Accept-Language"] = "zh-CN";
            request.Headers["Referer"] = "http://218.90.160.85:9090/bustravelguide/default.aspx";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            request.CookieContainer = (Application.Current as App).Container;

            request.BeginGetRequestStream(StationsRequestCallback, request);

            GlobalLoading.Instance.IsLoading = true;
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

            // GET randomming.aspx for Session and Cookies
            HttpWebRequest requestRandomming = (HttpWebRequest)WebRequest.Create("http://218.90.160.85:9090/bustravelguide/randomming.aspx");
            requestRandomming.Accept = "image/png, image/svg+xml, image/*;q=0.8, */*;q=0.5";
            requestRandomming.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:16.0) Gecko/20100101 Firefox/16.0 (wbs wp 1.0)";
            request.Headers["Accept-Encoding"] = "gzip, deflate";
            request.Headers["Accept-Language"] = "zh-CN";
            request.Headers["Referer"] = "http://218.90.160.85:9090/bustravelguide/default.aspx";
            requestRandomming.CookieContainer = (Application.Current as App).Container;

            requestRandomming.BeginGetResponse(iar => { }, requestRandomming);

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
                                               if (Directions == null)
                                               {
                                                   // first load the directions
                                                   Directions = Common.ResolveStations(result);
                                               }
                                               else
                                               {
                                                   // switch the direction
                                                   // save the download stations to current direction
                                                   Direction newDirection = Common.ResolveStations(result).First(d => d.IsSelected);
                                                   Directions.First(d => d.Value == newDirection.Value).Stations = newDirection.Stations;
                                               }

                                               stationsList.ItemsSource = Directions.First(d => d.IsSelected).Stations;

                                               if (!_isAddedDirectionButton)
                                               {
                                                   if (Directions.Count > 1)
                                                   {
                                                       ApplicationBarIconButton button = new ApplicationBarIconButton()
                                                                                             {
                                                                                                 IconUri =
                                                                                                     new Uri(
                                                                                                     "Assets/Icons/dark/appbar.next.rest.png",
                                                                                                     UriKind.Relative),
                                                                                                 IsEnabled = true,
                                                                                                 Text = "Direction"
                                                                                             };

                                                       button.Click += new EventHandler(ApplicationBarIconButtonDirection_Click);

                                                       ApplicationBar.Buttons.Add(button);
                                                       _isAddedDirectionButton = true;
                                                   }
                                               }
                                               txtInformation.Text = Directions.First(d => d.IsSelected).Name;

                                               stationsList.SelectedIndex = -1;
                                               _isListBoxDataBinded = false;

                                               if (Directions.First(d => d.IsSelected).Stations == null)
                                               {
                                                   // todo: Async save Stations information
                                                   IsolatedStorage.SaveToFile(Directions,
                                                                              string.Format("Data\\{0}.data",
                                                                                            Route.Value));
                                               }

                                               GlobalLoading.Instance.IsLoading = false;
                                           });
            }
        }

        private void LoadStations(int stationCount)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://218.90.160.85:9090/bustravelguide/default.aspx");
            request.Accept = "text/html, application/xhtml+xml, */*";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:16.0) Gecko/20100101 Firefox/16.0 (wbs wp 1.0)";
            request.Headers["Accept-Encoding"] = "gzip, deflate";
            request.Headers["Accept-Language"] = "zh-CN";
            request.Headers["Referer"] = "http://218.90.160.85:9090/bustravelguide/default.aspx";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            request.CookieContainer = (Application.Current as App).Container;

            _stationCount = stationCount;

            request.BeginGetRequestStream(StationDirectionRequestCallback, request);

            GlobalLoading.Instance.IsLoading = true;
        }

        private void StationDirectionRequestCallback(IAsyncResult ar)
        {
            HttpWebRequest request = (HttpWebRequest)ar.AsyncState;

            const string formatString = "__EVENTTARGET=ddlSegment&__EVENTARGUMENT=&__LASTFOCUS=&__VIEWSTATE={0}&txtRandom=&ddlRoute={1}&ddlSegment={2}";
            string postString = string.Format(formatString, (Application.Current as App).ViewState, Route.Value,
                                              _direction.Value);

            // add rpt$ctl00$hidSngserialID,rpt$ctl00$hidDualserialID etc
            StringBuilder sb = new StringBuilder(postString);
            for (int i = 0; i < _stationCount; i++)
            {
                sb.Append(string.Format("&rpt$ctl{0}$hidSngserialID={1}&rpt$ctl{0}$hidDualserialID={1}",
                                        i < 10 ? "0" + i.ToString() : i.ToString(), i + 1));
            }

            sb.Append(
                "&hidSngserialIDValue=&hidSngserialIDValueList=&hidJudgeFlg=&hidsngserialID=&hiddualserialID=&hidX=&hidY=");

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

            // GET randomming.aspx for Session and Cookies
            HttpWebRequest requestRandomming = (HttpWebRequest)WebRequest.Create("http://218.90.160.85:9090/bustravelguide/randomming.aspx");
            requestRandomming.Accept = "image/png, image/svg+xml, image/*;q=0.8, */*;q=0.5";
            requestRandomming.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:16.0) Gecko/20100101 Firefox/16.0 (wbs wp 1.0)";
            request.Headers["Accept-Encoding"] = "gzip, deflate";
            request.Headers["Accept-Language"] = "zh-CN";
            request.Headers["Referer"] = "http://218.90.160.85:9090/bustravelguide/default.aspx";
            requestRandomming.CookieContainer = (Application.Current as App).Container;

            requestRandomming.BeginGetResponse(iar => { }, requestRandomming);

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
                                               // todo: can't resolve other direction
                                               if (Directions == null)
                                               {
                                                   // first load the directions
                                                   Directions = Common.ResolveStations(result);
                                               }
                                               else
                                               {
                                                   // switch the direction
                                                   // save the download stations to current direction
                                                   Direction newDirection = Common.ResolveStations(result).First(d => d.IsSelected);
                                                   Directions.First(d => d.Value == newDirection.Value).Stations = newDirection.Stations;
                                               }

                                               stationsList.ItemsSource = Directions.First(d => d.IsSelected).Stations;

                                               if (!_isAddedDirectionButton)
                                               {
                                                   if (Directions.Count > 1)
                                                   {
                                                       ApplicationBarIconButton button = new ApplicationBarIconButton()
                                                       {
                                                           IconUri =
                                                               new Uri(
                                                               "Assets/Icons/dark/appbar.refresh.rest.png",
                                                               UriKind.Relative),
                                                           IsEnabled = true,
                                                           Text = "Direction"
                                                       };

                                                       button.Click += new EventHandler(ApplicationBarIconButtonDirection_Click);

                                                       ApplicationBar.Buttons.Add(button);
                                                       _isAddedDirectionButton = true;
                                                   }
                                               }
                                               txtInformation.Text = Directions.First(d => d.IsSelected).Name;

                                               stationsList.SelectedIndex = -1;
                                               _isListBoxDataBinded = false;

                                               if (Directions.First(d => d.IsSelected).Stations == null)
                                               {
                                                   // todo: Async save Stations information
                                                   IsolatedStorage.SaveToFile(Directions,
                                                                              string.Format("Data\\{0}.data",
                                                                                            Route.Value));
                                               }

                                               GlobalLoading.Instance.IsLoading = false;
                                           });
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
                LoadStations(direSelected.StationsCount);
            }
            else
            {
                stationsList.ItemsSource = _direction.Stations;
                txtInformation.Text = _direction.Name;
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
            if (MessageBoxResult.Cancel ==
                MessageBox.Show(string.Format("Are you want to refresh route {0}?", Route.Name), "Refresh Stations", MessageBoxButton.OKCancel))
            {
                return;
            }
            LoadStations();
        }
    }
}