using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace IpisCentralDisplayController.models
{
    public class Device: INotifyPropertyChanged
    {
        //public int Id { get; set; }
        //public DateTime Created { get; set; }
        //public DateTime Updated { get; set; }
        //public DeviceType DeviceType { get; set; }
        //public string Description { get; set; }
        //public string IpAddress { get; set; }
        //public bool Status { get; set; }
        //public DateTime LastStatusWhen { get; set; }
        //public int? NumOfLines { get; set; }


        //public byte SpeedByte { get; set; }
        //public byte EffectByte { get; set; }
        //public byte LetterSizeByte { get; set; }
        //public byte GapByte { get; set; }
        //public byte IntensityByte { get; set; }
        //public byte TimeDelayValueByte { get; set; }
        //public byte DataTimeoutValueByte { get; set; }


        //public bool IsReverseVideo { get; set; }
        //public bool IsEnabled { get; set; }


        private int _id;
        public int Id
        {
            get => _id;
            set { _id = value;
                OnPropertyChanged();
            }
        }

        private DateTime _created;
        public DateTime Created
        {
            get => _created;
            set { _created = value; OnPropertyChanged(); }
        }

        private DateTime _updated;
        public DateTime Updated
        {
            get => _updated;
            set { _updated = value; OnPropertyChanged(); }
        }

        private DeviceType _deviceType;
        public DeviceType DeviceType
        {
            get => _deviceType;
            set { _deviceType = value; OnPropertyChanged(); }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        private string _ipAddress;
        public string IpAddress
        {
            get => _ipAddress;
            set { _ipAddress = value; OnPropertyChanged(); }
        }

        private bool _status;
        public bool Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        private DateTime _lastStatusWhen;
        public DateTime LastStatusWhen
        {
            get => _lastStatusWhen;
            set { _lastStatusWhen = value; OnPropertyChanged(); }
        }

        private int? _numOfLines;
        public int? NumOfLines
        {
            get => _numOfLines;
            set { _numOfLines = value; OnPropertyChanged(); }
        }

        private byte _speedByte;
        public byte SpeedByte
        {
            get => _speedByte;
            set { _speedByte = value; OnPropertyChanged(); }
        }

        private byte _effectByte;
        public byte EffectByte
        {
            get => _effectByte;
            set { _effectByte = value; OnPropertyChanged(); }
        }

        private byte _letterSizeByte;
        public byte LetterSizeByte
        {
            get => _letterSizeByte;
            set { _letterSizeByte = value; OnPropertyChanged(); }
        }

        private byte _gapByte;
        public byte GapByte
        {
            get => _gapByte;
            set { _gapByte = value; OnPropertyChanged(); }
        }

        private byte _intensityByte;
        public byte IntensityByte
        {
            get => _intensityByte;
            set { _intensityByte = value; OnPropertyChanged(); }
        }

        private byte _timeDelayValueByte;
        public byte TimeDelayValueByte
        {
            get => _timeDelayValueByte;
            set { _timeDelayValueByte = value; OnPropertyChanged(); }
        }

        private byte _dataTimeoutValueByte;
        public byte DataTimeoutValueByte
        {
            get => _dataTimeoutValueByte;
            set { _dataTimeoutValueByte = value; OnPropertyChanged(); }
        }

        private bool _isReverseVideo;
        public bool IsReverseVideo
        {
            get => _isReverseVideo;
            set { _isReverseVideo = value; OnPropertyChanged(); }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set { _isEnabled = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
