using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using Microsoft.Phone.Controls;
using org.xepher.common;
using org.xepher.control;
using org.xepher.lang;
using org.xepher.model;

namespace org.xepher.wuxibus
{
    public partial class BusesPage : PhoneApplicationPage
    {
        private Route Route { get; set; }
        private Station Station { get; set; }
        private Direction Direction { get; set; }
        private string _randomming;
        private int _judgeFlg = 1;

        public BusesPage()
        {
            InitializeComponent();

            Route = (Application.Current as App).SelectedRoute;
            Station = (Application.Current as App).SelectedStation;
            Direction = (Application.Current as App).SelectedDirection;

            if (Common.GetIsNetworkAvailable(AppResource.MsgNetworkUnavailable))
            {
                GlobalLoading.Instance.IsLoading = true;
                Downloader.GetRandomming(ResolveRandomming, (Application.Current as App).Container);
            }
        }

        private void ResolveRandomming(IAsyncResult ar)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest) ar.AsyncState;
                HttpWebResponse response = (HttpWebResponse) request.EndGetResponse(ar);

                _randomming = response.Cookies["CheckCode"].ToString().Substring(10);

                Dispatcher.BeginInvoke(
                    () => Downloader.LoadBuses(BusesRequestCallback, (Application.Current as App).Container));
            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke(() => MessageBox.Show(ex.Message));
            }
        }

        private void BusesRequestCallback(IAsyncResult ar)
        {
            HttpWebRequest request = (HttpWebRequest) ar.AsyncState;

            string formatString =
                "__EVENTTARGET=linkBtnQuery&__EVENTARGUMENT=&__LASTFOCUS=&__VIEWSTATE={0}&txtRandom={10}&ddlRoute={1}&ddlSegment={2}&hidSngserialIDValue={3}&hidSngserialIDValueList={4}&hidJudgeFlg={5}&hidsngserialID={6}&hiddualserialID={7}&hidX={8}&hidY={9}";

            if (_judgeFlg == 2)
            {
                formatString =
                    "__EVENTTARGET=&__EVENTARGUMENT=&__LASTFOCUS=&__VIEWSTATE={0}&txtRandom={10}&ddlRoute={1}&ddlSegment={2}&hidSngserialIDValue={3}&hidSngserialIDValueList={4}&hidJudgeFlg={5}&hidsngserialID={6}&hiddualserialID={7}&hidX={8}&hidY={9}";
            }

            string postString = string.Format(formatString, (Application.Current as App).ViewState, Route.Value,
                                              Direction.Value, Station.SngserialID + 1, string.Empty,
                                              _judgeFlg == 2 ? _judgeFlg : ++_judgeFlg,
                                              Station.SngserialID + 1, Station.DualserialID + 1, 0, 0,
                                              _randomming);

            // add rpt$ctl00$hidSngserialID,rpt$ctl00$hidDualserialID etc
            StringBuilder sb = new StringBuilder(postString);
            for (int i = 0; i < Direction.Stations.Count; i++)
            {
                sb.Append(string.Format("&rpt%24ctl{0}%24hidSngserialID={1}&rpt%24ctl{0}%24hidDualserialID={1}",
                                        i < 10 ? "0" + i.ToString() : i.ToString(), i + 1));

                if (Station.SngserialID == i)
                {
                    sb.Append(string.Format("&rpt%24ctl{0}%24imgBtn.x={1}&rpt%24ctl{0}%24imgBtn.y={1}",
                                            i < 10 ? "0" + i.ToString() : i.ToString(), 0));
                }
            }

            byte[] postData = Encoding.UTF8.GetBytes(sb.ToString());

            using (Stream postStream = request.EndGetRequestStream(ar))
            {
                postStream.Write(postData, 0, postData.Length);
            }
            request.BeginGetResponse(BusesResponseCallback, request);
        }

        private void BusesResponseCallback(IAsyncResult ar)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)ar.AsyncState;
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(ar);

                string result;

                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                    Dispatcher.BeginInvoke(() =>
                                               {
                                                   // viewstate save
                                                   (Application.Current as App).ViewState = Common.GetViewState(result);

                                                   string gvInfo = string.Empty;
                                                   List<Bus> buses = Common.ResolveBuses(result, ref gvInfo);
                                                   if (buses != null)
                                                   {
                                                       foreach (Bus bus in buses)
                                                       {
                                                           ContentPanel.Children.Add(new UC_BusInfo(bus));
                                                       }
                                                   }
                                                   else
                                                   {
                                                       MessageBox.Show(gvInfo);
                                                   }
                                               });
                }
            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke(() => MessageBox.Show(ex.Message));
            }
            finally
            {
                Dispatcher.BeginInvoke(() => GlobalLoading.Instance.IsLoading = false);
            }
        }
    }
}