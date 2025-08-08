    using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace IpisCentralDisplayController.models
    {
        public class Platform: INotifyPropertyChanged
        {
            //public string PlatformNumber { get; set; }
            //public PlatformType PlatformType { get; set; }
            //public string Description { get; set; }
            //public string Subnet { get; set; }
            //public List<Device> Devices { get; set; } = new List<Device>();


        private string _platformNumber;
        public string PlatformNumber
        {
            get { return _platformNumber; }
            set { 
                _platformNumber = value;
                OnPropertyChanged();
            }
        }

        private PlatformType _platformType;
        public PlatformType PlatformType
        {
            get { return _platformType; }
            set
            {
                _platformType = value;
                OnPropertyChanged();
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }


        private string _subnet;
        public string Subnet
        {
            get { return _subnet; }
            set {
                _subnet = value;
                OnPropertyChanged();
            }
        }


        private ObservableCollection<Device> _devices;

        public ObservableCollection<Device> Devices
        {
            get { return _devices; }
            set {
                _devices = value;
                OnPropertyChanged();
            }
        }

        public Platform()
        {
            Devices = new ObservableCollection<Device>();
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
    }
