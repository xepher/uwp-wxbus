using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Host.Converter
{
    public class LteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = "4G";
            if (null == value || string.IsNullOrEmpty(value.ToString()))
            {
                result = string.Empty;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}