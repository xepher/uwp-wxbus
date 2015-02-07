using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace Org.Xepher.Kazuma.Utils.Converter
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string date = string.Empty;
            if (new DateTime() != (DateTime)value)
            {
                date = ((DateTime)value).TimeOfDay.ToString();
            }
            return date;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
