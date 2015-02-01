using System;
using System.Collections.Generic;
using System.Text;
using Caliburn.Micro;
using Org.Xepher.Kazuma.Models;
using ReactiveUI;

namespace Org.Xepher.Kazuma.ViewModels.UserControls
{
    public class RouteCardViewModel
    {
        public RouteCardViewModel(Route route, string image)
        {
            CurrentRoute = route;
            Image = image;
        }

        public Route CurrentRoute
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
