using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using org.xepher.common;
using org.xepher.control;
using org.xepher.lang;
using org.xepher.model;
using org.xepher.webservice;
using org.xepher.webservice.BusTravelGuideServiceReference;

namespace org.xepher.wuxibus
{
    public partial class BusPage : PhoneApplicationPage
    {
        private Station Station { get; set; }
        WebServiceWrapper warp = new WebServiceWrapper();

        public BusPage()
        {
            InitializeComponent();

            EventBind();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            InitializeUIComponent();

            GetBusALStationInfoCommon();

            base.OnNavigatedTo(e);
        }

        private void EventBind()
        {
            warp.soapClient.getBusALStationInfoCommonCompleted += soapClient_getBusALStationInfoCommonAsyncCompleted;
        }

        private void InitializeUIComponent()
        {
            Station = (Application.Current as App).SelectedStation;
            PageTitle.Text = Station.station_name;
        }

        private void soapClient_getBusALStationInfoCommonAsyncCompleted(object sender, getBusALStationInfoCommonCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                IEnumerable<BusALStationInfoCommon> lstBusALStationInfoCommon =
                    from item in e.Result.Nodes[0].Descendants("Table1")
                    select new BusALStationInfoCommon()
                               {
                                   stationname = item.Element("stationname").Value,
                                   actdatetime = DateTime.Parse(item.Element("actdatetime").Value),
                                   busselfid = int.Parse(item.Element("busselfid").Value),
                                   lastBus = item.Element("lastBus").Value,
                                   stationnum = int.Parse(item.Element("stationnum").Value)
                               };

                foreach (BusALStationInfoCommon busAlStationInfoCommon in lstBusALStationInfoCommon)
                {
                    busList.Items.Add(new UC_BusInfo(busAlStationInfoCommon));
                }

                string fdisMsg = e.Result.Nodes[1].Value;
                if (!string.IsNullOrEmpty(fdisMsg))
                {
                    GlobalLoading.Instance.IsLoading = false;
                    SystemTray.IsVisible = false;
                    if (MessageBoxResult.OK == MessageBox.Show(fdisMsg))
                    {
                        NavigationService.GoBack();
                    }
                }
            }
            GlobalLoading.Instance.IsLoading = false;
            SystemTray.IsVisible = false;
        }

        private void GetBusALStationInfoCommon()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                SystemTray.IsVisible = true;
                GlobalLoading.Instance.IsLoading = true;
                string returnStr = "";
                try
                {
                    warp.GetBusALStationInfoCommonAsync(Station.line_id, Station.segment_id, Station.station_num,
                                                        returnStr);
                }
                //catch (System.Reflection.TargetInvocationException ex)
                //{
                //    // TODO: system.reflection.targetinvocationexception 会抛出
                //    throw;
                //}
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                if (MessageBoxResult.OK == MessageBox.Show(AppResource.MsgNetworkUnavailable))
                {
                    NavigationService.GoBack();
                }
            }
        }
    }
}