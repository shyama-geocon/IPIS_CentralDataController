using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace IpisCentralDisplayController.converters
{
    public class ADToStatusListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string ad)
            {
                if (ad == "A")
                {
                    // Arrival statuses
                    return new List<string>
                    {
                        "Running Right Time",
                        "Will Arrive Shortly",
                        "Is Arriving On",
                        "Has Arrived On",
                        "Running Late",
                        "Cancelled",
                        "Indefinite Late",
                        "Terminated At",
                        "Platform Changed"
                    };
                }
                else if (ad == "D")
                {
                    // Departure statuses
                    return new List<string>
                    {
                        "Running Right Time",
                        "Cancelled",
                        "Is Ready To Leave",
                        "Is On Platform",
                        "Departed",
                        "Rescheduled",
                        "Diverted",
                        "Delayed",
                        "Platform Changed",
                        "Change Of Source"
                    };
                }
            }

            return new List<string>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
