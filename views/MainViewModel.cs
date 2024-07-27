using IpisCentralDisplayController.Managers;
using IpisCentralDisplayController.models;
using IpisCentralDisplayController.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace IpisCentralDisplayController.views
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public TrainListViewModel TrainListVM { get; set; }
        public ColorViewModel ColorVM { get; set; }

        private DisplayStyle _selectedStyle;

        public ObservableCollection<DisplayStyle> DisplayStyles { get; set; }

        public DisplayStyle SelectedStyle
        {
            get { return _selectedStyle; }
            set
            {
                if (_selectedStyle != value)
                {
                    _selectedStyle = value;
                    OnPropertyChanged();
                    UpdateTestString();
                }
            }
        }

        private void UpdateTestString()
        {
            if (SelectedStyle != null)
            {
                SelectedStyle.TestString = GetTestStringForLanguage(SelectedStyle.Language);
            }
        }

        private string GetTestStringForLanguage(RegionalLanguage language)
        {
            return language switch
            {
                RegionalLanguage.ENGLISH => "Indian Railways",
                RegionalLanguage.HINDI => "भारतीय रेल",
                RegionalLanguage.ASSAMESE => "ভাৰতীয় ৰেলৱে",
                RegionalLanguage.BANGLA => "ভারতীয় রেলওয়ে",
                RegionalLanguage.BODO => "--", // Bodo script representation
                RegionalLanguage.DOGRI => "भारतीय रेलवे",
                RegionalLanguage.GUJARATI => "ભારતીય રેલ્વે",
                RegionalLanguage.KANNADA => "ಭಾರತೀಯ ರೈಲ್ವೆ",
                RegionalLanguage.KASHMIRI => "ہندوستانی ریلوے",
                RegionalLanguage.KONKANI => "भारतीय रेल्वे",
                RegionalLanguage.MALAYALAM => "ഇന്ത്യൻ റെയിൽവേ",
                RegionalLanguage.MARATHI => "भारतीय रेल्वे",
                RegionalLanguage.MANIPURI => "ꯏꯟꯗꯤꯌꯟ ꯔꯦꯂꯋꯦꯖꯒꯤ ꯗꯥ",
                RegionalLanguage.NEPALI => "भारतीय रेलवे",
                RegionalLanguage.ODIA => "ଭାରତୀୟ ରେଳ",
                RegionalLanguage.PUNJABI => "ਭਾਰਤੀ ਰੇਲਵੇ",
                RegionalLanguage.SANSKRIT => "भारतीय रेलवे",
                RegionalLanguage.SANTHALI => "Bharot disom reak̕ rel gạḍi",
                RegionalLanguage.SINDHI => "هندستاني ريلوي",
                RegionalLanguage.TAMIL => "இந்திய ரயில்வே",
                RegionalLanguage.TELUGU => "భారతీయ రైల్వేలు",
                RegionalLanguage.URDU => "ہندوستانی ریلوے",
                _ => "Unknown Language"
            };
        }


        public ICommand SaveCommand { get; set; }

        private ObservableCollection<UserCategory> _userCategories;
        public ObservableCollection<UserCategory> UserCategories
        {
            get { return _userCategories; }
            set
            {
                if (_userCategories != value)
                {
                    _userCategories = value;
                    OnPropertyChanged();
                }
            }
        }

        public void LoadUserCategories(List<UserCategory> userCategories)
        {
            UserCategories.Clear();
            foreach (var category in userCategories)
            {
                UserCategories.Add(category);
            }
        }

        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get { return _users; }
            set
            {
                if (_users != value)
                {
                    _users = value;
                    OnPropertyChanged();
                }
            }
        }

        public void LoadUsers(List<User> users)
        {
            Users.Clear();
            foreach (var user in users)
            {
                Users.Add(user);
            }
        }

        private Platform _selectedPlatform;
        private Device _selectedDevice;

        public ObservableCollection<Platform> Platforms { get; set; }
        public ObservableCollection<Device> Devices { get; set; }

        public Platform SelectedPlatform
        {
            get { return _selectedPlatform; }
            set
            {
                if (_selectedPlatform != value)
                {
                    _selectedPlatform = value;
                    OnPropertyChanged();
                    LoadDevices();
                }
            }
        }

        public Device SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                if (_selectedDevice != value)
                {
                    _selectedDevice = value;
                    OnPropertyChanged();
                }
            }
        }

        private void LoadDevices()
        {            
            if (SelectedPlatform != null)
            {
                Devices.Clear();
                foreach (var device in SelectedPlatform.Devices)
                {
                    Devices.Add(device);
                }
            }
        }

        private AudioSettings _audioSettings;
        private ObservableCollection<string> _micInterfaces;
        private ObservableCollection<string> _monitorInterfaces;
        private ObservableCollection<string> _audioOutInterfaces;

        public AudioSettings AudioSettings
        {
            get => _audioSettings;
            set
            {
                if (_audioSettings != value)
                {
                    _audioSettings = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<string> MicInterfaces
        {
            get => _micInterfaces;
            set
            {
                _micInterfaces = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> MonitorInterfaces
        {
            get => _monitorInterfaces;
            set
            {
                _monitorInterfaces = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> AudioOutInterfaces
        {
            get => _audioOutInterfaces;
            set
            {
                _audioOutInterfaces = value;
                OnPropertyChanged();
            }
        }

        private RmsSettings _rmsSettings;
        private RmsSettingsManager _rmsSettingsManager;

        public RmsSettings RmsSettings
        {
            get => _rmsSettings;
            set
            {
                if (_rmsSettings != value)
                {
                    _rmsSettings = value;
                    OnPropertyChanged();
                }
            }
        }

        private ColorDisplayTheme _theme;
        private ObservableCollection<TrainDisplayTemplate> _trainTemplates;

        public ColorDisplayTheme Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TrainDisplayTemplate> TrainTemplates
        {
            get => _trainTemplates;
            set
            {
                _trainTemplates = value;
                OnPropertyChanged();
            }
        }


        public MainViewModel()
        {
            UserCategories = new ObservableCollection<UserCategory>();
            Users = new ObservableCollection<User>();

            Platforms = new ObservableCollection<Platform>();
            Devices = new ObservableCollection<Device>();

            TrainListVM = new TrainListViewModel();
            ColorVM = new ColorViewModel();

            // Initialize the collection and commands
            DisplayStyles = new ObservableCollection<DisplayStyle>();
            //SaveCommand = new RelayCommand(Save);

            MicInterfaces = new ObservableCollection<string>();
            MonitorInterfaces = new ObservableCollection<string>();
            AudioOutInterfaces = new ObservableCollection<string>();

            RmsSettings = new RmsSettings();

            TrainTemplates = new ObservableCollection<TrainDisplayTemplate>();
        }

        public void LoadRmsSettings(RmsSettings rmsSettings)
        {
            RmsSettings = rmsSettings;
        }   

        public void RefreshAudioInterfaces(AudioSettingsManager audioSettingsManager)
        {
            MicInterfaces.Clear();
            MonitorInterfaces.Clear();
            AudioOutInterfaces.Clear();

            var interfaces = audioSettingsManager.RefreshInterfaces();

            foreach (var audioInterface in interfaces)
            {
                MicInterfaces.Add(audioInterface);
                MonitorInterfaces.Add(audioInterface);
                AudioOutInterfaces.Add(audioInterface);
            }
        }

        public void LoadAudioSettings(AudioSettings audioSettings)
        {
            AudioSettings = audioSettings;
        }

        public void SaveAudioSettings(AudioSettingsManager audioSettingsManager)
        {
            audioSettingsManager.SaveAudioSettings(AudioSettings);
        }

        public void RefreshAudioSettings(AudioSettingsManager audioSettingsManager)
        {
            AudioSettings = audioSettingsManager.LoadAudioSettings();
        }

        public void StartMonitorAudioTest(AudioSettingsManager audioSettingsManager)
        {
            //audioSettingsManager.StartAudioTest(AudioSettings);
        }

        public void StopMonitorAudioTest(AudioSettingsManager audioSettingsManager)
        {
            //audioSettingsManager.StopAudioTest();
        }

        private void Save()
        {
            // Implement save logic here
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
