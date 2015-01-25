using System;
using System.Collections.Generic;
using System.Text;
using Caliburn.Micro;
using Org.Xepher.Kazuma.Models;

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

            base.OnInitialize();
        }

        public Route SelectedRoute { get; private set; }

        public string SelectedRouteFlag { get; set; }

        public string SelectedRouteId { get; set; }

        public string SelectedRouteName { get; set; }

    }
}
