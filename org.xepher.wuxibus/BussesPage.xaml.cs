using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using Microsoft.Phone.Controls;
using org.xepher.common;
using org.xepher.model;

namespace org.xepher.wuxibus
{
    public partial class BussesPage : PhoneApplicationPage
    {
        private Route Route { get; set; }
        private Station Station { get; set; }
        private Direction Direction { get; set; }
        private List<Bus> Busses { get; set; }
        private List<string> _paramsList;
        private string _randomming;
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public BussesPage()
        {
            InitializeComponent();

            Route = (Application.Current as App).SelectedRoute;
            Station = (Application.Current as App).SelectedStation;
            Direction = (Application.Current as App).SelectedDirection;

            Downloader.LoadBusses(BussesPreRequestCallback, (Application.Current as App).Container);
        }

        private void BussesPreRequestCallback(IAsyncResult ar)
        {
            HttpWebRequest preRequest = (HttpWebRequest)ar.AsyncState;

            const string formatString = "__EVENTTARGET=&__EVENTARGUMENT=&__LASTFOCUS=&__VIEWSTATE={0}&txtRandom=&ddlRoute={1}&ddlSegment={2}&hidSngserialIDValue=&hidSngserialIDValueList=&hidJudgeFlg=&hidsngserialID=&hiddualserialID=&hidX=89&hidY=113";
            string postString = string.Format(formatString, (Application.Current as App).ViewState, Route.Value,
                                              Direction.Value);

            // add rpt$ctl00$hidSngserialID,rpt$ctl00$hidDualserialID etc
            StringBuilder sb = new StringBuilder(postString);
            for (int i = 0; i < Direction.Stations.Count; i++)
            {
                sb.Append(string.Format("&rpt$ctl{0}$hidSngserialID={1}&rpt$ctl{0}$hidDualserialID={1}",
                                        i < 10 ? "0" + i.ToString() : i.ToString(), i + 1));
            }

            byte[] postData = Encoding.UTF8.GetBytes(sb.ToString());

            using (Stream postStream = preRequest.EndGetRequestStream(ar))
            {
                postStream.Write(postData, 0, postData.Length);
            }
            preRequest.BeginGetResponse(BussesPreResponseCallback, preRequest);
        }

        private void BussesPreResponseCallback(IAsyncResult ar)
        {
            HttpWebRequest preRequest = (HttpWebRequest)ar.AsyncState;
            HttpWebResponse preResponse = (HttpWebResponse)preRequest.EndGetResponse(ar);

            Downloader.GetRandomming(iar =>
                                         {
                                             HttpWebRequest request = (HttpWebRequest)iar.AsyncState;
                                             HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(iar);

                                             _randomming = response.Cookies["CheckCode"].ToString();
                                         }, (Application.Current as App).Container);

            string result;

            using (StreamReader reader = new StreamReader(preResponse.GetResponseStream(), Encoding.UTF8))
            {
                result = reader.ReadToEnd();

                // viewstate save
                (Application.Current as App).ViewState = Common.GetViewState(result);

                _paramsList = Common.GetParamsList(result);

                Dispatcher.BeginInvoke(() =>
                                           {
                                               StringBuilder sb = new StringBuilder();
                                               _paramsList.ForEach(p => sb.Append(p).Append("|"));

                                               txtData.Text = sb.ToString();

                                               GlobalLoading.Instance.IsLoading = false;
                                           });
            }
        }

        private void BussesRequestCallback(IAsyncResult ar)
        {
            HttpWebRequest request = (HttpWebRequest)ar.AsyncState;

            const string formatString = "__EVENTTARGET=linkBtnQuery&__EVENTARGUMENT=&__LASTFOCUS=&__VIEWSTATE={0}&txtRandom={10}&ddlRoute={1}&ddlSegment={2}&hidSngserialIDValue={3}&hidSngserialIDValueList={4}&hidJudgeFlg={5}&hidsngserialID={6}&hiddualserialID={7}&hidX={8}&hidY={9}";
            string postString = string.Format(formatString, (Application.Current as App).ViewState, Route.Value,
                                              Direction.Value, _paramsList[0], _paramsList[1], _paramsList[2],
                                              _paramsList[3], _paramsList[4], _paramsList[5], _paramsList[6],
                                              _randomming);

            // add rpt$ctl00$hidSngserialID,rpt$ctl00$hidDualserialID etc
            StringBuilder sb = new StringBuilder(postString);
            for (int i = 0; i < Direction.Stations.Count; i++)
            {
                sb.Append(string.Format("&rpt$ctl{0}$hidSngserialID={1}&rpt$ctl{0}$hidDualserialID={1}",
                                        i < 10 ? "0" + i.ToString() : i.ToString(), i + 1));
            }

            byte[] postData = Encoding.UTF8.GetBytes(postString);

            using (Stream postStream = request.EndGetRequestStream(ar))
            {
                postStream.Write(postData, 0, postData.Length);
            }
            request.BeginGetResponse(BussesResponseCallback, request);
        }

        private void BussesResponseCallback(IAsyncResult ar)
        {
            HttpWebRequest request = (HttpWebRequest)ar.AsyncState;
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(ar);

            Downloader.GetRandomming(iar => { }, (Application.Current as App).Container);

            string result;

            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                result = reader.ReadToEnd();
                Dispatcher.BeginInvoke(() =>
                                           {
                                               // viewstate save
                                               (Application.Current as App).ViewState = Common.GetViewState(result);

                                               List<Bus> busses = Common.ResolveBusses(result);
                                               MessageBox.Show("Bus ID: " + busses[0].ID + " Station: " +
                                                               busses[0].Station + " Time: " + busses[0].Time + " TTL: " +
                                                               busses[0].TTL);
                                           });
            }
        }

        private void btnPOST_Click(object sender, RoutedEventArgs e)
        {
            Downloader.LoadBusses(BussesRequestCallback, (Application.Current as App).Container);
        }

        private void btnShowData_Click(object sender, RoutedEventArgs e)
        {
            txtData.Text = _randomming;
        }
    }
}