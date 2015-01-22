using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Caliburn.Micro;
using Org.Xepher.Kazuma.Models;
using Org.Xepher.Kazuma.Utils;

namespace Org.Xepher.Kazuma.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Routes = new BindableCollection<RouteCardViewModel>();

            RequestData();
        }

        private async Task RequestData()
        {
            List<Route> result = await SignatureUtil.WebRequestAsync<List<Route>>(Constants.TEMPLATE_ALL_LINES);

            foreach (Route route in result)
            {
                Routes.Add(new RouteCardViewModel(route.RouteName, "ms-appx:///resources/images/bus_map_mark_bus_2x.png"));
            }

            MessageDialog dialog = new MessageDialog(Routes.Count.ToString());
            dialog.ShowAsync();
        }

        public BindableCollection<RouteCardViewModel> Routes
        {
            get;
            private set;
        }
    }
}
