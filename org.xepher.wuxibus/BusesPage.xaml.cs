using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using org.xepher.common;
using org.xepher.control;
using org.xepher.lang;
using org.xepher.model;
using org.xepher.webservice;
using org.xepher.webservice.BusTravelGuideServiceReference;
using org.xepher.webservice.Model;
using org.xepher.wuxibus.misc;

namespace org.xepher.wuxibus
{
    public partial class BusesPage : PhoneApplicationPage
    {
        private Station Station { get; set; }
        WebServiceWrap warp = new WebServiceWrap();

        public BusesPage()
        {
            InitializeComponent();

            EventBind();

            InitializeUIComponent();

            GetBusALStationInfoCommon();
        }

        private void InitializeUIComponent()
        {
            Station = (Application.Current as App).SelectedStation;
        }

        private void EventBind()
        {
            warp.soapClient.getBusALStationInfoCommonCompleted += new System.EventHandler<webservice.BusTravelGuideServiceReference.getBusALStationInfoCommonCompletedEventArgs>(soapClient_getBusALStationInfoCommonCompleted);
        }

        private void soapClient_getBusALStationInfoCommonCompleted(object sender, getBusALStationInfoCommonCompletedEventArgs e)
        {
            GlobalLoading.Instance.IsLoading = false;
            if (e.Error == null)
            {
                IEnumerable<BusALStationInfoCommon> lstBusALStationInfoCommon =
                    from item in e.Result.Nodes[0].Descendants("Table1")
                    select new BusALStationInfoCommon()
                               {
                                   stationname = item.Element("stationname").Value,
                                   actdatetime = item.Element("actdatetime").Value,
                                   busselfid = item.Element("busselfid").Value,
                                   lastBus = item.Element("lastBus").Value,
                                   stationnum = item.Element("stationnum").Value
                               };

                foreach (BusALStationInfoCommon busAlStationInfoCommon in lstBusALStationInfoCommon)
                {
                    ContentPanel.Children.Add(new UC_BusInfo(new Bus()
                                                                 {
                                                                     ID = int.Parse(busAlStationInfoCommon.busselfid),
                                                                     Station = busAlStationInfoCommon.stationname,
                                                                     Time =
                                                                         DateTime.Parse(
                                                                             busAlStationInfoCommon.actdatetime),
                                                                     TTL = int.Parse(busAlStationInfoCommon.stationnum)
                                                                 }));
                }

                string fdisMsg = e.Result.Nodes[1].Value;
                if (!string.IsNullOrEmpty(fdisMsg)) MessageBox.Show(fdisMsg);
            }
        }

        private void GetBusALStationInfoCommon()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                GlobalLoading.Instance.IsLoading = true;
                string returnStr = "";
                warp.GetBusALStationInfoCommon(Station.line_id, Station.segment_id,
                                               Station.station_num > (Application.Current as App).StationSplitNumber
                                                   ? Station.station_num - (Application.Current as App).StationSplitNumber
                                                   : Station.station_num, returnStr);
            }
            else
                MessageBox.Show(AppResource.MsgNetworkUnavailable);
        }
    }
}