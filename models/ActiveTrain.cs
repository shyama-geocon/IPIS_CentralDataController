using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using IpisCentralDisplayController.ntes;

namespace IpisCentralDisplayController.models
{
    public enum TrainSource
    {
        USER,
        NTES,
        TRAIN_DB
    }

    public class ActiveTrain : INotifyPropertyChanged
    {
        // Implement the INotifyPropertyChanged event
        public event PropertyChangedEventHandler PropertyChanged;

        // Method to raise the PropertyChanged event
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private DateTime _createdTime;
        public DateTime CreatedTime
        {
            get => _createdTime;
            set
            {
                if (_createdTime != value)
                {
                    _createdTime = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime _modifiedTime;
        public DateTime ModifiedTime
        {
            get => _modifiedTime;
            set
            {
                if (_modifiedTime != value)
                {
                    _modifiedTime = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isSelected;
        [field: NonSerialized]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        private TrainSource _ref;
        public TrainSource Ref
        {
            get => _ref;
            set
            {
                if (_ref != value)
                {
                    _ref = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _trainNumber;
        public string TrainNumber
        {
            get => _trainNumber;
            set
            {
                if (_trainNumber != value)
                {
                    _trainNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _trainNameEnglish;
        public string TrainNameEnglish
        {
            get => _trainNameEnglish;
            set
            {
                if (_trainNameEnglish != value)
                {
                    _trainNameEnglish = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _trainNameHindi;
        public string TrainNameHindi
        {
            get => _trainNameHindi;
            set
            {
                if (_trainNameHindi != value)
                {
                    _trainNameHindi = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _trainNameRegional;
        public string TrainNameRegional
        {
            get => _trainNameRegional;
            set
            {
                if (_trainNameRegional != value)
                {
                    _trainNameRegional = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _srcCode;
        public string SrcCode
        {
            get => _srcCode;
            set
            {
                if (_srcCode != value)
                {
                    _srcCode = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _srcNameEnglish;
        public string SrcNameEnglish
        {
            get => _srcNameEnglish;
            set
            {
                if (_srcNameEnglish != value)
                {
                    _srcNameEnglish = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _srcNameHindi;
        public string SrcNameHindi
        {
            get => _srcNameHindi;
            set
            {
                if (_srcNameHindi != value)
                {
                    _srcNameHindi = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _srcNameRegional;
        public string SrcNameRegional
        {
            get => _srcNameRegional;
            set
            {
                if (_srcNameRegional != value)
                {
                    _srcNameRegional = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _destCode;
        public string DestCode
        {
            get => _destCode;
            set
            {
                if (_destCode != value)
                {
                    _destCode = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _destNameEnglish;
        public string DestNameEnglish
        {
            get => _destNameEnglish;
            set
            {
                if (_destNameEnglish != value)
                {
                    _destNameEnglish = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _destNameHindi;
        public string DestNameHindi
        {
            get => _destNameHindi;
            set
            {
                if (_destNameHindi != value)
                {
                    _destNameHindi = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _destNameRegional;
        public string DestNameRegional
        {
            get => _destNameRegional;
            set
            {
                if (_destNameRegional != value)
                {
                    _destNameRegional = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _trainType;
        public string TrainType
        {
            get => _trainType;
            set
            {
                if (_trainType != value)
                {
                    _trainType = value;
                    OnPropertyChanged();
                }
            }
        }

        private TimeSpan? _sta;
        public TimeSpan? STA
        {
            get => _sta;
            set
            {
                if (_sta != value)
                {
                    _sta = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime? _sta_ts;
        public DateTime? STA_TS
        {
            get => _sta_ts;
            set
            {
                if (_sta_ts != value)
                {
                    _sta_ts = value;
                    OnPropertyChanged();
                }
            }
        }

        private TimeSpan? _std;
        public TimeSpan? STD
        {
            get => _std;
            set
            {
                if (_std != value)
                {
                    _std = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime? _std_ts;
        public DateTime? STD_TS
        {
            get => _std_ts;
            set
            {
                if (_std_ts != value)
                {
                    _std_ts = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _ad;
        public string AD
        {
            get => _ad;
            set
            {
                if (_ad != value)
                {
                    _ad = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _status;
        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<string> StatusOptions { get; set; }

        private int _statusIndex;
        public int StatusIndex
        {
            get => _statusIndex;
            set
            {
                if (_statusIndex != value)
                {
                    _statusIndex = value;
                    OnPropertyChanged(nameof(StatusIndex));
                    Status = StatusOptions[_statusIndex];
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        public List<string> ADOptions { get; set; } = new List<string> { "A", "D" };

        private int _adIndex;
        public int ADIndex
        {
            get => _adIndex;
            set
            {
                if (_adIndex != value)
                {
                    _adIndex = value;
                    OnPropertyChanged(nameof(ADIndex));
                    AD = ADOptions[_adIndex];
                    OnPropertyChanged(nameof(AD));
                    UpdateStatusOptions();
                }
            }
        }

        private TimeSpan? _lateBy;
        public TimeSpan? LateBy
        {
            get => _lateBy;
            set
            {
                if (_lateBy != value)
                {
                    _lateBy = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _lateByHours;
        public int LateByHours
        {
            get => _lateByHours;
            set
            {
                if (_lateByHours != value)
                {
                    _lateByHours = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _lateByMinutes;
        public int LateByMinutes
        {
            get => _lateByMinutes;
            set
            {
                if (_lateByMinutes != value)
                {
                    _lateByMinutes = value;
                    OnPropertyChanged();
                }
            }
        }

        private TimeSpan? _eta;
        public TimeSpan? ETA
        {
            get => _eta;
            set
            {
                if (_eta != value)
                {
                    _eta = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _etaHours;
        public int ETA_Hours
        {
            get => _etaHours;
            set
            {
                if (_etaHours != value)
                {
                    _etaHours = value;
                    ETA = new TimeSpan(value, ETA_Minutes, 0);
                    OnPropertyChanged();
                }
            }
        }

        private int _etaMinutes;
        public int ETA_Minutes
        {
            get => _etaMinutes;
            set
            {
                if (_etaMinutes != value)
                {
                    _etaMinutes = value;
                    ETA = new TimeSpan(ETA_Hours, value, 0);
                    OnPropertyChanged();
                }
            }
        }

        private TimeSpan? _etd;
        public TimeSpan? ETD
        {
            get => _etd;
            set
            {
                if (_etd != value)
                {
                    _etd = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _etdHours;
        public int ETD_Hours
        {
            get => _etdHours;
            set
            {
                if (_etdHours != value)
                {
                    _etdHours = value;
                    ETD = new TimeSpan(value, ETD_Minutes, 0);
                    OnPropertyChanged();
                }
            }
        }

        private int _etdMinutes;
        public int ETD_Minutes
        {
            get => _etdMinutes;
            set
            {
                if (_etdMinutes != value)
                {
                    _etdMinutes = value;
                    ETD = new TimeSpan(ETD_Hours, value, 0);
                    OnPropertyChanged();
                }
            }
        }

        private int _pfNo;
        public int PFNo
        {
            get => _pfNo;
            set
            {
                if (_pfNo != value)
                {
                    _pfNo = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _coachSequence;
        public string CoachSequence
        {
            get => _coachSequence;
            set
            {
                if (_coachSequence != value)
                {
                    _coachSequence = value;
                    OnPropertyChanged();
                }
            }
        }

        private List<string> _coachList;
        public List<string> CoachList
        {
            get => _coachList;
            set
            {
                if (_coachList != value)
                {
                    _coachList = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _taddbUpdate;
        public bool TADDB_Update
        {
            get => _taddbUpdate;
            set
            {
                if (_taddbUpdate != value)
                {
                    _taddbUpdate = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _cgdbUpdate;
        public bool CGDB_Update
        {
            get => _cgdbUpdate;
            set
            {
                if (_cgdbUpdate != value)
                {
                    _cgdbUpdate = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _announceUpdate;
        public bool Announce_Update
        {
            get => _announceUpdate;
            set
            {
                if (_announceUpdate != value)
                {
                    _announceUpdate = value;
                    OnPropertyChanged();
                }
            }
        }

        private void UpdateStatusOptions()
        {
            if (AD == "A")
            {
                StatusOptions = new List<string>
            {
                "Select Status", // Default option
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
            else if (AD == "D")
            {
                StatusOptions = new List<string>
            {
                "Select Status", // Default option
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
            else
            {
                StatusOptions = new List<string> { "Select Status" }; // Fallback
            }

            OnPropertyChanged(nameof(StatusOptions));
        }

        public ActiveTrain() 
        {
            CreatedTime = DateTime.Now;
            ModifiedTime = DateTime.Now;
        }

        public void UpdateModificationTime()
        {
            ModifiedTime = DateTime.Now;
        }

        public ActiveTrain(NtesTrain951 ntesTrain)
        {
            CreatedTime = DateTime.Now;
            ModifiedTime = DateTime.Now;

            Ref = TrainSource.NTES;
            TrainNumber = ntesTrain.TrainNo;
            TrainNameEnglish = ntesTrain.TrainName;
            TrainNameHindi = WebUtility.HtmlDecode(ntesTrain.TrainNameHindi);
            TrainNameRegional = ""; // This would be set based on the regional language, if needed.

            SrcCode = ntesTrain.Src;
            SrcNameEnglish = ntesTrain.SrcName;
            SrcNameHindi = WebUtility.HtmlDecode(ntesTrain.SrcNameHindi);
            SrcNameRegional = ""; // This would be set based on the regional language, if needed.

            DestCode = ntesTrain.Dstn;
            DestNameEnglish = ntesTrain.DstnName;
            DestNameHindi = WebUtility.HtmlDecode(ntesTrain.DstnNameHindi);
            DestNameRegional = ""; // This would be set based on the regional language, if needed.

            TrainType = ntesTrain.TrainTypeName;

            STA = ParseTime(ntesTrain.STA_HHMM);
            STA_TS = ParseDateTime(ntesTrain.STA_HHMMDDMM);

            STD = ParseTime(ntesTrain.STD_HHMM);
            STD_TS = ParseDateTime(ntesTrain.STD_HHMMDDMM);

            AD = ntesTrain.ADFlag;
            ADIndex = ADOptions.IndexOf(AD);

            UpdateStatusOptions();

            Status = ntesTrain.TrainStatus;
            StatusIndex = 0;

            for (int i = 0; i < StatusOptions.Count; i++)
            {
                if (StatusOptions[i].IndexOf(Status, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    StatusIndex = i;
                    break;
                }
            }

            if (ntesTrain.ExpectedDelay != null && ntesTrain.ExpectedDelay!="" && ntesTrain.ExpectedDelay.ToUpper() != "ON TIME")
            {
                LateBy = ParseTime(ntesTrain.DelayArr);
                LateByHours = LateBy.Value.Hours;
                LateByMinutes = LateBy.Value.Minutes;
            }
            
            ETA = ParseTime(ntesTrain.ETA_HHMM);

            ETD = ParseTime(ntesTrain.ETD_HHMM);

            PFNo = int.TryParse(ntesTrain.PlatformNo, out var platform) ? platform : 0;

            CoachSequence = !string.IsNullOrWhiteSpace(ntesTrain.DepartureCoachPosition?.Trim())
                ? ntesTrain.DepartureCoachPosition.Trim()
                : !string.IsNullOrWhiteSpace(ntesTrain.ArrivalCoachPosition?.Trim())
                    ? ntesTrain.ArrivalCoachPosition.Trim()
                    : string.Empty;

            CoachList = CoachSequence.Split('-').ToList();

            TADDB_Update = false;
            CGDB_Update = false;
            Announce_Update = false;
        }

        public ActiveTrain(TrainMaster train)
        {
            CreatedTime = DateTime.Now;
            ModifiedTime = DateTime.Now;
            Ref = TrainSource.TRAIN_DB;
            TrainNumber = train.TrainNumber;
            TrainNameEnglish = train.TrainNameEnglish;
            TrainNameHindi = train.TrainNameHindi;
            TrainNameRegional = train.TrainNameRegional;

            SrcCode = train.SrcCode;
            SrcNameEnglish = train.SrcNameEnglish;
            SrcNameHindi = train.SrcNameHindi;
            SrcNameRegional = train.SrcNameRegional;

            DestCode = train.DestCode;
            DestNameEnglish = train.DestNameEnglish;
            DestNameHindi = train.DestNameHindi;
            DestNameRegional = train.DestNameRegional;

            TrainType = train.TrainType;

            STA = train.STA;
            STD = train.STD;            

            ETA = train.STA;
            ETD = train.STD;

            CoachSequence = train.CoachSequence;
            CoachList = train.CoachList ?? new List<string>();

            TADDB_Update = false;
            CGDB_Update = false;
            Announce_Update = false;
        }

        public ActiveTrain DeepClone()
        {
            return new ActiveTrain
            {
                CreatedTime = this.CreatedTime,
                ModifiedTime = this.ModifiedTime,
                Ref = this.Ref,
                TrainNumber = this.TrainNumber,
                TrainNameEnglish = this.TrainNameEnglish,
                TrainNameHindi = this.TrainNameHindi,
                TrainNameRegional = this.TrainNameRegional,
                SrcCode = this.SrcCode,
                SrcNameEnglish = this.SrcNameEnglish,
                SrcNameHindi = this.SrcNameHindi,
                SrcNameRegional = this.SrcNameRegional,
                DestCode = this.DestCode,
                DestNameEnglish = this.DestNameEnglish,
                DestNameHindi = this.DestNameHindi,
                DestNameRegional = this.DestNameRegional,
                TrainType = this.TrainType,
                STA = this.STA,
                STA_TS = this.STA_TS,
                STD = this.STD,
                STD_TS = this.STD_TS,
                AD = this.AD,
                Status = this.Status,
                StatusOptions = this.StatusOptions != null ? new List<string>(this.StatusOptions) : new List<string>(), // Handle null list
                StatusIndex = this.StatusIndex,
                ADOptions = this.ADOptions != null ? new List<string>(this.ADOptions) : new List<string>(), // Handle null list
                ADIndex = this.ADIndex,
                LateBy = this.LateBy,
                LateByHours = this.LateByHours,
                LateByMinutes = this.LateByMinutes,
                ETA = this.ETA,
                ETA_Hours = this.ETA_Hours,
                ETA_Minutes = this.ETA_Minutes,
                ETD = this.ETD,
                ETD_Hours = this.ETD_Hours,
                ETD_Minutes = this.ETD_Minutes,
                PFNo = this.PFNo,
                CoachSequence = this.CoachSequence,
                CoachList = this.CoachList != null ? new List<string>(this.CoachList) : new List<string>(), // Handle null list
                TADDB_Update = this.TADDB_Update,
                CGDB_Update = this.CGDB_Update,
                Announce_Update = this.Announce_Update
            };
        }

        public void UpdateFrom(ActiveTrain source)
        {
            if (source == null) return;

            CreatedTime = source.CreatedTime;
            ModifiedTime = source.ModifiedTime;
            Ref = source.Ref;
            TrainNumber = source.TrainNumber;
            TrainNameEnglish = source.TrainNameEnglish;
            TrainNameHindi = source.TrainNameHindi;
            TrainNameRegional = source.TrainNameRegional;
            SrcCode = source.SrcCode;
            SrcNameEnglish = source.SrcNameEnglish;
            SrcNameHindi = source.SrcNameHindi;
            SrcNameRegional = source.SrcNameRegional;
            DestCode = source.DestCode;
            DestNameEnglish = source.DestNameEnglish;
            DestNameHindi = source.DestNameHindi;
            DestNameRegional = source.DestNameRegional;
            TrainType = source.TrainType;
            STA = source.STA;
            STA_TS = source.STA_TS;
            STD = source.STD;
            STD_TS = source.STD_TS;
            AD = source.AD;
            Status = source.Status;
            StatusOptions = source.StatusOptions != null ? new List<string>(source.StatusOptions) : new List<string>(); // Deep copy with null check
            StatusIndex = source.StatusIndex;
            ADOptions = source.ADOptions != null ? new List<string>(source.ADOptions) : new List<string>(); // Deep copy with null check
            ADIndex = source.ADIndex;
            LateBy = source.LateBy;
            LateByHours = source.LateByHours;
            LateByMinutes = source.LateByMinutes;
            ETA = source.ETA;
            ETA_Hours = source.ETA_Hours;
            ETA_Minutes = source.ETA_Minutes;
            ETD = source.ETD;
            ETD_Hours = source.ETD_Hours;
            ETD_Minutes = source.ETD_Minutes;
            PFNo = source.PFNo;
            CoachSequence = source.CoachSequence;
            CoachList = source.CoachList != null ? new List<string>(source.CoachList) : new List<string>(); // Deep copy with null check
            TADDB_Update = source.TADDB_Update;
            CGDB_Update = source.CGDB_Update;
            Announce_Update = source.Announce_Update;

            OnPropertyChanged(null); // Notify that all properties may have changed
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

        private DateTime? ParseDateTime(string staString)
        {
            if (string.IsNullOrEmpty(staString))
            {
                return null;
            }

            try
            {
                DateTime parsedDate = DateTime.ParseExact(staString, "HH:mm dd/MM", CultureInfo.InvariantCulture);
                return parsedDate;
            }
            catch (FormatException)
            {
                return null;
            }
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
