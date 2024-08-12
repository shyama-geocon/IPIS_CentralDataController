using IpisCentralDisplayController.ntes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace IpisCentralDisplayController.models
{
    public class TrainMaster
    {
        public string TrainNumber { get; set; }
        public string TrainNameEnglish { get; set; }
        public string TrainNameHindi { get; set; }
        public string TrainNameRegional { get; set; }

        public string SrcCode { get; set; }
        public string SrcNameEnglish { get; set; }
        public string SrcNameHindi { get; set; }
        public string SrcNameRegional { get; set; }

        public string DestCode { get; set; }
        public string DestNameEnglish { get; set; }
        public string DestNameHindi { get; set; }
        public string DestNameRegional { get; set; }

        public TimeSpan? STA { get; set; }
        public TimeSpan? STD { get; set; }

        public string DaysOfDeparture { get; set; }
        public string DaysOfArrival { get; set; }

        public string Platform { get; set; }
        public string CoachSequence { get; set; }
        public string TrainType { get; set; }

        public bool IsFromNTES { get; set; }

        // Default constructor
        public TrainMaster() { }

        // Constructor to convert from NtesTrain952
        public TrainMaster(NtesTrain952 ntesTrain)
        {
            TrainNumber = ntesTrain.TrainNo;
            TrainNameEnglish = ntesTrain.TrainName;
            TrainNameHindi = ntesTrain.TrainNameHindi;
            TrainNameRegional = ""; // Logic to determine the regional name if needed

            SrcCode = ntesTrain.Src;
            SrcNameEnglish = ntesTrain.SrcName;
            SrcNameHindi = ntesTrain.SrcNameHindi;
            SrcNameRegional = ""; // Logic to determine the regional name if needed

            DestCode = ntesTrain.Dstn;
            DestNameEnglish = ntesTrain.DstnName;
            DestNameHindi = ntesTrain.DstnNameHindi;
            DestNameRegional = ""; // Logic to determine the regional name if needed

            STA = ParseTime(ntesTrain.STA);
            STD = ParseTime(ntesTrain.STD);

            DaysOfDeparture = string.Join(",", ParseDaysOfWeek(ntesTrain.DaysOfDeparture));
            DaysOfArrival = string.Join(",", ParseDaysOfWeek(ntesTrain.DaysOfArrival));

            Platform = ntesTrain.PlatformNo;
            CoachSequence = ntesTrain.CoachPosition;
            TrainType = ntesTrain.TrainTypeName;
            IsFromNTES = true;
        }

        // Helper method to parse time string to TimeSpan
        private TimeSpan? ParseTime(string timeString)
        {
            if (TimeSpan.TryParseExact(timeString, @"hh\:mm", CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }
            return null; // Handle invalid or empty time strings
        }

        // Helper method to parse days of week string (e.g., "Mon,Wed,Fri") to List<DayOfWeek>
        private List<string> ParseDaysOfWeek(string daysString)
        {
            var days = new List<string>();
            if (!string.IsNullOrEmpty(daysString))
            {
                var dayNames = daysString.Split(',');
                foreach (var day in dayNames)
                {
                    if (Enum.TryParse<DayOfWeek>(day.Trim(), true, out var dayOfWeek))
                    {
                        days.Add(dayOfWeek.ToString());
                    }
                }
            }
            return days;
        }
    }
}
