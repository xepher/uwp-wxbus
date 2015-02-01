using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Org.Xepher.Kazuma.Models;
using Org.Xepher.Kazuma.Utils;
using System.Reactive;

namespace Org.Xepher.Kazuma.ViewModels
{
    public class RouteViewModel : ViewModelBase
    {
        public RouteViewModel(INavigationService navigationService)
            : base(navigationService)
        {
        }

        protected override void OnInitialize()
        {
            this.SelectedRoute = new Route() { Flag = SelectedRouteFlag, RouteId = SelectedRouteId, RouteName = SelectedRouteName };

            Observable.StartAsync(InitSegments);

            base.OnInitialize();
        }

        public Route SelectedRoute { get; private set; }

        public string SelectedRouteFlag { get; set; }

        public string SelectedRouteId { get; set; }

        public string SelectedRouteName { get; set; }

        private BindableCollection<Segment> _segments;
        public BindableCollection<Segment> Segments {
            get
            {
                return _segments;
            }
            private set
            {
                _segments = value;
                base.NotifyOfPropertyChange(() => Segments);
            }
        }

        private async Task InitSegments()
        {
            //if (GlobalLoading.Instance.IsLoading) return;
            //GlobalLoading.Instance.IsLoading = true;

            int retryCount = 0;
            BindableCollection<Segment> result;
            do
            {
                string requestUrl =
                    SignatureUtil.GetRealRequestUrl(string.Format(Constants.TEMPLATE_SEGMENTS, Constants.SETTING_USER_ID,
                        Constants.BUS_LAT, Constants.BUS_LNG, Constants.DEVICE_TOKEN, Constants.BUS_API_KEY,
                        SignatureUtil.GenerateSeqId(), SelectedRoute.RouteId,
                        Constants.BUS_API_SECRET));

                result = await SignatureUtil.WebRequestAsync<BindableCollection<Segment>>(requestUrl);
                if (++retryCount > 10) break;
            } while (result == null || result.Count == 0);

            if (retryCount > 10)
            {
                //GlobalLoading.Instance.IsLoading = false;
                //MessageBox.Show("网络异常，请稍后再试！");
                return;
            }

            Segments = result;

            //GlobalLoading.Instance.IsLoading = false;
        }

        public void SelectionChanged(StationWithRealTimeInfo sender)
        {
            DebugLog logger = new DebugLog(typeof(string));
            logger.Info(sender.ToString());
            
            // use this sender as parameter to call GetRealTimeInfo function
        }

        private async Task GetRealTimeInfo(StationWithRealTimeInfo station)
        {
            // if search is in-process, stop search this time
            //if (GlobalLoading.Instance.IsLoading) return;
            //GlobalLoading.Instance.IsLoading = true;

            int retryCount = 0;
            RealTimeBusData result;
            do
            {
                string requestUrl =
                    SignatureUtil.GetRealRequestUrl(string.Format(Constants.TEMPLATE_REALTIME_INFO,
                        Constants.SETTING_USER_ID, Constants.BUS_LAT, Constants.BUS_LNG, Constants.DEVICE_TOKEN,
                        Constants.BUS_API_KEY, SignatureUtil.GenerateSeqId(),
                        SelectedRoute.RouteId, Constants.BUS_API_SECRET, station.SegmentId,
                        station.StationId.Length > 10 ? station.StationSeq : station.StationId));

                result = await SignatureUtil.WebRequestAsync<RealTimeBusData>(requestUrl);
                if (++retryCount > 10) break;
            } while (null == result.Message);

            if (retryCount > 10)
            {
                //GlobalLoading.Instance.IsLoading = false;
                //MessageBox.Show("网络异常，请稍后再试！");
                return;
            }

            RealTimeBusData realTimeInfo = result;

            if (!string.IsNullOrEmpty(realTimeInfo.Message))
            {
                //GlobalLoading.Instance.IsLoading = false;
                //MessageBox.Show(realTimeInfo.Message);
                return;
            }

            // confirm which direction
            int indexList = 0;

            if (Segments.Count == 2)
            {
                if (string.IsNullOrEmpty(realTimeInfo.Result[0].CurStopNo))
                {
                    // for wuxibus and xihuibus
                    // circle line, with same SegmentId for two direction
                    if (Segments[0].List[0].SegmentId == Segments[1].List[0].SegmentId)
                    {
                        if (int.Parse(station.StationSeq) > int.Parse(
                            Segments[0].List.Find(s => int.Parse(s.StationSeq) == Segments[0].List.Count).StationSeq))
                        {
                            indexList = 1;
                        }
                    }
                    else
                    {
                        // two one-way lines, with different SegmentsId
                        if (station.SegmentId == Segments[1].List[0].SegmentId)
                        {
                            indexList = 1;
                        }
                    }
                }
                else
                {
                    // for xinqubus
                    if (station.SegmentId == Segments[1].List[0].SegmentId)
                    {
                        indexList = 1;
                    }
                }
            }

            // clear old data
            foreach (var station2Item in Segments[indexList].List)
            {
                if (null != station2Item.BusselfId)
                {
                    station2Item.ActDateTime = new DateTime();
                    station2Item.BusselfId = null;
                    station2Item.FlagTitle = null;
                }
            }

            // write realtime info
            foreach (var realTimeInfoItem in realTimeInfo.Result)
            {
                foreach (var station2Item in Segments[indexList].List)
                {
                    if (string.IsNullOrEmpty(realTimeInfoItem.CurStopNo))
                    {
                        // for wuxibus and xihuibus
                        if ((int.Parse(station2Item.StationSeq) ==
                             (int.Parse(station.StationSeq) - int.Parse(realTimeInfoItem.StationNum))))
                        {
                            station2Item.ActDateTime = realTimeInfoItem.ActDateTime;
                            station2Item.BusselfId = realTimeInfoItem.BusselfId;
                            station2Item.FlagTitle = realTimeInfoItem.Flag_Title;
                        }
                    }
                    else
                    {
                        // for xinqubus
                        if (int.Parse(station2Item.StationSeq) == int.Parse(realTimeInfoItem.CurStopNo))
                        {
                            station2Item.ActDateTime = realTimeInfoItem.ActDateTime;
                            station2Item.BusselfId = realTimeInfoItem.BusselfId;
                            station2Item.FlagTitle = realTimeInfoItem.Flag_Title;
                        }
                    }
                }
            }

            //GlobalLoading.Instance.IsLoading = false;
        }
    }
}
