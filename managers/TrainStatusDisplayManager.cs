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

            foreach (var status in (TrainStatus[])Enum.GetValues(typeof(TrainStatus)))
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

        private (string StatusType, string StatusDescription) GetStatusTypeAndDescription(TrainStatus status)
        {
            string statusType;
            string statusDescription;

            switch (status)
            {
                // Arrival statuses
                case TrainStatus.RunningRightTimeArrival:
                    statusType = "Arrival";
                    statusDescription = "Running Right Time";
                    break;
                case TrainStatus.WillArriveShortly:
                    statusType = "Arrival";
                    statusDescription = "Will Arrive Shortly";
                    break;
                case TrainStatus.IsArrivingOn:
                    statusType = "Arrival";
                    statusDescription = "Is Arriving On";
                    break;
                case TrainStatus.HasArrivedOn:
                    statusType = "Arrival";
                    statusDescription = "Has Arrived On";
                    break;
                case TrainStatus.RunningLateArrival:
                    statusType = "Arrival";
                    statusDescription = "Running Late";
                    break;
                case TrainStatus.CancelledArrival:
                    statusType = "Arrival";
                    statusDescription = "Cancelled";
                    break;
                case TrainStatus.IndefiniteLateArrival:
                    statusType = "Arrival";
                    statusDescription = "Indefinitely Late";
                    break;
                case TrainStatus.TerminatedAt:
                    statusType = "Arrival";
                    statusDescription = "Terminated At";
                    break;
                case TrainStatus.PlatformChangedArrival:
                    statusType = "Arrival";
                    statusDescription = "Platform Changed";
                    break;

                // Departure statuses
                case TrainStatus.RunningRightTimeDeparture:
                    statusType = "Departure";
                    statusDescription = "Running Right Time";
                    break;
                case TrainStatus.CancelledDeparture:
                    statusType = "Departure";
                    statusDescription = "Cancelled";
                    break;
                case TrainStatus.IsReadyToLeave:
                    statusType = "Departure";
                    statusDescription = "Is Ready to Leave";
                    break;
                case TrainStatus.IsOnPlatform:
                    statusType = "Departure";
                    statusDescription = "Is on Platform";
                    break;
                case TrainStatus.Departed:
                    statusType = "Departure";
                    statusDescription = "Departed";
                    break;
                case TrainStatus.Rescheduled:
                    statusType = "Departure";
                    statusDescription = "Rescheduled";
                    break;
                case TrainStatus.Diverted:
                    statusType = "Departure";
                    statusDescription = "Diverted";
                    break;
                case TrainStatus.DelayDeparture:
                    statusType = "Departure";
                    statusDescription = "Delay Departure";
                    break;
                case TrainStatus.PlatformChangeDeparture:
                    statusType = "Departure";
                    statusDescription = "Platform Change";
                    break;
                case TrainStatus.ChangeOfSource:
                    statusType = "Departure";
                    statusDescription = "Change of Source";
                    break;

                default:
                    statusType = "Unknown";
                    statusDescription = "Unknown";
                    break;
            }

            return (statusType, statusDescription);
        }
    }
}
