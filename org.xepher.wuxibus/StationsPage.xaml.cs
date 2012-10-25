using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using org.xepher.common;
using org.xepher.model;

namespace org.xepher.wuxibus
{
    public partial class StationsPage : PhoneApplicationPage
    {
        public Route route { get; set; }

        public StationsPage()
        {
            InitializeComponent();
            LoadRouteInformation();
        }

        private void LoadRouteInformation()
        {
            route = (Application.Current as App).SelectedRoute;
            //// deep clone
            //route = new Route()
            //            {
            //                Name = (Application.Current as App).SelectedRoute.Name,
            //                Value = (Application.Current as App).SelectedRoute.Value,
            //                Stations = (Application.Current as App).SelectedRoute.Stations
            //            };
            //(Application.Current as App).SelectedRoute = null;
            PageTitle.Text = route.Name;

            // start loading 
            WebClient client = new WebClient();
            Uri uri = new Uri("http://218.90.160.85:9090/bustravelguide/default.aspx", UriKind.Absolute);

            client.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            string formatString = "__EVENTTARGET=&__EVENTARGUMENT=&__LASTFOCUS=&__VIEWSTATE={0}&txtRandom=&ddlRoute={1}&btnSegment.x=30&btnSegment.y=14&ddlSegment=33450414&hidSngserialIDValue=&hidSngserialIDValueList=&hidJudgeFlg=&hidsngserialID=&hiddualserialID=&hidX=&hidY=";
            string postString = string.Format(formatString, (Application.Current as App).ViewState, route.Value);

            client.UploadStringCompleted += new UploadStringCompletedEventHandler(StationsDownloaded);
            client.UploadStringAsync(uri, "POST", postString);
            GlobalLoading.Instance.IsLoading = true;
        }

        // 下载车站信息
        private void StationsDownloaded(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Result == null || e.Error != null)
            {
                MessageBox.Show("There was an error downloading the Stations!");
            }
            else
            {
                // Resolve Stations
                List<Station> stations = ResolveStations(e.Result);
                stationsList.ItemsSource = stations;
            }
            GlobalLoading.Instance.IsLoading = false;
        }

        // 解析页面获得车站信息
        public List<Station> ResolveStations(string rawhtml)
        {
            string pattern = "<span id=\"rpt_ctl[0-9][0-9]_lblStation\">([^<]*)</span>";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            MatchCollection collection = regex.Matches(rawhtml);

            List<Station> stations = new List<Station>();

            int index = 0;
            foreach (Match match in collection)
            {
                index++;
                Station station = new Station()
                                      {
                                          Name = match.Groups[1].Value,
                                          Value = index
                                      };
                stations.Add(station);
            }

            route.Stations = stations;
            // save to isolatedstorage

            return stations;
        }

        private void stationsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //WebClient client = new WebClient();
            //Uri uri = new Uri("http://218.90.160.85:9090/bustravelguide/default.aspx", UriKind.Absolute);

            //client.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            //StringBuilder formatBuilder = new StringBuilder("__EVENTTARGET=&__EVENTARGUMENT=&__LASTFOCUS=&__VIEWSTATE={0}&txtRandom=&ddlRoute={1}&ddlSegment=33450414&hidSngserialIDValue=&hidSngserialIDValueList=&hidJudgeFlg=&hidsngserialID=&hiddualserialID=&hidX=203&hidY=119");
            //string value;
            //for (int index = 0; index < route.Stations.Count; index++)
            //{
            //    value = index < 10 ? "0" + index.ToString() : index.ToString();
            //    formatBuilder.Append("rpt$ctl" + value + "$hidSngserialID=" + (index + 1).ToString() + "&");
            //    formatBuilder.Append("rpt$ctl" + value + "$hidDualserialID=" + (index + 1).ToString() + "&");
            //}
            //int selectIndex = (stationsList.SelectedItem as Station).Value;
            //value = selectIndex < 10 ? "0" + selectIndex.ToString() : selectIndex.ToString();
            //formatBuilder.Append("rpt$ctl" + value + "$imgBtn.x=6" + "rpt$ctl" + value + "$imgBtn.y=13");
            //string postString = string.Format(formatBuilder.ToString(), (Application.Current as App).ViewState, route.Value);

            //client.UploadStringCompleted += new UploadStringCompletedEventHandler(BusInformationDownloaded);
            //client.UploadStringAsync(uri, "POST", postString);
            //GlobalLoading.Instance.IsLoading = true;
        }

        // 下载车辆信息
        private void BusInformationDownloaded(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Result == null || e.Error != null)
            {
                MessageBox.Show("There was an error downloading the Bus Information!");
            }
            else
            {
                // Resolve Busses
                List<Bus> busses= ResolveBusses(e.Result);
            }
            GlobalLoading.Instance.IsLoading = false;
        }

        // 解析页面获得车辆信息
        public List<Bus> ResolveBusses(string rawhtml)
        {
            return null;
        }
    }
}