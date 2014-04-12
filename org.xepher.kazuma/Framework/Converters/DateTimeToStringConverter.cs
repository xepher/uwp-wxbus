using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Framework.Converters
{
    public class DateTimeToStringConverter : IValueConverter
    {
        public string Format { get; set; }
        public bool IsDateTimeNullable { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string result = string.Empty;
            if (value is DateTime)
            {
                result = ((DateTime)value).ToString(this.Format ?? "d");
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime date;
            if (value != null && DateTime.TryParse(value.ToString(), out date))
            {
                return date;
            }
            else
            {
                if (this.IsDateTimeNullable)
                {
                    return null;
                }
                else
                {
                    return new DateTime();
                }
            }
        }
    }
}
