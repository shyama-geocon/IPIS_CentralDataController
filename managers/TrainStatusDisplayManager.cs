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
            var statusList = new List<string>
            {
                // Arrival statuses
                "Running Right Time",
                "Will Arrive Shortly",
                "Is Arriving On",
                "Has Arrived On",
                "Running Late",
                "Cancelled",
                "Indefinite Late",
                "Terminated At",
                "Platform Changed",

                // Departure statuses
                "Running Right Time",
                "Cancelled",
                "Is Ready to Leave",
                "Is on Platform",
                "Departed",
                "Rescheduled",
                "Diverted",
                "Delay Departure",
                "Platform Change",
                "Change of Source"
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


        private (string StatusType, string StatusDescription) GetStatusTypeAndDescription(string status)
        {
            string statusType;
            string statusDescription;

            switch (status)
            {
                // Arrival statuses
                case "Running Right Time":
                    statusType = "Arrival";
                    statusDescription = "Running Right Time";
                    break;
                case "Will Arrive Shortly":
                    statusType = "Arrival";
                    statusDescription = "Will Arrive Shortly";
                    break;
                case "Is Arriving On":
                    statusType = "Arrival";
                    statusDescription = "Is Arriving On";
                    break;
                case "Has Arrived On":
                    statusType = "Arrival";
                    statusDescription = "Has Arrived On";
                    break;
                case "Running Late":
                    statusType = "Arrival";
                    statusDescription = "Running Late";
                    break;
                case "Cancelled":
                    statusType = "Arrival";
                    statusDescription = "Cancelled";
                    break;
                case "Indefinite Late":
                    statusType = "Arrival";
                    statusDescription = "Indefinitely Late";
                    break;
                case "Terminated At":
                    statusType = "Arrival";
                    statusDescription = "Terminated At";
                    break;
                case "Platform Changed":
                    statusType = "Arrival";
                    statusDescription = "Platform Changed";
                    break;

                // Departure statuses
                //case "Cancelled":
                //    statusType = "Departure";
                //    statusDescription = "Cancelled";
                //    break;
                case "Is Ready to Leave":
                    statusType = "Departure";
                    statusDescription = "Is Ready to Leave";
                    break;
                case "Is on Platform":
                    statusType = "Departure";
                    statusDescription = "Is on Platform";
                    break;
                case "Departed":
                    statusType = "Departure";
                    statusDescription = "Departed";
                    break;
                case "Rescheduled":
                    statusType = "Departure";
                    statusDescription = "Rescheduled";
                    break;
                case "Diverted":
                    statusType = "Departure";
                    statusDescription = "Diverted";
                    break;
                case "Delay Departure":
                    statusType = "Departure";
                    statusDescription = "Delay Departure";
                    break;
                case "Platform Change":
                    statusType = "Departure";
                    statusDescription = "Platform Change";
                    break;
                case "Change of Source":
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
