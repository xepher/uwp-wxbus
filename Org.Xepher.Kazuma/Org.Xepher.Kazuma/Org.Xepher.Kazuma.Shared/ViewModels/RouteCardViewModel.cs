using System;
using System.Collections.Generic;
using System.Text;
using Org.Xepher.Kazuma.Models;

namespace Org.Xepher.Kazuma.ViewModels
{
    public class RouteCardViewModel
    {
        public RouteCardViewModel(Route route, string image)
        {
            RouteName = route.RouteName;
            RouteId = route.RouteId;
            Flag = route.Flag;
            Image = image;
        }

        public string RouteName
        {
            get;
            private set;
        }

        public string RouteId
        {
            get;
            private set;
        }
        public string Flag
        {
            get;
            private set;
        }

        public string Image
        {
            get;
            private set;
        }
    }
}
