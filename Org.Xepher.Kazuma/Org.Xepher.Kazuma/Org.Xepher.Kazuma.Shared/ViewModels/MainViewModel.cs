using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Org.Xepher.Kazuma.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private List<RouteCardViewModel> _sourceRoutes = new List<RouteCardViewModel>();

        public MainViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Routes = new BindableCollection<RouteCardViewModel>();

            MessageBus.Current.Listen<string>().Subscribe(s =>
            {
                Routes.Clear();
                foreach (RouteCardViewModel route in _sourceRoutes)
                {
                    if (string.IsNullOrEmpty(s) || route.RouteName.Contains(s))
                    {
                        Routes.Add(route);
                    }
                }
            });

            RequestData();
        }

        private async Task RequestData()
        {
            List<Route> result = await SignatureUtil.WebRequestAsync<List<Route>>(Constants.TEMPLATE_ALL_LINES);

            foreach (Route route in result)
            {
                RouteCardViewModel routeVM = new RouteCardViewModel(route,
                    "ms-appx:///resources/images/bus_map_mark_bus_2x.png");
                Routes.Add(routeVM);
                _sourceRoutes.Add(routeVM);
            }

            DebugLog logger = new DebugLog(typeof(string));
            logger.Info("Total routes count: {0}", result.Count);
        }

        public BindableCollection<RouteCardViewModel> Routes
        {
            get;
            private set;
        }

        public ReactiveCommand<ViewModelBase> NavigateCommand { get; protected set; }
    }
}
