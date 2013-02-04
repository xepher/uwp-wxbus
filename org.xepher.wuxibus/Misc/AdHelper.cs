using System.Windows;
using System.Windows.Controls;
using GHAdSDK;
using Microsoft.Phone.Shell;

namespace org.xepher.wuxibus.misc
{
    internal class AdHelper
    {
        private const string AdUnitId = "2617f04023277485b0a401f872228571";

        internal static void InitializeAds(Grid grid, IApplicationBar applicationBar)
        {
            GHAdView adView = new GHAdView(AdUnitId);
            adView.VerticalAlignment = VerticalAlignment.Bottom;
            adView.GHAdSuccess += (s, e) => { applicationBar.IsVisible = false; };
            adView.GHAdClosed += (s, e) => { applicationBar.IsVisible = true; };
            grid.Children.Add(adView);
        }
    }
}
