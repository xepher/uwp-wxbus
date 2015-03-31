using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Org.Xepher.Kazuma.Models;
using Org.Xepher.Kazuma.Utils;
using System.Linq;
using Windows.Storage;
using ReactiveUI;
using Splat;

namespace Org.Xepher.Kazuma.ViewModels
{
    public class RouteViewModel : ViewModelBase
    {
        public RouteViewModel(IScreen screen, Route selectedRoute)
            : base(screen)
        {
            base.PathSegment = "Route";
            this.SelectedRoute = selectedRoute;

            #region Query Configuration

            //this.WhenAnyValue(vm => vm.SelectedSegmentIndex, vm => vm.Segments, vm => vm.IsBusy, (x, y, z) => y.Count > 0 && !z)
            //    .Subscribe(async _ => await GetRealTimeInfo());

            #endregion Query Configuration

            #region Refresh Data Configuration

            RefreshCommand = ReactiveCommand.Create(this.WhenAny(vm => vm.IsBusy, r => !r.Value));

            RefreshCommand.Subscribe(async _ =>
            {
                Segments.Clear();
                await RequestInternetData();
            });

            SearchCommand = ReactiveCommand.Create(this.WhenAny(vm => vm.IsBusy, r => !r.Value));

            SearchCommand.Subscribe(async _ =>
            {
                await GetRealTimeInfo();
            });

            #endregion Refresh Data Configuration
            
            if (ApplicationDataSettingsHelper.ReadValue<bool>("IsLocalStorageOn"))
            {
                Observable.StartAsync(RequestLocalData);
            }
            else
            {
                Observable.StartAsync(RequestInternetData);
            }
        }

        public Route SelectedRoute { get; private set; }

        private int _selectedSegmentIndex;
        public int SelectedSegmentIndex
        {
            get { return _selectedSegmentIndex; }
            set { this.RaiseAndSetIfChanged(ref _selectedSegmentIndex, value); }
        }

        private IList<Segment> _segments;
        public IList<Segment> Segments
        {
            get { return _segments; }
            set { this.RaiseAndSetIfChanged(ref _segments, value); }
        }

        private async Task RequestLocalData()
        {
#if DEBUG
            this.Log().Debug("Load Segments via LocalFolder");
#endif
            IsBusy = true;

            Segments =
                await
                    StorageHelper.ReadData<ObservableCollection<Segment>>(ApplicationData.Current.LocalFolder,
                        String.Format("{0}.data", SelectedRoute.RouteId));

            if (null == Segments || Segments.Count == 0)
            {
                await RequestInternetData();
            }

            IsBusy = false;
        }

        private async Task RequestInternetData()
        {
#if DEBUG
            this.Log().Debug("Load Segments via internet");
#endif
            IsBusy = true;

            int retryCount = 0;
            do
            {
                string requestUrl =
                    SignatureUtil.GetRealRequestUrl(string.Format(Constants.TEMPLATE_SEGMENTS,
                        Constants.SETTING_USER_ID,
                        Constants.BUS_LAT, Constants.BUS_LNG, Constants.DEVICE_TOKEN, Constants.BUS_API_KEY,
                        SignatureUtil.GenerateSeqId(), SelectedRoute.RouteId,
                        Constants.BUS_API_SECRET));

                Segments = await SignatureUtil.WebRequestAsync<ObservableCollection<Segment>>(requestUrl);
                if (++retryCount > 10) break;
            } while (Segments == null || Segments.Count == 0);

            if (retryCount > 10)
            {
                //GlobalLoading.Instance.IsLoading = false;
                //MessageBox.Show("网络异常，请稍后再试！");
                return;
            }

            StorageHelper.WriteData(ApplicationData.Current.LocalFolder, String.Format("{0}.data", SelectedRoute.RouteId), Segments);

            IsBusy = false;
        }

        private async Task GetRealTimeInfo()
        {
            IsBusy = true;

            // use MemoizingMRUCache to cache realtime info
            StationWithRealTimeInfo station = _segments[SelectedSegmentIndex].List.Last();

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

            // clear old data
            foreach (var station2Item in Segments[SelectedSegmentIndex].List)
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
                foreach (var station2Item in Segments[SelectedSegmentIndex].List)
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
            
            IsBusy = false;
        }

        #region Refresh Data

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { this.RaiseAndSetIfChanged(ref _isBusy, value); }
        }

        public ReactiveCommand<object> RefreshCommand { get; protected set; }

        public ReactiveCommand<object> SearchCommand { get; protected set; }

        #endregion Refresh Data
    }
}
