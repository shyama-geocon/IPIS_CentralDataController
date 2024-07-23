using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using IpisCentralDisplayController.ntes;
using Newtonsoft.Json;
using Xceed.Wpf.Toolkit;

namespace IpisCentralDisplayController
{
    public class TrainViewModel : INotifyPropertyChanged
    {
        private readonly NtesTrain951 _train;

        public TrainViewModel(NtesTrain951 train)
        {
            _train = train;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string TrainNo
        {
            get => _train.TrainNo;
            set
            {
                if (_train.TrainNo != value)
                {
                    _train.TrainNo = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TrainName
        {
            get => _train.TrainName;
            set
            {
                if (_train.TrainName != value)
                {
                    _train.TrainName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ADFlag
        {
            get => _train.ADFlag;
            set
            {
                if (_train.ADFlag != value)
                {
                    _train.ADFlag = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TrainStatus
        {
            get => _train.TrainStatus;
            set
            {
                if (_train.TrainStatus != value)
                {
                    _train.TrainStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? STA
        {
            get => _train.STA;
            set
            {
                if (_train.STA != value)
                {
                    _train.STA = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? STD
        {
            get => _train.STD;
            set
            {
                if (_train.STD != value)
                {
                    _train.STD = value;
                    OnPropertyChanged();
                }
            }
        }

        public TimeSpan? Late
        {
            get => _train.DelayArr != null ? TimeSpan.Parse(_train.DelayArr) : (TimeSpan?)null;
            set
            {
                if (_train.DelayArr != value?.ToString())
                {
                    _train.DelayArr = value?.ToString();
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? ETA
        {
            get => _train.ETA;
            set
            {
                if (_train.ETA != value)
                {
                    _train.ETA = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? ETD
        {
            get => _train.ETD;
            set
            {
                if (_train.ETD != value)
                {
                    _train.ETD = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PlatformNo
        {
            get => _train.PlatformNo;
            set
            {
                if (_train.PlatformNo != value)
                {
                    _train.PlatformNo = value;
                    OnPropertyChanged();
                }
            }
        }

        //public bool TADDB
        //{
        //    get => _train.TADDB;
        //    set
        //    {
        //        if (_train.TADDB != value)
        //        {
        //            _train.TADDB = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        //public bool CGDB
        //{
        //    get => _train.CGDB;
        //    set
        //    {
        //        if (_train.CGDB != value)
        //        {
        //            _train.CGDB = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        //public bool ANN
        //{
        //    get => _train.ANN;
        //    set
        //    {
        //        if (_train.ANN != value)
        //        {
        //            _train.ANN = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
    }
}
