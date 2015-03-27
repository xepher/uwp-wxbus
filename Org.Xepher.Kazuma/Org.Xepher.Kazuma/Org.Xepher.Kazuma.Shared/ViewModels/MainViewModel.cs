using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;
using Org.Xepher.Kazuma.Models;
using Org.Xepher.Kazuma.Utils;
using ReactiveUI;

namespace Org.Xepher.Kazuma.ViewModels
{
    public class MainViewModel : ReactiveObject, IRoutableViewModel
    {
        /* COOLSTUFF: What is UrlPathSegment
         * 
         * Imagine that the router state is like the path of the URL - what 
         * would the path look like for this particular page? Maybe it would be
         * the current user's name, or an "id". In this case, it's just a 
         * constant. You can get the whole path via 
         * IRoutingState.GetUrlForCurrentRoute.
         */
        public string UrlPathSegment
        {
            get { return "Main"; }
        }
        
        public IScreen HostScreen { get; protected set; }

        // used to cache requested data, will use Windows.Storage.ApplicationData.Current.LocalFolder to store later
        private IList<Route> _sourceRoutes = new List<Route>();

        /* COOLSTUFF: Why the Screen here?
         *
         * Every RoutableViewModel has a pointer to its IScreen. This is really
         * useful in a unit test runner, because you can create a dummy screen,
         * invoke Commands / change Properties, then test to see if you navigated
         * to the correct new screen 
         */
        public MainViewModel(IScreen screen)
        {
            HostScreen = screen;

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
                       IList<Route> resultList = new List<Route>();
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

            #endregion

            #region Navigation Configuration
            #endregion

            Observable.StartAsync(RequestData);
        }

        private async Task RequestData()
        {
            Routes = await StorageHelper.ReadData<ObservableCollection<Route>>(ApplicationData.Current.LocalFolder, "Routes.data");

            if (null == Routes || Routes.Count == 0)
            {
                int retryCount = 0;
                do
                {
                    string requestUrl =
                        SignatureUtil.GetRealRequestUrl(string.Format(Constants.TEMPLATE_ALL_LINES,
                            Constants.SETTING_USER_ID,
                            Constants.BUS_LAT, Constants.BUS_LNG, Constants.DEVICE_TOKEN, Constants.BUS_API_KEY,
                            SignatureUtil.GenerateSeqId(), Constants.BUS_API_SECRET));

                    Routes = await SignatureUtil.WebRequestAsync<List<Route>>(requestUrl);
                    if (++retryCount > 10) break;
                } while (Routes == null || Routes.Count == 0);

                if (retryCount > 10)
                {
                    //GlobalLoading.Instance.IsLoading = false;
                    //MessageBox.Show("网络异常，请稍后再试！");
                    return;
                }

                StorageHelper.WriteData(ApplicationData.Current.LocalFolder, "Routes.data", Routes);
            }

            foreach (Route route in Routes)
            {
                _sourceRoutes.Add(route);
            }
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

        #endregion

        #region Navigation
        public void SelectionChanged(Route sender)
        {
            //navigationService.UriFor<RouteViewModel>().WithParam(x => x.SelectedRouteFlag, sender.Flag)
            //                                          .WithParam(x => x.SelectedRouteId, sender.RouteId)
            //                                          .WithParam(x => x.SelectedRouteName, sender.RouteName)
            //                                          .Navigate();
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
