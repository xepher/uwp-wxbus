using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Org.Xepher.Kazuma.Models;
using Org.Xepher.Kazuma.Utils;

namespace Org.Xepher.Kazuma.ViewModels
{
    public class RouteViewModel : ViewModelBase
    {
        public RouteViewModel(INavigationService navigationService)
            : base(navigationService)
        {
        }

        protected override void OnInitialize()
        {
            this.SelectedRoute = new Route() { Flag = SelectedRouteFlag, RouteId = SelectedRouteId, RouteName = SelectedRouteName };

            Observable.StartAsync(InitSegments);

            base.OnInitialize();
        }

        public Route SelectedRoute { get; private set; }

        public string SelectedRouteFlag { get; set; }

        public string SelectedRouteId { get; set; }

        public string SelectedRouteName { get; set; }

        private BindableCollection<Segment> _segments;
        public BindableCollection<Segment> Segments {
            get
            {
                return _segments;
            }
            private set
            {
                _segments = value;
                base.NotifyOfPropertyChange(() => Segments);
            }
        }

        private async Task InitSegments()
        {
            //if (GlobalLoading.Instance.IsLoading) return;
            //GlobalLoading.Instance.IsLoading = true;

            int retryCount = 0;
            BindableCollection<Segment> result;
            do
            {
                string requestUrl =
                    SignatureUtil.GetRealRequestUrl(string.Format(Constants.TEMPLATE_SEGMENTS, Constants.SETTING_USER_ID,
                        Constants.BUS_LAT, Constants.BUS_LNG, Constants.DEVICE_TOKEN, Constants.BUS_API_KEY,
                        SignatureUtil.GenerateSeqId(), SelectedRoute.RouteId,
                        Constants.BUS_API_SECRET));

                result = await SignatureUtil.WebRequestAsync<BindableCollection<Segment>>(requestUrl);
                if (++retryCount > 10) break;
            } while (result == null || result.Count == 0);

            if (retryCount > 10)
            {
                //GlobalLoading.Instance.IsLoading = false;
                //MessageBox.Show("网络异常，请稍后再试！");
                return;
            }

            Segments = result;

            //GlobalLoading.Instance.IsLoading = false;
        }
    }
}
