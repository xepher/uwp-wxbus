using System;
using System.Windows.Data;

namespace Host.Converter
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string date = string.Empty;
            if (new DateTime() != (DateTime)value)
            {
                date = ((DateTime)value).TimeOfDay.ToString();
            }
            return date;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
