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
using Windows.UI.StartScreen;
using Windows.UI.Xaml;

namespace Org.Xepher.Kazuma.ViewModels
{
    public class RouteViewModel : ViewModelBase
    {
        public RouteViewModel(IScreen screen, IMessageBus messageBus, Route selectedRoute)
            : base(screen, messageBus)
        {
            base.PathSegment = "Route";
            this.SelectedRoute = selectedRoute;

            #region Query Configuration

            //this.WhenAnyValue(vm => vm.SelectedSegmentIndex, vm => vm.Segments, vm => vm.IsBusy, (x, y, z) => y.Count > 0 && !z)
            //    .Subscribe(async _ => await GetRealTimeInfo());

            #endregion Query Configuration

            #region ApplicationBar Configuration

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

            PinCommand = ReactiveCommand.Create(this.WhenAny(vm => vm.IsBusy, r => !r.Value));

            PinCommand.Subscribe(async _ =>
            {
                await PinToScreen();
            });

            #endregion ApplicationBar Configuration

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

        private StationWithRealTimeInfo _selectedSegmentListItem;
        public StationWithRealTimeInfo SelectedSegmentListItem
        {
            get { return _selectedSegmentListItem; }
            set { this.RaiseAndSetIfChanged(ref _selectedSegmentListItem, value); }
        }

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
                if (retryCount > 1) base.HostMessageBus.SendMessage<string>(string.Format("获取数据失败，第{0}次尝试", retryCount - 1), "TopBarMessage");
            } while (Segments == null || Segments.Count == 0);

            if (retryCount > 10)
            {
                base.HostMessageBus.SendMessage<string>("网络异常，请稍后再试", "TopBarMessage");

                IsBusy = false;
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
                if (retryCount > 1) base.HostMessageBus.SendMessage<string>(string.Format("获取数据失败，第{0}次尝试", retryCount - 1), "TopBarMessage");
            } while (null == result.Message);

            if (retryCount > 10)
            {
                base.HostMessageBus.SendMessage<string>("网络异常，请稍后再试", "TopBarMessage");

                IsBusy = false;
                return;
            }

            RealTimeBusData realTimeInfo = result;

            if (!string.IsNullOrEmpty(realTimeInfo.Message))
            {
                base.HostMessageBus.SendMessage<string>(realTimeInfo.Message, "TopBarMessage");

                IsBusy = false;
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

        private async Task PinToScreen()
        {
            // Let us first verify if we need to pin or unpin
            if (SecondaryTile.Exists(Constants.APPBAR_TILE_ID + "." + SelectedRoute.RouteId))
            {
                // First prepare the tile to be unpinned
                SecondaryTile secondaryTile = new SecondaryTile(Constants.APPBAR_TILE_ID + "." + SelectedRoute.RouteId);

                // Now make the delete request.
                //bool isUnpinned = await secondaryTile.RequestDeleteForSelectionAsync(SecondaryTileHelper.GetElementRect((FrameworkElement)sender), Windows.UI.Popups.Placement.Above);
                bool isUnpinned = false;
                if (isUnpinned)
                {
                    //rootPage.NotifyUser(MainPage.appbarTileId + " unpinned.", NotifyType.StatusMessage);
                }
                else
                {
                    //rootPage.NotifyUser(MainPage.appbarTileId + " not unpinned.", NotifyType.ErrorMessage);
                }
            }
            else
            {
                // Prepare package images for the medium tile size in our tile to be pinned
                Uri square150x150Logo = new Uri("ms-appx:///Assets/Images/Logo150-150.png");

                // Create a Secondary tile with all the required arguments.
                // Note the last argument specifies what size the Secondary tile should show up as by default in the Pin to start fly out.
                // It can be set to TileSize.Square150x150, TileSize.Wide310x150, or TileSize.Default.  
                // If set to TileSize.Wide310x150, then the asset for the wide size must be supplied as well.  
                // TileSize.Default will default to the wide size if a wide size is provided, and to the medium size otherwise. 
                SecondaryTile secondaryTile = new SecondaryTile(Constants.APPBAR_TILE_ID + "." + SelectedRoute.RouteId,
                                                                SelectedRoute.RouteName,
                                                                SelectedRoute.RouteId,
                                                                square150x150Logo,
                                                                TileSize.Square150x150);

                // Whether or not the app name should be displayed on the tile can be controlled for each tile size.  The default is false.
                secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;

                // Specify a foreground text value.
                // The tile background color is inherited from the parent unless a separate value is specified.
                secondaryTile.VisualElements.ForegroundText = ForegroundText.Dark;

                // OK, the tile is created and we can now attempt to pin the tile.
                // Since pinning a secondary tile on Windows Phone will exit the app and take you to the start screen, any code after 
                // RequestCreateForSelectionAsync or RequestCreateAsync is not guaranteed to run.  For an example of how to use the OnSuspending event to do
                // work after RequestCreateForSelectionAsync or RequestCreateAsync returns, see Scenario9_PinTileAndUpdateOnSuspend in the SecondaryTiles.WindowsPhone project.
                await secondaryTile.RequestCreateAsync();
            }
        }

        #region ApplicationBar

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { this.RaiseAndSetIfChanged(ref _isBusy, value); }
        }

        public ReactiveCommand<object> RefreshCommand { get; protected set; }

        public ReactiveCommand<object> SearchCommand { get; protected set; }

        public ReactiveCommand<object> PinCommand { get; protected set; }

        #endregion ApplicationBar
    }
}
