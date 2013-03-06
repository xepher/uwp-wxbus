using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using Microsoft.Phone.Controls;
using org.xepher.common;
using org.xepher.control;
using org.xepher.lang;
using org.xepher.model;
using org.xepher.wuxibus.misc;

namespace org.xepher.wuxibus
{
    public partial class MainPage : PhoneApplicationPage
    {
        private App _app;
        private BackgroundWorker _backroungWorker;
        private Popup _splashScreenPopup;
        private List<Line> _lstLine;
        private Popup _trafficInfoPopup = new Popup();
        private About _about = new About();

        public MainPage()
        {
            InitializeComponent();

            _app = (Application.Current as App);

            InitializeResources();

#if DEBUG
            Application.Current.Host.Settings.EnableFrameRateCounter = true;
#endif

#if !DEBUG
            _app.AdHelperInstance.InitializeAds(LayoutRoot, panoramaContainer);
#endif
        }

        private void InitializeResources()
        {
            _splashScreenPopup = new Popup()
                {
                    IsOpen = true,
                    Child = new UC_SplashScreen()
                        {
                            Text = AppResource.PopupLoadingText
                        }
                };

            _backroungWorker = new BackgroundWorker();
            RunBackgroundWorker();
        }

        private void RunBackgroundWorker()
        {
            _backroungWorker.DoWork += ((s, args) =>
            {
                Dispatcher.BeginInvoke(
                    () => { (_splashScreenPopup.Child as UC_SplashScreen).Text = AppResource.LoadingTextReleaseDB; });
                // 释放zip到IsolatedStorage
                IsolatedStorage.Zip2IS(ReleaseResource.GetResource("/org.xepher.wuxibus;component/Data/wuxitraffic.zip"));

                FetchLines();
            });

            _backroungWorker.RunWorkerCompleted += ((s, args) => Dispatcher.BeginInvoke(() =>
                {
                    InitializeUIComponent();

                    _splashScreenPopup.IsOpen = false;
                }));

            _backroungWorker.RunWorkerAsync();
        }

        private void _line_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            _app.SelectedLine = (sender as UC_Line).Line;
            NavigationService.Navigate(new Uri("/StationPage.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_trafficInfoPopup.IsOpen)
            {
                _trafficInfoPopup.IsOpen = false;
                e.Cancel = true;
                return;
            }
            if (!_about.AboutPrompt.IsOpen)
            {
                MessageBoxResult result = MessageBox.Show(AppResource.MsgExitApplication,
                                                          AppResource.TitleExitApplication,
                                                          MessageBoxButton.OKCancel);
                if (MessageBoxResult.Cancel == result)
                {
                    e.Cancel = true;
                }
            }
        }

        private void FetchLines()
        {
            try
            {
                Dispatcher.BeginInvoke(
                    () => { (_splashScreenPopup.Child as UC_SplashScreen).Text = AppResource.LoadingTextPrepareLines; });

                _lstLine = _app.Lines = _app.DAHelperInstance.GetAllLine();
            }
            catch (Exception)
            {
                Dispatcher.BeginInvoke(
                    () => { (_splashScreenPopup.Child as UC_SplashScreen).Text = AppResource.LoadingTextDBDestroyed; });

                _app.DAHelperInstance.DisposeConnection();

                // 如果数据库出错的话会在这里捕捉到异常
                // 这里需要将自带数据库释放
                IsolatedStorage.Zip2IS(
                    ReleaseResource.GetResource("/org.xepher.wuxibus;component/Data/wuxitraffic.zip"), true);

                _app.DAHelperInstance.OpenConnection();

                FetchLines();
            }
        }

        private void InitializeUIComponent()
        {
            foreach (Line line in _lstLine)
            {
                UC_Line _line = new UC_Line()
                {
                    Text = line.line_name,
                    Line = line
                };

                _line.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(_line_Tap);
                linesList.Items.Add(_line);
            }
        }

        private void FetchTrafficInfo()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://221.130.60.79:8080/Bus/sendinfo.action");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";

            request.BeginGetRequestStream(RequestCallback, request);
        }

        private void RequestCallback(IAsyncResult ar)
        {
            HttpWebRequest request = (HttpWebRequest)ar.AsyncState;

            StringBuilder sb = new StringBuilder();
            sb.Append("index=");
            sb.Append(HttpUtility.UrlEncode("0"));
            sb.Append("&");
            sb.Append("length=");
            sb.Append(HttpUtility.UrlEncode("all"));

            byte[] postData = Encoding.UTF8.GetBytes(sb.ToString());

            using (Stream postStream = request.EndGetRequestStream(ar))
            {
                postStream.Write(postData, 0, postData.Length);
            }
            request.BeginGetResponse(ResponseCallback, request);
        }

        private void ResponseCallback(IAsyncResult ar)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)ar.AsyncState;
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(ar);

                List<TrafficInfo> trafficInfos;

                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    trafficInfos = TrafficInfoHelper.ResolveReturnString(reader.ReadToEnd());
                }

                Dispatcher.BeginInvoke(() =>
                    {
                        foreach (TrafficInfo trafficInfo in trafficInfos)
                        {
                            UC_TrafficInfo _trafficInfo = new UC_TrafficInfo()
                                {
                                    Text = trafficInfo.Title,
                                    TrafficInfo = trafficInfo
                                };
                            _trafficInfo.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(_trafficInfo_Tap);

                            trafficInfoList.Items.Add(_trafficInfo);
                        }
                    });
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void _trafficInfo_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TrafficInfo trafficInfo = (sender as UC_TrafficInfo).TrafficInfo;

            _trafficInfoPopup.Child = new UC_TrafficDetailInfo()
                {
                    InfoTitle = trafficInfo.Title,
                    InfoContent = trafficInfo.Content,
                    InfoDate = trafficInfo.Date
                };

            _trafficInfoPopup.IsOpen = true;
        }

        private void BtnLocate_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/LocatePage.xaml", UriKind.Relative));
        }

        private void BtnSetting_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SettingPage.xaml", UriKind.Relative));
        }

        private void BtnAbout_OnClick(object sender, RoutedEventArgs e)
        {
            _about = new About();
            _about.Show();
        }

        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SearchPage.xaml", UriKind.Relative));
        }

        private void BtnChange_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void BtnLoadTrafficInfo_OnClick(object sender, RoutedEventArgs e)
        {
            //Dispatcher.BeginInvoke(
            //    () => { (_splashScreenPopup.Child as UC_SplashScreen).Text = "Fetch Traffic Information..."; });
            btnLoadTrafficInfo.Visibility = Visibility.Collapsed;
            FetchTrafficInfo();
        }
    }
}