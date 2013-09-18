using System.Windows;

namespace org.xepher.wuxibus.misc
{
    public class ThemeHelper
    {
        public static Theme GetTheme()
        {
            var tempVisibility = Application.Current.Resources["PhoneLightThemeVisibility"];
            var visibility = tempVisibility == null ? Visibility.Collapsed : (Visibility)tempVisibility;
            return (visibility == Visibility.Visible) ? Theme.Light : Theme.Dark;
        }
    }
}
