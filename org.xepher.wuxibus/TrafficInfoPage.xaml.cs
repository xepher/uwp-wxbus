using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using org.xepher.control;
using org.xepher.model;
using org.xepher.wuxibus.misc;

namespace org.xepher.wuxibus
{
    public partial class TrafficInfoPage : PhoneApplicationPage
    {
        private Popup _trafficInfoPopup;
        private UC_TrafficDetailInfo _detailInfo;

        public TrafficInfoPage()
        {
            InitializeComponent();

            _trafficInfoPopup = new Popup();

            _detailInfo = new UC_TrafficDetailInfo();
            _detailInfo.Root.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x2C, 0x3E, 0x5B));
            _detailInfo.Border.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x9B, 0xE3));
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        #region 公交信息
        private void BtnLoadTrafficInfo_OnClick(object sender, RoutedEventArgs e)
        {
            btnLoadTrafficInfo.Visibility = Visibility.Collapsed;
            FetchTrafficInfo();
        }

        private void FetchTrafficInfo()
        {
            trafficInfoList.Items.Clear();

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
                    Brush borderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x9B, 0xE3));
                    Brush rootBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x80, 0xB8, 0xDB));
                    foreach (TrafficInfo trafficInfo in trafficInfos)
                    {
                        UC_TrafficInfo _trafficInfo = new UC_TrafficInfo();
                        _trafficInfo.Root.Background = rootBrush;
                        _trafficInfo.Border.Background = borderBrush;
                        _trafficInfo.Text = trafficInfo.Title;
                        _trafficInfo.TrafficInfo = trafficInfo;
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

            _detailInfo.InfoTitle = trafficInfo.Title;
            _detailInfo.InfoContent = trafficInfo.Content;
            _detailInfo.InfoDate = trafficInfo.Date;

            _trafficInfoPopup.Child = _detailInfo;
            _trafficInfoPopup.IsOpen = true;
        }
        #endregion

        private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
        {
            if (_trafficInfoPopup.IsOpen)
            {
                _trafficInfoPopup.IsOpen = false;
                e.Cancel = true;
            }
        }
    }
}