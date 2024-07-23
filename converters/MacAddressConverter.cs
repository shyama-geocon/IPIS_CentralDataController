using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace IpisCentralDisplayController.converters
{
    public class MacAddressConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 6)
                return "";

            byte[] macAddressBytes = new byte[6];
            for (int i = 0; i < 6; i++)
            {
                if (values[i] != null && byte.TryParse(values[i].ToString(), out byte byteValue))
                    macAddressBytes[i] = byteValue;
                else
                    return "";
            }

            return string.Join(":", macAddressBytes.Select(b => b.ToString("X2")));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
