using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Org.Xepher.Kazuma.Models;
using Org.Xepher.Kazuma.Utils;
using ReactiveUI;
using Splat;
using System.Linq;

namespace Org.Xepher.Kazuma.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // used to cache requested data, will use Windows.Storage.ApplicationData.Current.LocalFolder to store later
        private IList<Route> _sourceRoutes = new ObservableCollection<Route>();

        public MainViewModel(IScreen screen, IMessageBus messageBus)
            : base(screen, messageBus)
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
                        Routes = _sourceRoutes;
                    }
                    else
                    {
                        IList<Route> resultList = new ObservableCollection<Route>();
                        foreach (Route route in _sourceRoutes)
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
                    base.HostScreen.Router.Navigate.Execute(new RouteViewModel(base.HostScreen, base.HostMessageBus, r));

                    this.SelectedRoute = null;
                });

            // SettingsView
            NavigateSettingsCommand = ReactiveCommand.Create();

            NavigateSettingsCommand.Subscribe(_ =>
            {
                base.HostScreen.Router.Navigate.Execute(new SettingsViewModel(base.HostScreen, base.HostMessageBus));
            });

            #endregion Navigation Configuration

            #region Refresh Data Configuration

            RefreshCommand = ReactiveCommand.Create(this.WhenAny(vm => vm.IsBusy, r => !r.Value));

            RefreshCommand.Subscribe(async _ =>
            {
                await RequestInternetData();
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

            if (ApplicationDataSettingsHelper.ReadValue<bool>(Constants.SETTINGS_IS_LOCALSTORAGE_ENABLED))
            {
                Observable.StartAsync(RequestLocalData);
            }
            else
            {
                Observable.StartAsync(RequestInternetData);
            }
        }

        private async Task RequestLocalData()
        {
#if DEBUG
            this.Log().Debug("Load Routes via LocalFolder");
#endif
            IsBusy = true;

            Routes = await StorageHelper.ReadData<ObservableCollection<Route>>(ApplicationData.Current.LocalFolder, Constants.STORAGE_FILE_ROUTES);

            if (null == Routes || Routes.Count == 0)
            {
                await RequestInternetData();
            }
            else
            {
                _sourceRoutes = Routes.Select(x => x).ToList();
            }

            IsBusy = false;
        }

        private async Task RequestInternetData()
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
                        Constants.BUS_LAT, Constants.BUS_LNG, Constants.DEVICE_TOKEN, Constants.BUS_API_KEY,
                        SignatureUtil.GenerateSeqId(), Constants.BUS_API_SECRET));

                _routesResult = await SignatureUtil.WebRequestAsync<ObservableCollection<Route>>(requestUrl);
                if (++retryCount > 10) break;
                if (retryCount > 1) base.HostMessageBus.SendMessage<string>(string.Format(Constants.MSG_NETWORK_RETRY, retryCount - 1), Constants.MSGBUS_TOKEN_MESSAGEBAR);
            } while (null == _routesResult || _routesResult.Count == 0);

            if (retryCount > 10)
            {
                base.HostMessageBus.SendMessage<string>(Constants.MSG_NETWORK_UNAVAILABLE, Constants.MSGBUS_TOKEN_MESSAGEBAR);

                IsBusy = false;
                return;
            }

            StorageHelper.WriteData(ApplicationData.Current.LocalFolder, Constants.STORAGE_FILE_ROUTES, _routes);

            _sourceRoutes.Clear();
            Routes.Clear();
            Routes = _routesResult;
            _sourceRoutes = _routesResult.Select(x => x).ToList();

            IsBusy = false;
        }

        private IList<Route> _routes;

        public IList<Route> Routes
        {
            get { return _routes; }
            set { this.RaiseAndSetIfChanged(ref _routes, value); }
        }

        public string Title
        {
            get { return string.Format("WxBus -> Current Available Routes Count: {0}", _sourceRoutes.Count); }
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

        private bool _isBusy;
        public bool IsBusy
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
    }
}
