using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Xepher.Kazuma.ViewModels
{
    public class RouteCardViewModel
    {
        public RouteCardViewModel(string routeName, string image)
        {
            RouteName = routeName;
            Image = image;
        }

        public string RouteName
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
