using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Input;
using Caliburn.Micro;
using Org.Xepher.Kazuma.Models;
using Org.Xepher.Kazuma.Utils;
using ReactiveUI;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.System.Threading;
using Org.Xepher.Kazuma.ViewModels.UserControls;
using Windows.Storage;

namespace Org.Xepher.Kazuma.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // used to cache requested data, will use Windows.Storage.ApplicationData.Current.LocalFolder to store later
        private List<RouteCardViewModel> _sourceRoutes = new List<RouteCardViewModel>();
        ApplicationDataContainer localSettings = null;

        public MainViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            localSettings = ApplicationData.Current.LocalSettings;

            #region FilterData Configuration
            FilterAsyncCommand = ReactiveCommand.CreateAsyncTask(_ => FilterData(), RxApp.TaskpoolScheduler);

            this.ObservableForProperty(vm => vm.FilterTerm)
                .Throttle(TimeSpan.FromMilliseconds(800), RxApp.MainThreadScheduler)
                .Select(v => v.Value)
                .DistinctUntilChanged()
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .InvokeCommand(FilterAsyncCommand);
            #endregion

            Routes = new BindableCollection<RouteCardViewModel>();

            Observable.StartAsync(RequestData);
        }

        private async Task RequestData()
        {
            List<Route> result = await StorageHelper.ReadData<List<Route>>(ApplicationData.Current.LocalFolder, "Routes.data");

            if (result.Count == 0)
            {
                int retryCount = 0;
                do
                {
                    string requestUrl =
                        SignatureUtil.GetRealRequestUrl(string.Format(Constants.TEMPLATE_ALL_LINES,
                            Constants.SETTING_USER_ID,
                            Constants.BUS_LAT, Constants.BUS_LNG, Constants.DEVICE_TOKEN, Constants.BUS_API_KEY,
                            SignatureUtil.GenerateSeqId(), Constants.BUS_API_SECRET));

                    result = await SignatureUtil.WebRequestAsync<List<Route>>(requestUrl);
                    if (++retryCount > 10) break;
                } while (result == null || result.Count == 0);

                if (retryCount > 10)
                {
                    //GlobalLoading.Instance.IsLoading = false;
                    //MessageBox.Show("网络异常，请稍后再试！");
                    return;
                }
                
                StorageHelper.WriteData(ApplicationData.Current.LocalFolder, "Routes.data", result);
            }

            foreach (Route route in result)
            {
                RouteCardViewModel routeVM = new RouteCardViewModel(route,
                    "ms-appx:///resources/images/bus_map_mark_bus_2x.png");
                Routes.Add(routeVM);
                _sourceRoutes.Add(routeVM);
            }

            DebugLog logger = new DebugLog(typeof(string));
            logger.Info("Total routes count: {0}", _sourceRoutes.Count);
        }

        public BindableCollection<RouteCardViewModel> Routes { get; set; }

        #region FilterData
        private string _FilterTerm;
        public string FilterTerm
        {
            get { return _FilterTerm; }
            set
            {
                _FilterTerm = value;
                base.NotifyOfPropertyChange(() => FilterTerm);
            }
        }

        private ReactiveCommand<Unit> FilterAsyncCommand { get; set; }

        private Task<Unit> FilterData()
        {
            return Task.Run(() =>
            {
                Routes.Clear();
                foreach (RouteCardViewModel route in _sourceRoutes)
                {
                    if (string.IsNullOrEmpty(FilterTerm) || route.CurrentRoute.RouteName.Contains(FilterTerm))
                    {
                        Routes.Add(route);
                    }
                }

                return new Unit();
            });
        }
        #endregion

        #region Navigation
        public void SelectionChanged(RouteCardViewModel sender)
        {
            DebugLog logger = new DebugLog(typeof(string));
            logger.Info(sender.CurrentRoute.RouteName);

            navigationService.UriFor<RouteViewModel>().WithParam(x => x.SelectedRouteFlag, sender.CurrentRoute.Flag)
                                                      .WithParam(x => x.SelectedRouteId, sender.CurrentRoute.RouteId)
                                                      .WithParam(x => x.SelectedRouteName, sender.CurrentRoute.RouteName)
                                                      .Navigate();
        }
        #endregion

        #region Debug
        private async void ClearCache()
        {
            await ApplicationData.Current.ClearAsync();
        }

        #endregion Debug
    }
}
