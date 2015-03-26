using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.Xepher.Kazuma.Models;
using Org.Xepher.Kazuma.Utils;
using System.Reactive;
using Windows.UI.Xaml.Controls;
using System.Linq;
using ReactiveUI;

namespace Org.Xepher.Kazuma.ViewModels
{
    public class RouteViewModel : ViewModelBase
    {
        public RouteViewModel()
            : base()
        {
        }
        
        public string SelectedRouteFlag { get; set; }

        public string SelectedRouteId { get; set; }

        public string SelectedRouteName { get; set; }

        public int SelectedIndex { get; private set; }

        private ObservableCollection<Segment> _segments;
        public ObservableCollection<Segment> Segments
        {
            get
            {
                return _segments;
            }
            private set
            {
                _segments = value;
            }
        }

        private async Task InitSegments()
        {
            //if (GlobalLoading.Instance.IsLoading) return;
            //GlobalLoading.Instance.IsLoading = true;

            int retryCount = 0;
            ObservableCollection<Segment> result;
            do
            {
                string requestUrl =
                    SignatureUtil.GetRealRequestUrl(string.Format(Constants.TEMPLATE_SEGMENTS, Constants.SETTING_USER_ID,
                        Constants.BUS_LAT, Constants.BUS_LNG, Constants.DEVICE_TOKEN, Constants.BUS_API_KEY,
                        SignatureUtil.GenerateSeqId(), SelectedRouteId,
                        Constants.BUS_API_SECRET));

                result = await SignatureUtil.WebRequestAsync<ObservableCollection<Segment>>(requestUrl);
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

        public void SelectionChanged(int sender)
        {
            SelectedIndex = sender;
            // 1. Get Current Index
            // 2. Clear old value
            // 3. Get realtime information, should wait Setments are loaded
            Observable.StartAsync(GetRealTimeInfo);
        }

        private async Task GetRealTimeInfo()
        {
            // use MemoizingMRUCache to cache realtime info
            StationWithRealTimeInfo station =_segments[SelectedIndex].List.Last();

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
                        SelectedRouteId, Constants.BUS_API_SECRET, station.SegmentId,
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
            
            // clear old data
            foreach (var station2Item in Segments[SelectedIndex].List)
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
                foreach (var station2Item in Segments[SelectedIndex].List)
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
