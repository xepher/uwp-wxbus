using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Xepher.Kazuma.ViewModels
{
    public class RouteCardViewModel
    {
        public RouteCardViewModel(string routeName)
        {
            RouteName = routeName;
        }

        public string RouteName
        {
            get;
            private set;
        }
    }
}
