using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Org.Xepher.Kazuma.Models;
using Org.Xepher.Kazuma.Utils;
using ReactiveUI;

namespace Org.Xepher.Kazuma.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // used to cache requested data, will use Windows.Storage.ApplicationData.Current.LocalFolder to store later
        private IList<Route> _sourceRoutes = new List<Route>();

        public MainViewModel(IScreen screen)
            : base(screen)
        {
            PathSegment = "Main";

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
