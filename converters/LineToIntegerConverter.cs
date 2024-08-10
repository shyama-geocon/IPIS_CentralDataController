using System;
using System.Globalization;
using System.Windows.Data;

namespace IpisCentralDisplayController.converters
{
    public class LineToIntegerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index)
            {
                return index switch
                {
                    0 => 128, // 6-line
                    1 => 256, // 12-line
                    2 => 384, // 18-line
                    3 => 1080, // LED-TV HD
                    4 => -1,  // Custom
                    _ => 0
                };
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return intValue switch
                {
                    128 => 0, // 6-line
                    256 => 1, // 12-line
                    384 => 2, // 18-line
                    1080 => 3, // LED-TV HD
                    -1 => 4,  // Custom
                    _ => 0
                };
            }
            return -1;
        }
    }
}
