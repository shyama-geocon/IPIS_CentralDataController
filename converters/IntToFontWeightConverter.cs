using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace IpisCentralDisplayController.converters
{

    public class IntToFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return intValue switch
                {
                    0 => FontWeights.Normal,
                    1 => FontWeights.Bold,
                    _ => FontWeights.Normal,
                };
            }
            return FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FontWeight fontWeight)
            {
                return fontWeight == FontWeights.Bold ? 1 : 0;
            }
            return 0;
        }
    }

}
