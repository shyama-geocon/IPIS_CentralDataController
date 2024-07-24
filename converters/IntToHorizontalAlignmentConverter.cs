using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace IpisCentralDisplayController.converters
{
    public class IntToHorizontalAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return intValue switch
                {
                    0 => HorizontalAlignment.Left,
                    1 => HorizontalAlignment.Center,
                    2 => HorizontalAlignment.Right,
                    _ => HorizontalAlignment.Left,
                };
            }
            return HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is HorizontalAlignment alignment)
            {
                return alignment switch
                {
                    HorizontalAlignment.Left => 0,
                    HorizontalAlignment.Center => 1,
                    HorizontalAlignment.Right => 2,
                    _ => 0,
                };
            }
            return 0;
        }
    }

}
