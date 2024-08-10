using System;
using System.Globalization;
using System.Windows.Data;

namespace IpisCentralDisplayController.converters
{
    public class TimeSpanToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpan)
            {
                return timeSpan.TotalSeconds;
            }
            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
            {
                return TimeSpan.FromSeconds(doubleValue);
            }
            return TimeSpan.Zero;
        }
    }
}
