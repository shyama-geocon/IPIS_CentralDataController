using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using IpisCentralDisplayController.models;

namespace IpisCentralDisplayController.converters
{
    public class UserRightsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is List<UserRights> userRights)
            {
                return string.Join(", ", userRights);
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return str.Split(new[] { ", " }, StringSplitOptions.None).Select(r => Enum.Parse(typeof(UserRights), r)).ToList();
            }
            return new List<UserRights>();
        }
    }
}
