using IpisCentralDisplayController.Managers;
using IpisCentralDisplayController.models;
using IpisCentralDisplayController.Models;
using System;
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

        private RmsServerSettings _rmsSettings;

        public RmsServerSettings RmsSettings
        {
            get => _rmsSettings;
            set
            {
                if (_rmsSettings != value)
                {
                    _rmsSettings = value;
                    OnPropertyChanged(nameof(RmsSettings));
                }
            }
        }

        private CAPServerSettings _capSettings;
        public CAPServerSettings CapSettings
        {
            get => _capSettings;
            set
            {
                if (_capSettings != value)
                {
                    _capSettings = value;
                    OnPropertyChanged(nameof(CapSettings));
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

        private ObservableCollection<MediaFile> _mediaFiles;
        public ObservableCollection<MediaFile> MediaFiles
        {
            get { return _mediaFiles; }
            set
            {
                if (_mediaFiles != value)
                {
                    _mediaFiles = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<ImageFile> _imageFiles;
        public ObservableCollection<ImageFile> ImageFiles
        {
            get { return _imageFiles; }
            set
            {
                if (_imageFiles != value)
                {
                    _imageFiles = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<TextSlideFile> _textSlideFiles;
        public ObservableCollection<TextSlideFile> TextSlideFiles
        {
            get { return _textSlideFiles; }
            set
            {
                if (_textSlideFiles != value)
                {
                    _textSlideFiles = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<VideoFile> _videoFiles;
        public ObservableCollection<VideoFile> VideoFiles
        {
            get { return _videoFiles; }
            set
            {
                if (_videoFiles != value)
                {
                    _videoFiles = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<AudioFile> _audioFiles;
        public ObservableCollection<AudioFile> AudioFiles
        {
            get { return _audioFiles; }
            set
            {
                if (_audioFiles != value)
                {
                    _audioFiles = value;
                    OnPropertyChanged();
                }
            }
        }

        //public void LoadMediaFiles(List<MediaFile> mediaFiles)
        //{
        //    MediaFiles.Clear();
        //    foreach (var mediaFile in mediaFiles)
        //    {
        //        MediaFiles.Add(mediaFile);
        //    }
        //}

        public void LoadMediaFiles(List<MediaFile> mediaFiles)
        {
            MediaFiles.Clear();
            ImageFiles.Clear();
            TextSlideFiles.Clear();
            VideoFiles.Clear();
            AudioFiles.Clear();

            foreach (var mediaFile in mediaFiles)
            {
                MediaFiles.Add(mediaFile);

                switch (mediaFile)
                {
                    case ImageFile imageFile:
                        ImageFiles.Add(imageFile);
                        break;
                    case TextSlideFile textSlideFile:
                        TextSlideFiles.Add(textSlideFile);
                        break;
                    case VideoFile videoFile:
                        VideoFiles.Add(videoFile);
                        break;
                    case AudioFile audioFile:
                        AudioFiles.Add(audioFile);
                        break;
                }
            }
        }

        private int _ivdOvdHeight = 256;
        public int IvdOvdHeight
        {
            get => _ivdOvdHeight;
            set
            {
                _ivdOvdHeight = value;
                OnPropertyChanged();
                UpdatePixelDimensions();
            }
        }

        private ObservableCollection<Timeline> _timelines;
        public ObservableCollection<Timeline> Timelines
        {
            get { return _timelines; }
            set
            {
                if (_timelines != value)
                {
                    _timelines = value;
                    OnPropertyChanged();
                }
            }
        }

        private Timeline _selectedTimeline;
        public Timeline SelectedTimeline
        {
            get { return _selectedTimeline; }
            set
            {
                if (_selectedTimeline != value)
                {
                    _selectedTimeline = value;
                    OnPropertyChanged();
                    LoadTimelineItems();
                }
            }
        }

        private TimelineItem _selectedTimelineItem;
        public TimelineItem SelectedTimelineItem
        {
            get { return _selectedTimelineItem; }
            set
            {
                if (_selectedTimelineItem != value)
                {
                    _selectedTimelineItem = value;
                    OnPropertyChanged();
                }
            }
        }


        private ObservableCollection<TimelineItem> _timelineItems;
        public ObservableCollection<TimelineItem> TimelineItems
        {
            get { return _timelineItems; }
            set
            {
                if (_timelineItems != value)
                {
                    _timelineItems = value;
                    OnPropertyChanged();
                }
            }
        }

        public void LoadTimelines(List<Timeline> timelines)
        {
            Timelines.Clear();
            foreach (var timeline in timelines)
            {
                Timelines.Add(timeline);
            }

            if (Timelines.Any())
            {
                SelectedTimeline = Timelines.First();
            }
        }

        private void LoadTimelineItems()
        {
            if (SelectedTimeline != null)
            {
                TimelineItems.Clear();
                foreach (var item in SelectedTimeline.Items)
                {
                    TimelineItems.Add(item);
                }
            }
        }

        private int _pixelWidth;
        public int PixelWidth
        {
            get => _pixelWidth;
            set
            {
                if (_pixelWidth != value)
                {
                    _pixelWidth = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _pixelHeight;
        public int PixelHeight
        {
            get => _pixelHeight;
            set
            {
                if (_pixelHeight != value)
                {
                    _pixelHeight = value;
                    OnPropertyChanged();
                }
            }
        }

        private void UpdatePixelDimensions()
        {
            switch (IvdOvdHeight)
            {
                case 0: // 6-line
                    PixelWidth = 128;
                    PixelHeight = 64;
                    break;
                case 1: // 12-line
                    PixelWidth = 256;
                    PixelHeight = 128;
                    break;
                case 2: // 18-line
                    PixelWidth = 384;
                    PixelHeight = 192;
                    break;
                case 3: // LED-TV HD
                    PixelWidth = 1920;
                    PixelHeight = 1080;
                    break;
                case 4: // Custom
                        // Keep the current values or set them based on user input.
                        // PixelWidth and PixelHeight should remain editable in this case.
                    break;
                default:
                    PixelWidth = 128; // Default values (fall-back case)
                    PixelHeight = 64;
                    break;
            }

            //// If the selected option is not "Custom," make sure the values are read-only.
            //if (IvdOvdHeight != 4)
            //{
            //    // Assume PixelWidthControl and PixelHeightControl are your IntegerUpDown controls
            //    PixelWidthControl.IsReadOnly = true;
            //    PixelHeightControl.IsReadOnly = true;
            //}
            //else
            //{
            //    PixelWidthControl.IsReadOnly = false;
            //    PixelHeightControl.IsReadOnly = false;
            //}
        }

        public bool IsCustomResolution
        {
            get { return IvdOvdHeight == 4; } // Assuming 4 is the index for Custom
        }

        private ObservableCollection<ActiveTrain> _activeTrains;

        public ObservableCollection<ActiveTrain> ActiveTrains
        {
            get => _activeTrains;
            set
            {
                if (_activeTrains != value)
                {
                    _activeTrains = value;
                    OnPropertyChanged(nameof(ActiveTrains));
                }
            }
        }

        public void LoadActiveTrains(List<ActiveTrain> activeTrains)
        {
            if (activeTrains == null || !activeTrains.Any())
            {
                ActiveTrains.Clear();
                return;
            }

            ActiveTrains.Clear(); // Clear the existing collection

            foreach (var train in activeTrains)
            {
                ActiveTrains.Add(train); // Add each train to the ObservableCollection
            }

            OnPropertyChanged(nameof(ActiveTrains)); // Notify the UI that the collection has changed
        }

        private ActiveTrain _selectedTrain;
        public ActiveTrain SelectedTrain
        {
            get => _selectedTrain;
            set
            {
                if (_selectedTrain != value)
                {
                    _selectedTrain = value;
                    OnPropertyChanged(nameof(SelectedTrain));
                }
            }
        }

        private ObservableCollection<EventLog> _eventLogs;
        public ObservableCollection<EventLog> EventLogs
        {
            get { return _eventLogs; }
            set
            {
                if (_eventLogs != value)
                {
                    _eventLogs = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainViewModel()
        {
            UserCategories = new ObservableCollection<UserCategory>();
            Users = new ObservableCollection<User>();

            Platforms = new ObservableCollection<Platform>();
            Devices = new ObservableCollection<Device>();

            ColorVM = new ColorViewModel();

            // Initialize the collection and commands
            DisplayStyles = new ObservableCollection<DisplayStyle>();
            //SaveCommand = new RelayCommand(Save);

            MicInterfaces = new ObservableCollection<string>();
            MonitorInterfaces = new ObservableCollection<string>();
            AudioOutInterfaces = new ObservableCollection<string>();            

            TrainTemplates = new ObservableCollection<TrainDisplayTemplate>();

            MediaFiles = new ObservableCollection<MediaFile>();
            ImageFiles = new ObservableCollection<ImageFile>();
            TextSlideFiles = new ObservableCollection<TextSlideFile>();
            VideoFiles = new ObservableCollection<VideoFile>();
            AudioFiles = new ObservableCollection<AudioFile>();

            Timelines = new ObservableCollection<Timeline>();
            TimelineItems = new ObservableCollection<TimelineItem>();

            _activeTrains = new ObservableCollection<ActiveTrain>();

            _eventLogs = new ObservableCollection<EventLog>();
        }

        public void UpdateActiveTrains(List<ActiveTrain> trains)
        {
            ActiveTrains.Clear();

            foreach (var train in trains)
            {
                ActiveTrains.Add(train);
            }
        }

        public void LoadRmsSettings(RmsServerSettings rmsSettings)
        {
            RmsSettings = rmsSettings ?? new RmsServerSettings();
        }

        public void LoadCAPServerSettings(CAPServerSettings capsSettings)
        {
            CapSettings = capsSettings ?? new CAPServerSettings(); 
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

        public void LoadEventLogs(List<EventLog> logs)
        {
            EventLogs.Clear();
            foreach (var log in logs)
            {
                EventLogs.Add(log);
            }
        }

        public void LoadFilteredEventLogs(List<EventLog> filteredLogs)
        {
            EventLogs.Clear();
            foreach (var log in filteredLogs)
            {
                EventLogs.Add(log);
            }
        }

        public void LoadLogsFromDate(List<EventLog> allLogs, DateTime fromDate, bool isDescending, int numberOfLogs)
        {
            var filteredLogs = allLogs
                .Where(log => log.Timestamp >= fromDate) // Filter logs from the specific date
                .OrderBy(log => log.Timestamp) // Order by Timestamp (ascending by default)
                .Take(numberOfLogs); // Take the last N logs

            if (isDescending)
            {
                filteredLogs = filteredLogs.OrderByDescending(log => log.Timestamp); // If descending, change the order
            }

            EventLogs.Clear();
            foreach (var log in filteredLogs)
            {
                EventLogs.Add(log);
            }
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
