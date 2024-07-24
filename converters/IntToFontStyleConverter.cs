using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace IpisCentralDisplayController.converters
{
    public class IntToFontStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return intValue switch
                {
                    0 => FontStyles.Normal,
                    1 => FontStyles.Italic,
                    2 => FontStyles.Oblique,
                    _ => FontStyles.Normal,
                };
            }
            return FontStyles.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FontStyle fontStyle)
            {
                return fontStyle == FontStyles.Italic ? 1 : fontStyle == FontStyles.Oblique ? 2 : 0;
            }
            return 0;
        }
    }

}
