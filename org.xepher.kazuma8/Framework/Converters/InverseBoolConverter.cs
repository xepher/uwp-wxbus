using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Framework.Converters
{
    /// <summary>
    /// Inverts a boolean value.
    /// </summary>
    public sealed class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Invert(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Invert(value);
        }

        private static object Invert(object value)
        {
            var result = false;
            if (value is bool)
            {
                return !(bool)value;
            }

            return result;
        }
    }
}
