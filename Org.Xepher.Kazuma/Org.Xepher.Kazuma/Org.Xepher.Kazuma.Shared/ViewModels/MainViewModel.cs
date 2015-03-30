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

namespace Org.Xepher.Kazuma.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // used to cache requested data, will use Windows.Storage.ApplicationData.Current.LocalFolder to store later
        private IList<Route> _sourceRoutes = new ObservableCollection<Route>();

        public MainViewModel(IScreen screen)
            : base(screen)
        {
            base.PathSegment = "Main";

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
                    base.HostScreen.Router.Navigate.Execute(new RouteViewModel(base.HostScreen, r));

                    this.SelectedRoute = null;
                });

            // SettingsView
            NavigateSettingsCommand = ReactiveCommand.Create();

            NavigateSettingsCommand.Subscribe(_ =>
            {
                base.HostScreen.Router.Navigate.Execute(new SettingsViewModel(base.HostScreen));
            });

            #endregion Navigation Configuration

            #region Refresh Data Configuration

            RefreshCommand = ReactiveCommand.Create(this.WhenAny(vm => vm.IsBusy, r => !r.Value));

            RefreshCommand.Subscribe(async _ =>
            {
                _sourceRoutes.Clear();
                Routes.Clear();
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

            Observable.StartAsync(RequestLocalData);
        }

        private void BindSourceRoutes()
        {
            foreach (Route route in Routes)
            {
                _sourceRoutes.Add(route);
            }
        }

        private async Task RequestLocalData()
        {
#if DEBUG
            this.Log().Debug("Load Routes via LocalFolder");
#endif
            IsBusy = true;

            Routes = await StorageHelper.ReadData<ObservableCollection<Route>>(ApplicationData.Current.LocalFolder, "Routes.data");

            if (null == Routes || Routes.Count == 0)
            {
                await RequestInternetData();
            }
            else
            {
                BindSourceRoutes();
            }

            IsBusy = false;
        }

        private async Task RequestInternetData()
        {
#if DEBUG
            this.Log().Debug("Load Routes via internet");
#endif

            IsBusy = true;

            int retryCount = 0;
            do
            {
                string requestUrl =
                    SignatureUtil.GetRealRequestUrl(string.Format(Constants.TEMPLATE_ALL_LINES,
                        Constants.SETTING_USER_ID,
                        Constants.BUS_LAT, Constants.BUS_LNG, Constants.DEVICE_TOKEN, Constants.BUS_API_KEY,
                        SignatureUtil.GenerateSeqId(), Constants.BUS_API_SECRET));

                Routes = await SignatureUtil.WebRequestAsync<ObservableCollection<Route>>(requestUrl);
                if (++retryCount > 10) break;
            } while (Routes == null || Routes.Count == 0);

            if (retryCount > 10)
            {
                //GlobalLoading.Instance.IsLoading = false;
                //MessageBox.Show("网络异常，请稍后再试！");
                return;
            }

            StorageHelper.WriteData(ApplicationData.Current.LocalFolder, "Routes.data", Routes);

            BindSourceRoutes();

            IsBusy = false;
        }

        private IList<Route> _routes;

        public IList<Route> Routes
        {
            get { return _routes; }
            set { this.RaiseAndSetIfChanged(ref _routes, value); }
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
