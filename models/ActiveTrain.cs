using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Windows;
using IpisCentralDisplayController.ntes;
using IpisCentralDisplayController.views;

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

        private string specialStatusCodeField;
        public string SpecialStatusCodeField
        {
            get { return specialStatusCodeField; }
            set { specialStatusCodeField = value; }
        }


        #region TimeScheduled

        private int _STA_Hours;
        public int STA_Hours
        {

            //get => STA.HasValue ? STA.Value.Hours : 0;
           // get => _STA_Hours;

            get { return _STA_Hours; }
            set
            {
                _STA_Hours = value;
               
                if (STA.HasValue)
                {
                    STA = new TimeSpan(value, STA.Value.Minutes, 0);
                }
                else
                {
                    STA = new TimeSpan(value, 0, 0);
                }
                OnPropertyChanged();

                if (SelectedADOption == "A")
                {                  
                    ETA_Hours = value + LateByHours;
     
                }
                else if(SelectedADOption == "D")
                {
                    ETA_Hours = STA_Hours ;
                   
                }

            }
            
        }


        private int _STA_Minutes;
        public int STA_Minutes
        {
            //get => STA.HasValue ? STA.Value.Minutes : 0;
            get { return _STA_Minutes; }
            set 
            {  
                _STA_Minutes = value;

                if (STA.HasValue)
                {
                    STA = new TimeSpan(STA.Value.Hours, value,  0);
                }
                else
                {
                    STA = new TimeSpan(0, value, 0);
                }
                OnPropertyChanged();
               

                if (SelectedADOption == "A")
                {
                    ETA_Minutes = value + LateByMinutes;
                }
                else if (SelectedADOption == "D")
                {
                    ETA_Minutes = STA_Minutes ;

                }

                //if (SelectedADOption == "A")
                //{
                //    //LateBy = new TimeSpan(value, LateByMinutes, 0);
                //    ETA_Minutes = value + STA_Minutes + LateByMinutes;
                //    ETD_Minutes = value + STD_Minutes + LateByMinutes;

                //}
                //else if (SelectedADOption == "D")
                //{
                //    //  LateBy = new TimeSpan(value, 0, 0);
                //    ETD_Minutes = value + STD_Minutes + LateByMinutes;
                //}





            }
        }



        private int _STD_Hours;
        public int STD_Hours
        {
            get { return _STD_Hours; }
            set 
            {   _STD_Hours = value;

                if (STD.HasValue)
                {
                    STD = new TimeSpan(value, STD.Value.Minutes, 0);
                }
                else
                {
                    STD = new TimeSpan(value, 0, 0);
                }

                OnPropertyChanged();
                // ETD_Hours = value;
                ETD_Hours = value + LateByHours;




            }
        }


        private int _STD_Minutes;
        public int STD_Minutes
        {
            get { return _STD_Minutes; }
            set 
            {
                _STD_Minutes = value;

                if (STD.HasValue)
                {
                    STD = new TimeSpan(STD.Value.Hours, value, 0);
                }
                else
                {
                    STD = new TimeSpan(0, value, 0);
                }
                OnPropertyChanged();
                //ETD_Minutes = value;
                ETD_Minutes = value + LateByMinutes;

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


        #endregion



        #region TimeExpected

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

                    if(SelectedADOption == "A")
                    {
                        //LateBy = new TimeSpan(value, LateByMinutes, 0);
                        ETA_Hours = value + STA_Hours;
                        ETD_Hours = value + STD_Hours;

                    }
                    else if (SelectedADOption == "D")
                    {
                        //  LateBy = new TimeSpan(value, 0, 0);
                        ETD_Hours = value + STD_Hours;
                    }

                    LateBy = new TimeSpan(value, LateByMinutes, 0);

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

                    if (SelectedADOption == "A")
                    {
                        //LateBy = new TimeSpan(value, LateByMinutes, 0);
                        ETA_Minutes = value + STA_Minutes;
                        ETD_Minutes = value + STD_Minutes;

                    }
                    else if (SelectedADOption == "D")
                    {
                        //  LateBy = new TimeSpan(value, 0, 0);
                        ETD_Minutes = value + STD_Minutes;
                    }

                    LateBy = new TimeSpan(LateByHours, value, 0);

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




        #endregion

        private string _oldPFNo;

        public string OldPFNo
        {
            get { return _oldPFNo; }
            set { _oldPFNo = value; }
        }

        #region SpecialStatusstuffPOPUP

        private byte _statusByte;
        public byte StatusByte
        {
            get { return _statusByte; }
            set { _statusByte = value;

                if (value == 0x08)//Terminated At
                {
                  //  IsEnabledStationNameForSplStatusCode = true ;
                    FieldForSplStatusCode= "Terminated At Station:";
                    OnPropertyChanged();

                    SplPopUpStation POPup = new SplPopUpStation(this, "Terminated At Station",value);
                    ////  trainMasterDbWindow.Owner = this;
                    //// POPup.Owner = this;
                    POPup.ShowDialog();

                }
                else if (value == 0x10)//Diverted
                {
                  //  IsEnabledStationNameForSplStatusCode = true ;
                    FieldForSplStatusCode = "Diverted To Station:";

                    SplPopUpStation POPup = new SplPopUpStation(this, "Diverted To Station:", value);
                    //  trainMasterDbWindow.Owner = this;
                    // POPup.Owner = this;
                    POPup.ShowDialog();

                }
                else if (value == 0x13)//Change Of Source
                {
                    //IsEnabledStationNameForSplStatusCode = true ;
                    FieldForSplStatusCode = "Change Of Source Station:";

                    SplPopUpStation POPup = new SplPopUpStation(this, "Change Of Source Station:", value);
                    //  trainMasterDbWindow.Owner = this;
                    // POPup.Owner = this;
                    POPup.ShowDialog();

                }
                else if(value == 0x09) // platform number changed
                {
                    OldPFNo = PFNo.ToString();

                    SplPopUpOldPFNo POPup = new SplPopUpOldPFNo(this);
                    POPup.ShowDialog();
                }
                else
                {
                    //IsEnabledStationNameForSplStatusCode = false ;
                    FieldForSplStatusCode = "";
                    SplStationNameEnglish = "";
                    SplStationNameHindi = "";
                    SplStationNameRegional = "";
                    SplStationCode = "";

                }

              //  OldPFNo = PFNo.ToString();


                // OnPropertyChanged();

            }
        }

        private ObservableCollection<SplStatusOptionItem> _splStatusOptions;
        public ObservableCollection<SplStatusOptionItem> SplStatusOptions
        {
            get => _splStatusOptions;
            set { _splStatusOptions = value; OnPropertyChanged(); }
        }

        private SplStatusOptionItem _splStatusSelectedOption;
        public SplStatusOptionItem SplStatusSelectedOption
        {
            get => _splStatusSelectedOption;
            set { _splStatusSelectedOption = value; OnPropertyChanged(); }
        }

        private string _splStatusField;
        public string SplStatusField
        {
            get { return _splStatusField; }
            set { _splStatusField = value; }
        }


        private string _fieldForSplStatusCode;
        public string FieldForSplStatusCode
        {
            get { return _fieldForSplStatusCode; }
            set
            {

                if (_fieldForSplStatusCode != value)
                {
                    _fieldForSplStatusCode = value;
                    OnPropertyChanged();
                }
            }
        }







        #endregion

        #region Station :SpecialStatusAdditionalFields


        private string _splStationNameEnglish;
        public string SplStationNameEnglish
        {
            get { return _splStationNameEnglish; }
            set { _splStationNameEnglish = value; }
        }

        private string _splStationNameHindi;
        public string SplStationNameHindi
        {
            get { return _splStationNameHindi; }
            set { _splStationNameHindi = value; }
        }

        private string _splStationNameRegional;
        public string SplStationNameRegional
        {
            get { return _splStationNameRegional; }
            set { _splStationNameRegional = value; }
        }

        private string _splStationCode;
        public string SplStationCode
        {
            get { return _splStationCode; }
            set { _splStationCode = value; }
        }


        #endregion


        private string _stationNameForSplStatusCode;
        public string StationNameForSplStatusCode
        {
            get { return _stationNameForSplStatusCode; }
            set {

                if (_stationNameForSplStatusCode != value)
                {
                    _stationNameForSplStatusCode = value;
                    OnPropertyChanged();
                }        
            }
        }


        //private bool _isEnabledStationNameForSplStatusCode;
        //public bool IsEnabledStationNameForSplStatusCode
        //{
        //    get { return _isEnabledStationNameForSplStatusCode; }
        //    set
        //    {
        //        if (_isEnabledStationNameForSplStatusCode != value)
        //        {
        //            _isEnabledStationNameForSplStatusCode = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}



      


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

       

      

        //private string _ad;
        //public string AD
        //{
        //    get => _ad;
        //    set
        //    {
        //        if (_ad != value)
        //        {
        //            _ad = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}
       
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

        //public List<string> StatusOptions { get; set; }

        //private int _statusIndex;
        //public int StatusIndex
        //{
        //    get => _statusIndex;
        //    set
        //    {
        //        if (_statusIndex != value)
        //        {
        //            _statusIndex = value;
        //            OnPropertyChanged(nameof(StatusIndex));
        //            Status = StatusOptions[_statusIndex];
        //            OnPropertyChanged(nameof(Status));
        //        }
        //    }
        //}

        public ObservableCollection<string> StatusOptions { get; set; } = new ObservableCollection<string> { "A", "D" };

        private string _selectedStatusOption;

        public string SelectedStatusOption
        {
            get => _selectedStatusOption;
            set
            {
                _selectedStatusOption = value;
                OnPropertyChanged(nameof(SelectedStatusOption));
                //OnPropertyChangedforLoading("StatusByte");

                //When status option delection is changed then StatusByte will also change.
                if (SelectedADOption == "A")
                {
                    if (_selectedStatusOption == "Running Right Time")
                    {
                        StatusByte = 0x01;
                    }
                    else if (_selectedStatusOption == "Will Arrive Shortly")
                    {
                        StatusByte = 0x02;
                    }
                    else if (_selectedStatusOption == "Is Arriving On")
                    {
                        StatusByte = 0x03;
                    }
                    else if (_selectedStatusOption == "Has Arrived On")
                    {
                        StatusByte = 0x04;
                    }
                    else if (_selectedStatusOption == "Running Late")
                    {
                        StatusByte = 0x05;
                    }
                    else if (_selectedStatusOption == "Cancelled")
                    {
                        StatusByte = 0x06;
                    }
                    else if (_selectedStatusOption == "Indefinite Late")
                    {
                        StatusByte = 0x07;
                    }
                    else if (_selectedStatusOption == "Terminated At")
                    {
                        StatusByte = 0x08;
                    }
                    else if (_selectedStatusOption == "Platform Changed")
                    {
                        StatusByte = 0x09;
                    }
                    else
                    {
                        throw new Exception(message: $"Unknown status message: ");
                    }

                }

                else if (SelectedADOption == "D")
                {
                    if (_selectedStatusOption == "Running Right Time")
                    {
                        StatusByte = 0x0A;
                    }
                    else if (_selectedStatusOption == "Cancelled")
                    {
                        StatusByte = 0x0B;
                    }
                    else if (_selectedStatusOption == "Is Ready To Leave")
                    {
                        StatusByte = 0x0C;
                    }
                    else if (_selectedStatusOption == "Is On Platform")
                    {
                        StatusByte = 0x0D;
                    }
                    else if (_selectedStatusOption == "Departed")
                    {
                        StatusByte = 0x0E;
                    }
                    else if (_selectedStatusOption == "Rescheduled")
                    {
                        StatusByte = 0x0F;
                    }
                    else if (_selectedStatusOption == "Diverted")
                    {
                        StatusByte = 0x10;
                    }
                    else if (_selectedStatusOption == "Delayed")
                    {
                        StatusByte = 0x11;
                    }
                    else if (_selectedStatusOption == "Platform Changed")
                    {
                        StatusByte = 0x12;
                    }
                    else if (_selectedStatusOption == "Change Of Source")
                    {
                        StatusByte = 0x13;
                    }
                    else
                    {
                        throw new Exception(message: $"Unknown status message: ");
                    }

                }

            }
        }


        //public List<string> ADOptions { get; set; } = new List<string> { "A", "D" };

        //private int _adIndex;
        //public int ADIndex
        //{
        //    get => _adIndex;
        //    set
        //    {
        //        if (_adIndex != value)
        //        {
        //            _adIndex = value;
        //            OnPropertyChanged(nameof(ADIndex));
        //            AD = ADOptions[_adIndex];
        //            OnPropertyChanged(nameof(AD));
        //            UpdateStatusOptions();
        //        }
        //    }
        //}


        public ObservableCollection<string> ADOptions { get; set; } = new ObservableCollection<string> { "A", "D" };

        private string _selectedADOption;
        public string SelectedADOption
        {
            get => _selectedADOption;
            set
            {
                if (_selectedADOption != value)
                {
                    _selectedADOption = value;
                    OnPropertyChanged(nameof(SelectedADOption));

                    UpdateStatusOptions();

                    if (value == "A")
                    {
                        ETA_Hours = STA_Hours + LateByHours;
                        ETA_Minutes = STA_Minutes + LateByMinutes;

                        ETD_Hours= STD_Hours + LateByHours;
                        ETD_Minutes = STD_Minutes + LateByMinutes;

                    }
                    else if(value== "D")
                    {
                        ETA_Hours = STA_Hours ;
                        ETA_Minutes = STA_Minutes ;

                        ETD_Hours = STD_Hours + LateByHours;
                        ETD_Minutes = STD_Minutes + LateByMinutes;

                    }

                    LateBy = new TimeSpan(LateByHours, LateByMinutes, 0);
                    ETA = new TimeSpan(ETA_Hours, ETA_Minutes, 0);
                    ETD = new TimeSpan(ETD_Hours, ETD_Minutes, 0);



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
                    _coachSequence = !string.IsNullOrWhiteSpace(value)
                                    ? value.Trim()
                                    : string.Empty;

                    // Convert List<string> to ObservableCollection<string>  
                    CoachListEnglish = new ObservableCollection<string>(_coachSequence.Split('-').ToList());

                    OnPropertyChanged();
                }
            }
        }

        //     CoachSequence = !string.IsNullOrWhiteSpace(_coachSequence)
        //? _coachSequence.Trim()
        //: !string.IsNullOrWhiteSpace(ntesTrain.ArrivalCoachPosition?.Trim())
        //    ? ntesTrain.ArrivalCoachPosition.Trim()
        //    : string.Empty;

        private ObservableCollection<string> _coachListEnglish;
        public ObservableCollection<string> CoachListEnglish
        {
            get => _coachListEnglish;
            set
            {
                if (_coachListEnglish != value)
                {
                    _coachListEnglish = value;
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
            if (SelectedADOption == "A")
            {
                StatusOptions = new ObservableCollection<string>
            {
                //"Select Status", // Default option
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
            else if (SelectedADOption == "D")
            {
                StatusOptions = new ObservableCollection<string>
            {
                //"Select Status", // Default option
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
                StatusOptions = new ObservableCollection<string> { "Select Status" }; // Fallback
            }
            SelectedStatusOption = StatusOptions.FirstOrDefault(); // Set default selection to the first item
            OnPropertyChanged(nameof(StatusOptions));
            OnPropertyChanged(nameof(SelectedStatusOption));
        }

        public ActiveTrain() 
        {
            CreatedTime = DateTime.Now;
            ModifiedTime = DateTime.Now;
           // CoachList = new ObservableCollection<string>();
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

            //AD = ntesTrain.ADFlag;
            SelectedADOption = ntesTrain.ADFlag;
            //ADIndex = ADOptions.IndexOf(AD);
            SelectedADOption = ntesTrain.ADFlag;

            UpdateStatusOptions();

            Status = ntesTrain.TrainStatus;
            SelectedStatusOption = ntesTrain.TrainStatus;
            //StatusIndex = 0;
            
            //for (int i = 0; i < StatusOptions.Count; i++)
            //{
            //    if (StatusOptions[i].IndexOf(Status, StringComparison.OrdinalIgnoreCase) >= 0)
            //    {
            //        StatusIndex = i;
            //        break;
            //    }
            //}

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

            //CoachList = CoachSequence.Split('-').ToList();
           // CoachList = new ObservableCollection<string>(_coachSequence.Split('-').ToList());
           //comeback

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
            CoachListEnglish = train.CoachListEnglish != null ? new ObservableCollection<string>(train.CoachListEnglish) : new ObservableCollection<string>();

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

                STA_Hours = this.STA_Hours,
                STA_Minutes = this.STA_Minutes,
                STD_Hours = this.STD_Hours,
                STD_Minutes = this.STD_Minutes,
                //AD = this.AD,
                _selectedADOption = this.SelectedADOption,
                Status = this.Status,
                StatusOptions = this.StatusOptions != null ? new ObservableCollection<string>(this.StatusOptions) : new ObservableCollection<string>(), // Handle null list
                                                                                                                                                        // StatusIndex = this.StatusIndex,
                SelectedStatusOption = this.SelectedStatusOption,
                ADOptions = this.ADOptions != null ? new ObservableCollection<string>(this.ADOptions) : new ObservableCollection<string>(), // Handle null list
                //ADIndex = this.ADIndex,
                SelectedADOption = this.SelectedADOption,
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
              //  CoachList = this.CoachList != null ? new ObservableCollection<string>(this.CoachList) : new ObservableCollection<string>(), // Handle null list
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
            STA_Hours = source.STA_Hours;//custom addition
            STA_Minutes = source.STA_Minutes; //custom addition
            STD_Hours = source.STD_Hours;
            STD_Minutes = source.STD_Minutes;
            STA_TS = source.STA_TS;
            STD = source.STD;
            STD_TS = source.STD_TS;
            //AD = source.AD;
            SelectedADOption = source.SelectedADOption; 
            Status = source.Status;
            StatusOptions = source.StatusOptions != null ? new ObservableCollection<string>(source.StatusOptions) : new ObservableCollection<string>(); // Deep copy with null check
            //StatusIndex = source.StatusIndex;
            SelectedStatusOption = source.SelectedStatusOption; // Assuming you want to copy the selected status option
            ADOptions = source.ADOptions != null ? new ObservableCollection<string>(source.ADOptions) : new ObservableCollection<string>(); // Deep copy with null check
            // ADIndex = source.ADIndex;
            SelectedADOption = source.SelectedADOption; // Assuming you want to copy the selected AD option
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
            //comeback
       //     CoachList = source.CoachList != null ? new ObservableCollection<string>(source.CoachList) : new ObservableCollection<string>(); // Deep copy with null check
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
