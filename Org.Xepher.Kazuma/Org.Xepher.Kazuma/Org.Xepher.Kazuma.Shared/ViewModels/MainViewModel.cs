using Org.Xepher.Kazuma.Common;
using Org.Xepher.Kazuma.Models;
using Org.Xepher.Kazuma.Utils;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Storage;

namespace Org.Xepher.Kazuma.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(IAppBootstrapper bootstrapper, IMessageBus messageBus)
            : base(bootstrapper, messageBus)
        {
            base.PathSegment = Constants.PATH_SEGMENT_MAIN;

            #region FilterData Configuration

            this.ObservableForProperty(vm => vm.FilterTerm)
                .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
                .Select(v => v.Value)
                .DistinctUntilChanged()
                .Subscribe(v =>
                {
                    if (string.IsNullOrEmpty(v))
                    {
                        Routes = SourceRoutes;
                    }
                    else
                    {
                        IList<Route> resultList = new ObservableCollection<Route>();
                        foreach (Route route in SourceRoutes)
                        {
                            if (route.RouteName.Contains(v))
                            {
                                resultList.Add(route);
                            }
                        }
                        Routes = resultList;
                    }
                });

            #endregion FilterData Configuration

            #region Navigation Configuration
            // RouteView
            this.ObservableForProperty(vm => vm.SelectedRoute)
                .Where(v => null != v.Value)
                .Select(v => v.Value)
                //.DistinctUntilChanged()
                .Subscribe(r =>
                {
                    base.HostScreen.Router.Navigate.Execute(new RouteViewModel(base.HostBootstrapper, base.HostMessageBus, r));

                    this.SelectedRoute = null;
                });

            // SettingsView
            NavigateSettingsCommand = ReactiveCommand.Create();

            NavigateSettingsCommand.Subscribe(_ =>
            {
                base.HostScreen.Router.Navigate.Execute(new SettingsViewModel(base.HostBootstrapper, base.HostMessageBus));
            });

            #endregion Navigation Configuration

            #region Refresh Data Configuration

            RefreshCommand = ReactiveCommand.Create(this.WhenAny(vm => vm.IsBusy, r => !r.Value));

            RefreshCommand.Subscribe(async _ =>
            {
                await RequestInternetRoutes();
            });

            // don't use ToProperty(), because the ToProperty() is a Lazy Observation
            // https://github.com/reactiveui/ReactiveUI/blob/master/docs/basics/to-property.md
            this.WhenAny(vm => vm.IsBusy, x => x.Value)
                .Select(x => !x)
                .Subscribe(x =>
                {
                    IsEnabled = x;
                });

            #endregion Refresh Data Configuration

            #region Location Configuration

            LocationCommand = ReactiveCommand.Create(this.WhenAny(vm => vm.IsBusy, r => !r.Value));

            LocationCommand.Subscribe(async _ =>
            {
                IsBusy = true;

                messageBus.SendMessage<string>(Constants.MSG_MAP_LOCATION_GET, Constants.MSGBUS_TOKEN_MESSAGEBAR);

                Geolocator geolocator = new Geolocator { ReportInterval = 1000, DesiredAccuracy = PositionAccuracy.High, DesiredAccuracyInMeters = 10, MovementThreshold = 5 };
                Geoposition location = await geolocator.GetGeopositionAsync(TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(5));

                if (location.Coordinate.Point.Position.Latitude == HostBootstrapper.MyPosition.Latitude && location.Coordinate.Point.Position.Longitude == HostBootstrapper.MyPosition.Longitude)
                {
#if DEBUG
                    messageBus.SendMessage<string>(Constants.MSG_MAP_LOCATION_HAVENT_CHANGE, Constants.MSGBUS_TOKEN_MESSAGEBAR);
#endif
                }
                else
                {
                    messageBus.SendMessage<BasicGeoposition>(location.Coordinate.Point.Position, Constants.MSGBUS_TOKEN_MY_GEOPOSITION);
                }

                IsBusy = false;
            });

            #endregion Location Configuration

            #region Request data when MyPosition is ready

            this.WhenAnyValue(vm => vm.HostBootstrapper.MyPosition)
                .Subscribe(position =>
                {
                    if (!SourceRoutes.Any())
                    {
                        if (ApplicationDataSettingsHelper.ReadValue<bool>(Constants.SETTINGS_IS_LOCALSTORAGE_ENABLED))
                        {
                            Observable.StartAsync(RequestLocalRoutes);
                        }
                        else
                        {
                            Observable.StartAsync(RequestInternetRoutes);
                        }
                    }

                    if (position.Longitude != 0 && position.Latitude != 0)
                    {
                        Observable.StartAsync(RequestSegmentsNearby);
                    }
                });

            #endregion Request data when MyPosition is ready
        }

        private async Task RequestLocalRoutes()
        {
#if DEBUG
            this.Log().Debug("Load Routes via LocalFolder");
#endif
            IsBusy = true;

            Routes = await StorageHelper.ReadData<ObservableCollection<Route>>(ApplicationData.Current.LocalFolder, Constants.STORAGE_FILE_ROUTES);

            if (null == Routes || !Routes.Any())
            {
                await RequestInternetRoutes();
            }
            else
            {
                SourceRoutes.Clear();
                foreach (Route item in Routes)
                {
                    SourceRoutes.Add(item);
                }
            }

            IsBusy = false;
        }

        private async Task RequestInternetRoutes()
        {
#if DEBUG
            this.Log().Debug("Load Routes via internet");
#endif
            ObservableCollection<Route> _routesResult;
            IsBusy = true;

            int retryCount = 0;
            do
            {
                string requestUrl =
                    SignatureUtil.GetRealRequestUrl(string.Format(Constants.TEMPLATE_ALL_LINES,
                        Constants.SETTING_USER_ID,
                        HostBootstrapper.MyPosition.Latitude,
                        HostBootstrapper.MyPosition.Longitude,
                        Constants.DEVICE_TOKEN, Constants.BUS_API_KEY,
                        SignatureUtil.GenerateSeqId(), Constants.BUS_API_SECRET));

                _routesResult = await SignatureUtil.WebRequestAsync<ObservableCollection<Route>>(requestUrl);
                if (++retryCount > 10) break;
                if (retryCount > 1) base.HostMessageBus.SendMessage<string>(string.Format(Constants.MSG_NETWORK_RETRY, retryCount - 1), Constants.MSGBUS_TOKEN_MESSAGEBAR);
            } while (null == _routesResult || !_routesResult.Any());

            if (retryCount > 10)
            {
                base.HostMessageBus.SendMessage<string>(Constants.MSG_NETWORK_UNAVAILABLE, Constants.MSGBUS_TOKEN_MESSAGEBAR);

                IsBusy = false;
                return;
            }

            StorageHelper.WriteData(ApplicationData.Current.LocalFolder, Constants.STORAGE_FILE_ROUTES, _routesResult);

            SourceRoutes.Clear();
            Routes.Clear();

            foreach (Route item in _routesResult)
            {
                SourceRoutes.Add(item);
                Routes.Add(item);
            }

            IsBusy = false;
        }

        private async Task RequestSegmentsNearby()
        {
#if DEBUG
            this.Log().Debug("Load Segments nearby via internet");
#endif
            Dictionary<string, SegmentNearby> _segmentsNearbyResult;

            int retryCount = 0;
            do
            {
#if DEBUG
                string requestUrl =
                    SignatureUtil.GetRealRequestUrl(string.Format(Constants.TEMPLATE_SEGMENTS_NEARBY,
                        Constants.SETTING_USER_ID,
                        31.574089, 120.292374, Constants.DEVICE_TOKEN, Constants.BUS_API_KEY,
                        SignatureUtil.GenerateSeqId(), Constants.BUS_API_SECRET));
#else
                BasicGeoposition encryptedPosition = GeoHelper.bd_encrypt(GeoHelper.gcj_encrypt(HostBootstrapper.MyPosition));

                string requestUrl =
                    SignatureUtil.GetRealRequestUrl(string.Format(Constants.TEMPLATE_SEGMENTS_NEARBY,
                        Constants.SETTING_USER_ID,
                        encryptedPosition.Latitude,
                        encryptedPosition.Longitude, 
                        Constants.DEVICE_TOKEN, Constants.BUS_API_KEY,
                        SignatureUtil.GenerateSeqId(), Constants.BUS_API_SECRET));
#endif

                _segmentsNearbyResult = await SignatureUtil.WebRequestAsync<Dictionary<string, SegmentNearby>>(requestUrl);
                if (++retryCount > 10) break;
                if (retryCount > 1) base.HostMessageBus.SendMessage<string>(string.Format(Constants.MSG_NETWORK_RETRY_OUT_OF_RANGE, retryCount - 1), Constants.MSGBUS_TOKEN_MESSAGEBAR);
            } while (null == _segmentsNearbyResult || !_segmentsNearbyResult.Any());

            if (retryCount > 10)
            {
                base.HostMessageBus.SendMessage<string>(Constants.MSG_NETWORK_UNAVAILABLE_OUT_OF_RANGE, Constants.MSGBUS_TOKEN_MESSAGEBAR);

                IsBusy = false;
                return;
            }

            SegmentsNearby = new ObservableCollection<SegmentNearby>(_segmentsNearbyResult.Values.ToList());
        }

        private IList<Route> _routes = new ObservableCollection<Route>();

        public IList<Route> Routes
        {
            get { return _routes; }
            set { this.RaiseAndSetIfChanged(ref _routes, value); }
        }

        private IList<Route> _sourceRoutes = new ObservableCollection<Route>();

        public IList<Route> SourceRoutes
        {
            get { return _sourceRoutes; }
            set { this.RaiseAndSetIfChanged(ref _sourceRoutes, value); }
        }

        private IList<SegmentNearby> _segmentsNearby;

        public IList<SegmentNearby> SegmentsNearby
        {
            get { return _segmentsNearby; }
            set { this.RaiseAndSetIfChanged(ref _segmentsNearby, value); }
        }

        #region FilterData

        private string _filterTerm = string.Empty;

        public string FilterTerm
        {
            get { return _filterTerm; }
            set { this.RaiseAndSetIfChanged(ref _filterTerm, value); }
        }

        #endregion FilterData

        #region Navigation parameter

        private Route _selectedRoute;

        public Route SelectedRoute
        {
            get { return _selectedRoute; }
            set { this.RaiseAndSetIfChanged(ref _selectedRoute, value); }
        }

        public ReactiveCommand<object> NavigateSettingsCommand { get; protected set; }

        #endregion Navigation parameter

        #region Refresh Data

        private bool _isBusy = true;
        private bool IsBusy
        {
            get { return _isBusy; }
            set { this.RaiseAndSetIfChanged(ref _isBusy, value); }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { this.RaiseAndSetIfChanged(ref _isEnabled, value); }
        }

        public ReactiveCommand<object> RefreshCommand { get; protected set; }

        #endregion Refresh Data

        #region Location

        public ReactiveCommand<object> LocationCommand { get; protected set; }

        #endregion Location
    }
}
