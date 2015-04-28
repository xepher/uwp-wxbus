using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation;

namespace Org.Xepher.Kazuma.Common
{
    public interface IAppBootstrapper : IScreen
    {
        BasicGeoposition MyPosition { get; set; }
    }
}
