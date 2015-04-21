using Org.Xepher.Kazuma.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Org.Xepher.Kazuma.Utils
{
    public class SecondaryTileHelper
    {
        public static Route PrepareNavigationParameter(string tileId, string navigateArguments)
        {
            if (tileId.StartsWith(Constants.APPBAR_TILE_ID))
            {
                return new Route { RouteId = navigateArguments };
            }
            return null;
        }
    }
}
