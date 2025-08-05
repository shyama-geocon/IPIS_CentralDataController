using IpisCentralDisplayController.ntes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

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

        public int STA_Hours
        {
            get => STA.HasValue ? STA.Value.Hours : 0;
            set
            {
                if (STA.HasValue)
                {
                    STA = new TimeSpan(value, STA.Value.Minutes, 0);
                }
                else
                {
                    STA = new TimeSpan(value, 0, 0);
                }
            }
        }

        public int STA_Minutes
        {
            get => STA.HasValue ? STA.Value.Minutes : 0;
            set
            {
                if (STA.HasValue)
                {
                    STA = new TimeSpan(STA.Value.Hours, value, 0);
                }
                else
                {
                    STA = new TimeSpan(0, value, 0);
                }
            }
        }

        public int STD_Hours
        {
            get => STD.HasValue ? STD.Value.Hours : 0;
            set
            {
                if (STD.HasValue)
                {
                    STD = new TimeSpan(value, STD.Value.Minutes, 0);
                }
                else
                {
                    STD = new TimeSpan(value, 0, 0);
                }
            }
        }

        public int STD_Minutes
        {
            get => STD.HasValue ? STD.Value.Minutes : 0;
            set
            {
                if (STD.HasValue)
                {
                    STD = new TimeSpan(STD.Value.Hours, value, 0);
                }
                else
                {
                    STD = new TimeSpan(0, value, 0);
                }
            }
        }


        public string DaysOfDeparture { get; set; }
        public string DaysOfArrival { get; set; }

        public string Platform { get; set; }
        public string CoachSequence { get; set; }
        public List<string> CoachList { get; set; }
        public string TrainType { get; set; }

        public bool IsFromNTES { get; set; }

        // Default constructor
        public TrainMaster() { }

        // Constructor to convert from NtesTrain952
        public TrainMaster(NtesTrain952 ntesTrain)
        {
            TrainNumber = ntesTrain.TrainNo;
            TrainNameEnglish = ntesTrain.TrainName;
            TrainNameHindi = WebUtility.HtmlDecode(ntesTrain.TrainNameHindi);
            TrainNameRegional = "";

            SrcCode = ntesTrain.Src;
            SrcNameEnglish = ntesTrain.SrcName;
            SrcNameHindi = WebUtility.HtmlDecode(ntesTrain.SrcNameHindi);
            SrcNameRegional = "";

            DestCode = ntesTrain.Dstn;
            DestNameEnglish = ntesTrain.DstnName;
            DestNameHindi = WebUtility.HtmlDecode(ntesTrain.DstnNameHindi);
            DestNameRegional = "";

            STA = ParseTime(ntesTrain.STA);
            STD = ParseTime(ntesTrain.STD);

            DaysOfDeparture = string.Join(",", ParseDaysOfWeek(ntesTrain.DaysOfDeparture));
            DaysOfArrival = string.Join(",", ParseDaysOfWeek(ntesTrain.DaysOfArrival));

            Platform = ntesTrain.PlatformNo;
            CoachSequence = !string.IsNullOrWhiteSpace(ntesTrain.DepartureCoachPosition?.Trim())
    ? ntesTrain.DepartureCoachPosition.Trim()
    : !string.IsNullOrWhiteSpace(ntesTrain.ArrivalCoachPosition?.Trim())
        ? ntesTrain.ArrivalCoachPosition.Trim()
        : string.Empty;
            CoachList = CoachSequence.Split('-').ToList();

            TrainType = ntesTrain.TrainTypeName;
            IsFromNTES = true;
        }

        public TrainMaster(ActiveTrain activeTrain)
        {
            TrainNumber = activeTrain.TrainNumber;
            TrainNameEnglish = activeTrain.TrainNameEnglish;
            TrainNameHindi = activeTrain.TrainNameHindi;
            TrainNameRegional = activeTrain.TrainNameRegional;

            SrcCode = activeTrain.SrcCode;
            SrcNameEnglish = activeTrain.SrcNameEnglish;
            SrcNameHindi = activeTrain.SrcNameHindi;
            SrcNameRegional = activeTrain.SrcNameRegional;

            DestCode = activeTrain.DestCode;
            DestNameEnglish = activeTrain.DestNameEnglish;
            DestNameHindi = activeTrain.DestNameHindi;
            DestNameRegional = activeTrain.DestNameRegional;

            STA = activeTrain.STA;
            STD = activeTrain.STD;

            DaysOfDeparture = "";
            DaysOfArrival = "";

            Platform = activeTrain.PFNo.ToString();
            CoachSequence = activeTrain.CoachSequence;
         //   CoachList = activeTrain.CoachList ?? new List<string>();
            CoachList = activeTrain.CoachListEnglish != null ? new List<string>(activeTrain.CoachListEnglish) : new List<string>();

            TrainType = activeTrain.TrainType;
            IsFromNTES = activeTrain.Ref == TrainSource.USER;
        }


        private TimeSpan? ParseTime(string timeString)
        {
            if (string.IsNullOrWhiteSpace(timeString))
            {
                return TimeSpan.Zero;
            }

            if (TimeSpan.TryParseExact(timeString, @"hh\:mm", CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            return null;
        }


        private List<DayOfWeek> ParseDaysOfWeek(string daysString)
        {
            var days = new List<DayOfWeek>();
            if (!string.IsNullOrEmpty(daysString))
            {
                if (daysString.Trim().ToUpper() == "DAILY")
                {
                    days.AddRange(Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>());
                    return days;
                }

                var dayNames = daysString.Split(',');

                foreach (var day in dayNames)
                {
                    string trimmedDay = day.Trim().ToUpper();
                    switch (trimmedDay)
                    {
                        case "MON":
                            days.Add(DayOfWeek.Monday);
                            break;
                        case "TUE":
                            days.Add(DayOfWeek.Tuesday);
                            break;
                        case "WED":
                            days.Add(DayOfWeek.Wednesday);
                            break;
                        case "THU":
                            days.Add(DayOfWeek.Thursday);
                            break;
                        case "FRI":
                            days.Add(DayOfWeek.Friday);
                            break;
                        case "SAT":
                            days.Add(DayOfWeek.Saturday);
                            break;
                        case "SUN":
                            days.Add(DayOfWeek.Sunday);
                            break;
                        default:
                            Console.WriteLine($"Unrecognized day format: {trimmedDay}");
                            break;
                    }
                }
            }
            return days;
        }
    }
}
