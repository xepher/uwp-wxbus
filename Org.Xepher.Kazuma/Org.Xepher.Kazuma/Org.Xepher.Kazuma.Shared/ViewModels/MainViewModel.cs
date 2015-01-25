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

namespace Org.Xepher.Kazuma.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // used to cache requested data, will use MemoizingMRUCache to store later
        private List<RouteCardViewModel> _sourceRoutes = new List<RouteCardViewModel>();

        public MainViewModel(INavigationService navigationService)
            : base(navigationService)
        {
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
            
            RequestData();
        }

        private async Task RequestData()
        {
            if (_sourceRoutes.Count == 0)
            {
                List<Route> result = await SignatureUtil.WebRequestAsync<List<Route>>(Constants.TEMPLATE_ALL_LINES);

                foreach (Route route in result)
                {
                    RouteCardViewModel routeVM = new RouteCardViewModel(route,
                        "ms-appx:///resources/images/bus_map_mark_bus_2x.png");
                    Routes.Add(routeVM);
                    _sourceRoutes.Add(routeVM);
                }
            }

            DebugLog logger = new DebugLog(typeof(string));
            logger.Info("Total routes count: {0}", _sourceRoutes.Count);
        }
        
        public BindableCollection<RouteCardViewModel> Routes { get; private set; }

        #region FilterData
        private string _FilterTerm;
        public string FilterTerm
        {
            get { return _FilterTerm; }
            set
            {
                _FilterTerm = value;
                NotifyOfPropertyChange(() => FilterTerm);
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
    }
}
