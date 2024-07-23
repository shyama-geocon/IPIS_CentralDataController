using IpisCentralDisplayController.models;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace IpisCentralDisplayController.converters
{
    public class LanguageToFontFamilyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RegionalLanguage language)
            {
                switch (language)
                {
                    case RegionalLanguage.ENGLISH:
                    case RegionalLanguage.KASHMIRI:
                    case RegionalLanguage.SINDHI:
                    case RegionalLanguage.URDU:
                        return new FontFamily("Arial");
                    default:
                        return new FontFamily("Nirmala UI");
                }
            }
            return new FontFamily("Arial"); // Default font
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
