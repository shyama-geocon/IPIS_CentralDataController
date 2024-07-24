using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace IpisCentralDisplayController.converters
{
    public class IntToVerticalAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return intValue switch
                {
                    0 => VerticalAlignment.Top,
                    1 => VerticalAlignment.Center,
                    2 => VerticalAlignment.Bottom,
                    _ => VerticalAlignment.Top,
                };
            }
            return VerticalAlignment.Top;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is VerticalAlignment alignment)
            {
                return alignment switch
                {
                    VerticalAlignment.Top => 0,
                    VerticalAlignment.Center => 1,
                    VerticalAlignment.Bottom => 2,
                    _ => 0,
                };
            }
            return 0;
        }
    }

}
