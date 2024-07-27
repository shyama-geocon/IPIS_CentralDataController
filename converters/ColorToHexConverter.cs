using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace IpisCentralDisplayController.converters
{
    public class ColorToHexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Color color)
            {
                return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Optional: Implement if necessary
            return Binding.DoNothing;
        }
    }

}
