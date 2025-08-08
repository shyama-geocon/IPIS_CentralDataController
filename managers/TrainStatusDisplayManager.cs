using IpisCentralDisplayController.Helpers;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace IpisCentralDisplayController.models
{
    public class TrainStatusDisplayManager
    {
        private readonly IJsonHelper _jsonHelper;
        private readonly string _displaySettingsKey = "displaySettings";

        public TrainStatusDisplayManager(IJsonHelper jsonHelper)
        {
            _jsonHelper = jsonHelper;
        }

        public (ColorDisplayTheme Theme, List<TrainDisplayTemplate> TrainTemplates) LoadDisplaySettings()
        {
            // Assuming Load returns a tuple or a specific format
            var settings = _jsonHelper.Load<(ColorDisplayTheme Theme, List<TrainDisplayTemplate> TrainTemplates)>(_displaySettingsKey);
            if (settings == default || settings.TrainTemplates == null || settings.TrainTemplates.Count == 0)
            {
                return GetDefaultDisplaySettings();
            }
            return settings;
        }

        public void SaveDisplaySettings(ColorDisplayTheme theme, List<TrainDisplayTemplate> trainTemplates)
        {
            var settings = (Theme: theme, TrainTemplates: trainTemplates);
            _jsonHelper.Save(_displaySettingsKey, settings);
        }

        private (ColorDisplayTheme Theme, List<TrainDisplayTemplate> TrainTemplates) GetDefaultDisplaySettings()
        {
            var defaultTheme = new ColorDisplayTheme
            {
                BackgroundColor = Color.FromRgb(0, 0, 0),        // Black
                HorizontalLineColor = Color.FromRgb(255, 255, 255), // White
                VerticalLineColor = Color.FromRgb(255, 255, 255),   // White
                MessageLineColor = Color.FromRgb(255, 255, 255)     // White
            };

            var defaultTrainTemplates = new List<TrainDisplayTemplate>();

            // Define a list of status strings for arrival and departure
            //var statusList = new List<string>
            //{
            //    // Arrival statuses
            //    "Running Right Time",
            //    "Will Arrive Shortly",
            //    "Is Arriving On",
            //    "Has Arrived On",
            //    "Running Late",
            //    "Cancelled",
            //    "Indefinite Late",
            //    "Terminated At",
            //    "Platform Changed",

            //    // Departure statuses
            //    "Running Right Time  ",
            //    "Cancelled  ",
            //    "Is Ready to Leave",
            //    "Is on Platform",
            //    "Departed",
            //    "Rescheduled",
            //    "Diverted",
            //    "Delay Departure",
            //    "Platform Change",
            //    "Change of Source"
            //};

            var statusList = new List<byte>
            {
                // Arrival statuses
               0x01,
                0x02,
                0x03,
                0x04,
                0x05,
                0x06,
                0x07,
                0x08,
                0x09,

                // Departure statuses
                0x0A,
                0x0B,
                0x0C,
                0x0D,
                0x0E,
                0x0F,
                0x10,
                0x11,
                0x12,
                0x13

            };

            foreach (var status in statusList)
            {
                var (statusType, statusDescription) = GetStatusTypeAndDescription(status);

                defaultTrainTemplates.Add(new TrainDisplayTemplate
                {
                    StatusType = statusType,
                    StatusDescription = statusDescription,
                    TrainNoColor = Color.FromRgb(255, 255, 0),        // Yellow
                    TrainNameColor = Color.FromRgb(255, 255, 0),      // Yellow
                    TrainTimeColor = Color.FromRgb(255, 255, 0),      // Yellow
                    TrainADColor = Color.FromRgb(255, 255, 0),        // Yellow
                    TrainPFColor = Color.FromRgb(255, 255, 0)         // Yellow
                });
            }

            return (defaultTheme, defaultTrainTemplates);
        }


        private (string StatusType, string StatusDescription) GetStatusTypeAndDescription(byte status)
        {
            string statusType;
            string statusDescription;

            switch (status)
            {
                // Arrival statuses
                case 0x01:
                    statusType = "Arrival";
                    statusDescription = "Running Right Time";
                    break;
                case 0x02:
                    statusType = "Arrival";
                    statusDescription = "Will Arrive Shortly";
                    break;
                case 0x03:
                    statusType = "Arrival";
                    statusDescription = "Is Arriving On";
                    break;
                case 0x04:
                    statusType = "Arrival";
                    statusDescription = "Has Arrived On";
                    break;
                case 0x05:
                    statusType = "Arrival";
                    statusDescription = "Running Late";
                    break;
                case 0x06:
                    statusType = "Arrival";
                    statusDescription = "Cancelled";
                    break;
                case 0x07:
                    statusType = "Arrival";
                    statusDescription = "Indefinitely Late";
                    break;
                case 0x08:
                    statusType = "Arrival";
                    statusDescription = "Terminated At";
                    break;
                case 0x09:
                    statusType = "Arrival";
                    statusDescription = "Platform Changed";
                    break;

                // Departure statuses
                case 0x0A:
                    statusType = "Departure";
                    statusDescription = "Running Right Time";
                    break;
                case 0x0B:
                    statusType = "Departure";
                    statusDescription = "Cancelled";
                    break;
                case 0x0C:
                    statusType = "Departure";
                    statusDescription = "Is Ready to Leave";
                    break;
                case 0x0D:
                    statusType = "Departure";
                    statusDescription = "Is on Platform";
                    break;
                case 0x0E:
                    statusType = "Departure";
                    statusDescription = "Departed";
                    break;
                case 0x0F:
                    statusType = "Departure";
                    statusDescription = "Rescheduled";
                    break;
                case 0x10:
                    statusType = "Departure";
                    statusDescription = "Diverted";
                    break;
                case 0x11:
                    statusType = "Departure";
                    statusDescription = "Delay Departure";
                    break;
                case 0x12:
                    statusType = "Departure";
                    statusDescription = "Platform Change";
                    break;
                case 0x13:
                    statusType = "Departure";
                    statusDescription = "Change of Source";
                    break;

                // Specific to Departure but also an Arrival status:
                //case "Running Right Time":
                //    statusType = "Departure";
                //    statusDescription = "Running Right Time";
                //    break;

                default:
                    statusType = "Unknown";
                    statusDescription = "Unknown";
                    break;
            }

            return (statusType, statusDescription);
        }

    }
}
