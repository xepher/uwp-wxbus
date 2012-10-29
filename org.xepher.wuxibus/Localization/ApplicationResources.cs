using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Data;

namespace org.xepher.wuxibus.Localization
{
    public class ApplicationResources : IValueConverter
    {
        private static readonly ResourceManager resourceManager = new ResourceManager("slLocalization.MyStrings",
                                                                                      Assembly.GetExecutingAssembly());

        private static CultureInfo _UICulture = Thread.CurrentThread.CurrentUICulture;
        public static CultureInfo UICulture
        {
            get { return _UICulture; }
            set { _UICulture = value; }
        }

        public string Get(string resource)
        {
            return resourceManager.GetString(resource, _UICulture);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var reader = (ApplicationResources)value;
            return reader.Get((string)parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
