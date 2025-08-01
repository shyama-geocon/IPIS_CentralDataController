using CommunityToolkit.Mvvm.Input;
using IpisCentralDisplayController.commands;
using IpisCentralDisplayController.Managers;
using IpisCentralDisplayController.models;
using IpisCentralDisplayController.models.DisplayCommunication;
using IpisCentralDisplayController.Models;
using IpisCentralDisplayController.services.DisplayCommunicationServices;
using MongoDB.Driver.Core.Servers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;
using Microsoft.Extensions.Logging.Internal;
using IpisCentralDisplayController.services.Announcement;
using IpisCentralDisplayController.managers;
using IpisCentralDisplayController.models.Announcement;
using static System.Windows.Forms.Design.AxImporter;
using IpisCentralDisplayController.Helpers;

namespace IpisCentralDisplayController.views
{

    #region SplStatus ComboBox Options

    public class SplStatusOptionItem 
    {
        public string OptionString { get; set; }
        public string StationCode { get; set; }

    }

   #endregion

    public class MainViewModel : INotifyPropertyChanged
    {

        #region AudioAnnouncements

        private int _repeatAnnouncement;
        public int RepeatAnnouncement
        {
            get { return _repeatAnnouncement; }
            set { _repeatAnnouncement = value;
                OnPropertyChanged();
            }
        }

        AudioPlayerService audioPlayerService;
        List<AudioElement> audioPacket = new List<AudioElement>();      

        private AnnFormat _selectedFormat;
        public AnnFormat SelectedFormat
        {
            get { return _selectedFormat; }
            set { _selectedFormat = value; }
        }


        public commands.RelayCommand PlayPauseAnnCommand { get; }
        public commands.RelayCommand StopAnnCommand { get; }

        //  public IAsyncRelayCommand PlayPauseAnnCommand => new AsyncRelayCommand(PlayPauseAnn);

        //StopAnnCommand = new commands.RelayCommand(PlayPauseAnn, CanExecute_StopAnnouncement);

        //private async Task PlayPauseAnn() //MAKE ASYNC JUST LIKE TADDB SET
        private void  PlayPauseAnn(object parameter) //MAKE ASYNC JUST LIKE TADDB SET
        {
            if (PlayPauseToolTip == "Play")
            {

                PlayAudio = true;
               //  PauseAudio = false;
                if(PauseAudio == true) {

                    PauseAudio = false;
                }
                else if(PauseAudio == false) {
                    for (int i = 0; i < RepeatAnnouncement; i++)
                    {
                        foreach (ActiveTrain train in ActiveTrains)
                        {
                            if (train.Announce_Update == true)
                            {
                                audioPacket = audioPlayerService.AudioSequenceBuilder(train, WorkspaceManager.GetWorkspacePath());
                                // await audioPlayerService.PlayAudioSequenceAsync(audioPacket, AnnInProgress);
                                //await audioPlayerService.PlayAudioSequenceAsync(
                                //                                                 audioPacket,
                                //                                                 () => AnnInProgress,         // a method to get the current value
                                //                                                 value => AnnInProgress = value, // a method to set the value
                                //                                                 SelectedFormat,
                                //                                                 () => PauseAudio,         // a method to get the current value
                                //                                                 value => PauseAudio = value // a method to set the value

                                //                                               );
                                //https://chatgpt.com/c/68887230-f634-8013-84fa-376d32ba320c


                                Task.Run(() =>
                                audioPlayerService.PlayAudioSequenceAsync(
                                    audioPacket,
                                    SelectedFormat,

                                    () => PlayAudio,
                                    value => PlayAudio = value,

                                    () => PauseAudio,
                                    value => PauseAudio = value,

                                    () => StopAudio,
                                    value => StopAudio = value,

                                    () => MuteAudio,
                                    value => MuteAudio = value

                                    //() => MuteAudio,
                                    //value => PauseAudio = value

                                )
                            );


                            }

                            //AudioPlayerService.PlaySequenceAsync(AudioPlayerService.AudioSequenceBuilder(train, WorkspaceManager.GetWorkspacePath()));
                            //await AudioPlayerService.PlayAudioSequenceAsync(AudioPlayerService.AudioSequenceBuilder(train, WorkspaceManager.GetWorkspacePath()));
                            //await AudioPlayerService.PlayMp3Async("C:\\Users\\Public\\Music\\Sample Music\\Kalimba.mp3");
                        }

                    }

                }


            }

            else if (PlayPauseToolTip == "Pause")
            {
                //if(PlayAudio == )
                PlayPauseToolTip = "Play";
                PauseAudio= true;
                PlayAudio = false;



            }



}

        private void StopAnn(object parameter)
        {
            StopAudio = true;
            PlayAudio = false;
            PauseAudio = false;
            MuteAudio = false;


        }



        // When one action flag changes , be sure to change all the other action flags accordingly
        #region ActionsFlags 

        private bool _stopAudio=true;
        public bool StopAudio
        {
            get { return _stopAudio; }
            set { _stopAudio = value; }
        }

        private bool _playAudio =false;
        public bool PlayAudio
        {
            get { return _playAudio; }
            set {
                _playAudio = value;
                if (_playAudio)
                {
                    PlayPauseToolTip = "Pause";
                    AnnInProgress = true;
                }
                //else
                //{
                //    PlayPauseToolTip = "Play";
                //    AnnInProgress = false;
                //}
            }
        }


        private bool _pauseAudio = false;
        public bool PauseAudio
        {
            get { return _pauseAudio; }
            set {
                _pauseAudio = value;
                if(_pauseAudio)
                {
                    PlayPauseToolTip = "Play";
                }
                //else
                //{
                //    PlayPauseToolTip = "Pause";
                //}
            }
        }


        private bool _muteAudio = false;
        public bool MuteAudio
        {
            get { return _muteAudio; }
            set {
                _muteAudio = value;
                if (_muteAudio)
                {
                    MuteUnmuteToolTip = "Unmute";
                    AnnIsMute = true;
                }
                else
                {
                    MuteUnmuteToolTip = "Mute";
                    AnnIsMute = false;
                }
            }
        }



        #endregion


        #region State_NODIRECTSETTINGALWAYS SETFROMTHEACTION_FLAGS

        private bool _annInProgress = false;
        public bool AnnInProgress
        {
            get { return _annInProgress; }
            set { _annInProgress = value; }
        }

        private bool _annIsPaused = false;
        public bool AnnIsPaused
        {
            get { return _annIsPaused; }
            set { _annIsPaused = value; }
        }

        private bool _annIsMute = false;
        public bool AnnIsMute
        {
            get { return _annIsMute; }
            set { _annIsMute = value; }
        }


        #endregion


        #region ToolTips_NODIRECTSETTINGALWAYS SETFROMTHEACTION_FLAGS

        //  public string PlayPauseTooltip => IsPlaying ? "Pause" : "Play";

        private string _playPauseToolTip = "Play";
        public string PlayPauseToolTip
        {
            get { return _playPauseToolTip; }
            set { _playPauseToolTip = value;
                OnPropertyChanged(); // Notify property change for UI updates
                if (value == "Pause")
                {
                    Button1Icon = "&#xE769;"; // Pause icon
                }
                else if (value == "Play")
                {
                    Button1Icon = "&#xE768;";//Play
                }

            }
        }


        private string _muteUnmuteToolTip = "Mute" ;
        public string MuteUnmuteToolTip
        {
            get { return _muteUnmuteToolTip; }
            set { _muteUnmuteToolTip = value;
                OnPropertyChanged(); // Notify property change for UI updates
            }
        }


        #endregion


        #region Icon_NODIRECTSETTINGALWAYS SETFROMTHETOOLTIP

        private string _button1Icon= "&#xE768;";//Play
        public string Button1Icon
        {
            get { return _button1Icon; }
            set { 
                _button1Icon = value;
                OnPropertyChanged(); // Notify property change for UI updates

                
               

            }
        }

        //BUTOONS 2 IS STOP BUTTON WHICH ALWAYS HAS A FIXED ICON

        private string _button3Icon;
        public string Button3Icon
        {
            get { return _button3Icon; }
            set {
                _button3Icon = value; 
            }
        }


        #endregion

        //BUTTON LABELS ??? : USE SET ONLY PROPERTY

        #endregion

        #region SplStatusPopUp

        private ObservableCollection<SplStatusOptionItem> _stationOptionsHolder;
        public ObservableCollection<SplStatusOptionItem> StationOptionsHolder
        {
            get => _stationOptionsHolder;
            set { _stationOptionsHolder = value; OnPropertyChanged(); }
        }

        private void OpenPopupWindow(byte statusByte)
        {
            // Populate combo options based on statusCode
            switch (statusByte)
            {
                case 0x08:   //Terminated At
                             //  SplStatusOptions.Clear();

                    SelectedTrain.SplStatusOptions = new ObservableCollection<SplStatusOptionItem>();

                    //var jsonHelperAdapter = new SettingsJsonHelperAdapter();
                    //var _stationManager = new StationManager(jsonHelperAdapter);

                    //var stations = _stationManager.LoadStations();

                    //foreach (var station in stations)
                    //{
                    //    SplStatusOptionItem tempitem = new SplStatusOptionItem();
                    //    tempitem.OptionString = $"{station.StationCode}: {station.StationNameEnglish}";
                    //    tempitem.StationCode = station.StationCode;

                    //    SelectedTrain.SplStatusOptions.Add(tempitem);
                    //}

                    SelectedTrain.SplStatusOptions = StationOptionsHolder;

                    SelectedTrain.FieldForSplStatusCode = "Terminated At Station:";

                    break;

                //case 2:
                //    SplStatusOptions = new ObservableCollection<string> { "Option C", "Option D" };
                //    break;

                //default:
                //    SplStatusOptions = new ObservableCollection<string> { "Default 1", "Default 2" };
                //    break;
            }

            //var popup = new SplStatusPopUp
            //{
            //    DataContext = this
            //};

            //var popup = new SplStatusPopUp(this);
            
            //popup.ShowDialog();
        }

        #endregion



        private ActiveTrain _selectedTrain;
        public ActiveTrain SelectedTrain
        {
            get => _selectedTrain;
            set
            {
                if (_selectedTrain != value)
                {
                    if (_selectedTrain != null)
                    {
                        _selectedTrain.PropertyChanged -= OnCategoryPropertyChanged;
                    }

                    _selectedTrain = value;
                    OnPropertyChanged(nameof(SelectedTrain));

                    if (_selectedTrain != null)
                    {
                        _selectedTrain.PropertyChanged += OnCategoryPropertyChanged;
                    }
                }
            }
        }

        private void OnCategoryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedTrain.StatusByte))
            {
                // When StatusCode changes, open popup
                OpenPopupWindow(_selectedTrain.StatusByte);
            }
        }


        public MainViewModel()
        {
            #region DeviceCongigurationsOptions

            SpeedOptions = new ObservableCollection<ConfigOptionObject>
            {
                new ConfigOptionObject { DisplayName = "Lowest", ByteValue = 0x00 },
                new ConfigOptionObject { DisplayName = "Low", ByteValue = 0x01 },
                new ConfigOptionObject { DisplayName = "Medium", ByteValue = 0x02 },
                new ConfigOptionObject { DisplayName = "High", ByteValue = 0x03 },
                new ConfigOptionObject { DisplayName = "Highest", ByteValue = 0x04 }
            };

            EffectOptions = new ObservableCollection<ConfigOptionObject>
            {
                new ConfigOptionObject { DisplayName = "Reserved", ByteValue = 0x00 },
                new ConfigOptionObject { DisplayName = "Curtain Left to Right", ByteValue = 0x01 },
                new ConfigOptionObject { DisplayName = "Curtain Top to Bottom", ByteValue = 0x02 },
                new ConfigOptionObject { DisplayName = "Curtain Bottom to Top", ByteValue = 0x03 },
                new ConfigOptionObject { DisplayName = "Typing Left to Right ", ByteValue = 0x04 },
                new ConfigOptionObject { DisplayName = "Running Right to Left  ", ByteValue = 0x05 },
                new ConfigOptionObject { DisplayName = "Running Top to Bottom ", ByteValue = 0x06 },
                new ConfigOptionObject { DisplayName = "Running Bottom to Top", ByteValue = 0x07 },
                new ConfigOptionObject { DisplayName = "Flashing ", ByteValue = 0x08 },
                new ConfigOptionObject { DisplayName = "Stable / Static ", ByteValue = 0x09 },
            };

            LetterSizeOptions = new ObservableCollection<ConfigOptionObject>
            {
                new ConfigOptionObject { DisplayName = "7", ByteValue = 0x00 },
                new ConfigOptionObject { DisplayName = "8", ByteValue = 0x01 },
                new ConfigOptionObject { DisplayName = "10", ByteValue = 0x02 },
                new ConfigOptionObject { DisplayName = "12", ByteValue = 0x03 },
                new ConfigOptionObject { DisplayName = "14", ByteValue = 0x04 },
                new ConfigOptionObject { DisplayName = "16", ByteValue = 0x05 },
            };

            IntensityOptions = new ObservableCollection<ConfigOptionObject>
            {
                new ConfigOptionObject { DisplayName = "25% ", ByteValue = 0x01 },
                new ConfigOptionObject { DisplayName = "50% ", ByteValue = 0x02 },
                new ConfigOptionObject { DisplayName = "75% ", ByteValue = 0x03 },
                new ConfigOptionObject { DisplayName = "100% ", ByteValue = 0x04 },

            };

            GapOptions = new ObservableCollection<ConfigOptionObject>
            {
                new ConfigOptionObject { DisplayName = "1", ByteValue = 0x00 },
                new ConfigOptionObject { DisplayName = "2", ByteValue = 0x01 },
                new ConfigOptionObject { DisplayName = "3", ByteValue = 0x02 },
                new ConfigOptionObject { DisplayName = "4", ByteValue = 0x03 },
                new ConfigOptionObject { DisplayName = "5", ByteValue = 0x04 },
                new ConfigOptionObject { DisplayName = "6", ByteValue = 0x05 },
                new ConfigOptionObject { DisplayName = "7", ByteValue = 0x06 },
                new ConfigOptionObject { DisplayName = "8", ByteValue = 0x07 },
            };

            
            //SelectedEffectOption = EffectOptions.FirstOrDefault();
            //SelectedSpeedOption = SpeedOptions.FirstOrDefault();
            //SelectedLetterSizeOption = LetterSizeOptions.FirstOrDefault();
            //SelectedIntensityOption = IntensityOptions.FirstOrDefault();

            #endregion


            var jsonHelperAdapter = new SettingsJsonHelperAdapter();
            var _stationManager = new StationManager(jsonHelperAdapter);
            var stations = _stationManager.LoadStations();

            StationOptionsHolder = new ObservableCollection<SplStatusOptionItem>();
            foreach (var station in stations)
            {
                SplStatusOptionItem tempitem = new SplStatusOptionItem();
                tempitem.OptionString = $"{station.StationCode}: {station.StationNameEnglish}";
                tempitem.StationCode = station.StationCode;

                StationOptionsHolder.Add(tempitem);
            }


            //  PlayAnnCommand = new commands.RelayCommand(PlayAnnouncement, CanSaveConfiguration);
            audioPlayerService = new AudioPlayerService();

            SaveCommandTemp = new commands.RelayCommand(SaveConfiguration, CanSaveConfiguration);
            OpenCommandTemp = new commands.RelayCommand(LoadConfiguration);

            //StopAnnCommand = new commands.RelayCommand(StopAnnouncement, CanExecute_StopAnnouncement);
            //PauseAnnCommand = new commands.RelayCommand(PauseAnnouncement, CanExecute_PauseAnnouncement);

            PlayPauseAnnCommand = new commands.RelayCommand(PlayPauseAnn);
            StopAnnCommand = new commands.RelayCommand(StopAnn);
           

            _tcpClientService = new TcpClientService();
            //Results = new ObservableCollection<string>();
            Results = new ObservableCollection<byte>();
            _servers = new List<ServerConfig>();

            UserCategories = new ObservableCollection<UserCategory>();
            Users = new ObservableCollection<User>();

            Platforms = new ObservableCollection<Platform>();
            Devices = new ObservableCollection<Device>();

            ColorVM = new ColorViewModel();

            // Initialize the collection and commands
            DisplayStyles = new ObservableCollection<DisplayStyle>();
            //SaveCommand = new
            //(Save);

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

            FrameBuilderForPFDBService = new FrameBuilderForPFDB();
            //FrameBuilderForAGDBService = new FrameBuilderForAGDB();
            FrameBuilderForMultilineService = new FrameBuilderForMultiline();
           

        }


        #region DeviceCongigurationsOptions
        public ObservableCollection<ConfigOptionObject> SpeedOptions { get; set; }
        public ObservableCollection<ConfigOptionObject> EffectOptions { get; set; }
        public ObservableCollection<ConfigOptionObject> LetterSizeOptions { get; set; }
        public ObservableCollection<ConfigOptionObject> IntensityOptions { get; set; }
        public ObservableCollection<ConfigOptionObject> GapOptions { get; set; }

        private ConfigOptionObject _selectedSpeedOption;
        public ConfigOptionObject SelectedSpeedOption
        {
            get { return _selectedSpeedOption; }
            set { 
                _selectedSpeedOption = value;
                OnPropertyChanged();
            }
        }

      
        private ConfigOptionObject _selectedEffectOption;
        public ConfigOptionObject SelectedEffectOption 
        {
            get { return _selectedEffectOption; }
            set {
                _selectedEffectOption = value;
                OnPropertyChanged();
            }
        }

       
        private ConfigOptionObject _selectedLetterSizeOption;
        public ConfigOptionObject SelectedLetterSizeOption  
        {
            get { return _selectedLetterSizeOption; }
            set
            {
                _selectedLetterSizeOption = value;
                OnPropertyChanged();
            }
        }

     
        private ConfigOptionObject _selectedIntensityOption;
        public ConfigOptionObject SelectedIntensityOption
        {
            get { return _selectedIntensityOption; }
            set
            {
                _selectedIntensityOption = value;
                OnPropertyChanged();
            }
        }

        private ConfigOptionObject _selectedGapOption;
        public ConfigOptionObject SelectedGapOption
        {
            get { return _selectedGapOption; }
            set { 
                _selectedGapOption = value;
                OnPropertyChanged();
            }
        }




        private int _timeDelay;
        public int TimeDelay
        {
            get { return _timeDelay; }
            set { _timeDelay = value; }
        }

        private int _dataTimeout;
        public int DataTimeout
        {
            get { return _dataTimeout; }
            set { _dataTimeout = value; }
        }

        private bool _reverseVideo = false;
        public bool ReverseVideo
        {
            get { return _reverseVideo; }
            set { _reverseVideo = value; }
        }

        private bool _deviceIsEnabled = true;
        public bool DeviceIsEnabled
        {
            get { return _deviceIsEnabled; }
            set { _deviceIsEnabled = value; }
        }





        #endregion


        //SP
        #region TCP STUFF

        private readonly TcpClientService _tcpClientService;
        private readonly List<ServerConfig> _servers;
        private ObservableCollection<byte> _results;
        private bool _isOperationInProgress;

        //DO WE REALLY NEED AN OBSERVABLE COLLECTION FOR THIS?
        //WE CAN PROBABBLY MAKE DO WITH A LIST, PLEASE CHECK
        public ObservableCollection<byte> Results
        {
            get => _results;
            set
            {
                _results = value;
                OnPropertyChanged(nameof(Results));
            }
        }       

        #endregion

        FrameBuilderForPFDB FrameBuilderForPFDBService;
        FrameBuilderForMultiline FrameBuilderForMultilineService;


        #region TADDB_SET_Command
        //public RelayCommand TADDB_SET_Command => new RelayCommand(execute => TADDB_SET(), canExecute => CanExecuteTADDB_SET());
        //asynchronous relay command, part of communitytoolkit.mvvm
        public IAsyncRelayCommand TADDB_SET_Command => new AsyncRelayCommand(TADDB_SET, CanExecuteTADDB_SET);

        public ICommand SaveCommandTemp { get; }
        public ICommand OpenCommandTemp { get; }


        // You can also check out RelayCommandforConcurrency from the GOAT Simulator project
        
        private async Task TADDB_SET()
        {
            IsOperationInProgress = true;
            Results.Clear();

            _servers.Clear();

            //Makes all the frames for all the displays
            //Adds Ip address, port number and frame to a new ServerConfig object
            //Adds this serverConfig object to the _servers list

            #region PFDB
            //PFDB    //Adds ServerConfig object FOR PFDB to _servers list       
            foreach (ActiveTrain train in ActiveTrains)
            {
                if (train.TADDB_Update == true)
                {

                    foreach (Platform platform in Platforms)
                {
                    // IMPORTANT NOTE: THIS IS ASSUMING PLATFORM NUMBERS ARE INT DATA TYPES ONLY
                    // THIS WILL NOT WORK IF SPECIAL PF NUMBER ARE THERE SINCE THEY ARE ALPHANUMERIC
                    if (int.TryParse(platform.PlatformNumber, out int platformNumber))
                    {
                        if (platformNumber == train.PFNo)
                        {
                            foreach(Device device in platform.Devices)
                            {
                                if(device.DeviceType == DeviceType.PFDB) 
                                {

                                    //this method will be changed to a task eventually
                                    // I don't think chnging this to a task is really necessary
                                    _servers.Add(FrameBuilder( device, train));                                

                                }

                                //else if (device.DeviceType == DeviceType.AGDB)
                                //{
                                //    FrameBuilderforAGDB(train, platform);
                                //}
                            }
                        }
                    }
                    else
                    {
                        throw new Exception($"Invalid platform number: {platform.PlatformNumber}");
                    }
                }

                }
            }
            #endregion

            #region MonoMLDB
            //For MonoMLDB  // EXTEND FOR IVD AND OVD LATER
            List<TrainToBeDisplayed> trainToBeDisplayedList = new List<TrainToBeDisplayed>();
            foreach (ActiveTrain train in ActiveTrains)
            {
                if (train.TADDB_Update == true)
                {
                    trainToBeDisplayedList.Add(FrameBuilderForMultilineService.AddTrainToBeDisplayedtoList(train));

                }
            }

            foreach (Platform platform in Platforms)
            {
                if (platform.PlatformType == PlatformType.Concourse)
                {
                    foreach (Device device in platform.Devices)
                    {
                        if (device.DeviceType == DeviceType.MLDB)
                        {

                            //this method will be changed to a task eventually
                            // I don't think chnging this to a task is really necessary
                            _servers.Add(FrameBuilder(device, null, trainToBeDisplayedList));

                        }
                    }
                }

                else
                {
                    throw new Exception($"Invalid platform number: {platform.PlatformNumber}");
                }
            }


            #endregion





            try
            {
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30))) // Timeout after 30 seconds
                {
                    // Run TCP connections in parallel
                    var tasks = _servers.Select(server => _tcpClientService.SendPacketAsync(server, cts.Token));
                    var responses = await Task.WhenAll(tasks);

                    // Process results
                    foreach (var (success, response, errorMessage) in responses)
                    {
                        //if (success)
                        //    Results.Add($"Success: Received {response}");

                        //else
                        //    Results.Add($"Error: {errorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                //Results.Add($"Operation failed: {ex.Message}");
            }
            finally
            {
                IsOperationInProgress = false;
            }
        }

        private ServerConfig FrameBuilder( Device device , ActiveTrain? train=null, List<TrainToBeDisplayed>? trainToBeDisplayedList =null)
        {
            byte[] frame; 

            ServerConfig serverConfig = new ServerConfig
            {
                IpAddress = device.IpAddress,
                Port = 25000,//Port number fixed according to the document              
            };

            if (device.DeviceType == DeviceType.PFDB)
            {
                //First step is extraction of data from the ActiveTrain instance  and device(IP etc)           
                FrameBuilderForPFDBService.ReadAndAddDirectFields(train, device);

                //Second step is iterating over the list of platforms and
                //1) Validating whether or not the platform associated with the train instance exists or not
                //2) If platform instance exists, then identify the number of PFDB on the platform. Number of PFDB == number of data packets to be made.(Atleast for now)
                //3) Once the number of PFDB is identified, we need to extract the destination IP address and port number will be fixed

                FrameBuilderForPFDBService.ProcessDataFromReadFields();

                FrameBuilderForPFDBService.AddFixedBytes();

                serverConfig.Packet = FrameBuilderForPFDBService.CompileFrame();

            }

            else if (device.DeviceType == DeviceType.MLDB)
            { 

            }

            //remove later
            // frame = FrameBuilderForPFDBService.AddStartBytes();
            return serverConfig;
        }


        private bool CanExecuteTADDB_SET()
        {
            return !IsOperationInProgress;

            //Need to add all possible validation mechanisms
            return true; // Placeholder, replace with actual condition
        }

        #endregion

        public bool IsOperationInProgress
        {
            get => _isOperationInProgress;
            set
            {
                _isOperationInProgress = value;
                OnPropertyChanged(nameof(IsOperationInProgress));
                // TADDB_SET_Command.RaiseCanExecuteChanged(); // Notify command to re-evaluate CanExecute
                // public IAsyncRelayCommand TADDB_SET_Command => new AsyncRelayCommand(TADDB_SET, CanExecuteTADDB_SET);

               TADDB_SET_Command.NotifyCanExecuteChanged();
                // I think this shourld do the job, not sure though
                //Not even sure about why do we even need this here
            }
        }


        private string _filePath;
        private bool CanSaveConfiguration(object parameter)
        {
            // Add validation logic if needed (e.g., check if Station is not null)
            return ActiveTrains != null;
        }

        private void SaveConfiguration(object parameter)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                Title = "Save Station Configuration",
                DefaultExt = "json",
                AddExtension = true,
                OverwritePrompt = true
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    _filePath = saveFileDialog.FileName;
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
                    };

                    string jsonString = System.Text.Json.JsonSerializer.Serialize(ActiveTrains, options);
                    File.WriteAllText(_filePath, jsonString);

                    // Verify file creation
                    if (File.Exists(_filePath))
                    {
                        System.Windows.MessageBox.Show($"Configuration saved successfully to {_filePath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("File was not created. Please check the file path and permissions.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    System.Windows.MessageBox.Show($"Permission denied when saving to {_filePath}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (System.Text.Json.JsonException ex)
                {
                    System.Windows.MessageBox.Show($"Serialization error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error saving configuration: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("Save operation cancelled.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LoadConfiguration(object parameter)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                Title = "Load Station Configuration"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    _filePath = openFileDialog.FileName;
                    if (!File.Exists(_filePath))
                    {
                        System.Windows.MessageBox.Show($"File not found: {_filePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    string jsonString = File.ReadAllText(_filePath);
                    var options = new JsonSerializerOptions
                    {
                        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
                    };

                    ActiveTrains = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<ActiveTrain>>(jsonString, options);

                    //   OnPropertyChanged(nameof(Station));
                    OnPropertyChangedforLoading("ActiveTrains");
                    System.Windows.MessageBox.Show("Configuration loaded successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (System.Text.Json.JsonException ex)
                {
                    System.Windows.MessageBox.Show($"Deserialization error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)//getting error over here
                {
                    System.Windows.MessageBox.Show($"Error loading configuration: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    var message = ex.ToString();
                    File.WriteAllText("error_log.txt", message);
                    System.Windows.MessageBox.Show(message);

                }
            }
        }

        public void OnPropertyChangedforLoading(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }






       

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

        //public ObservableCollection<Platform> Platforms { get; set; }
        //public ObservableCollection<Device> Devices { get; set; }

        private ObservableCollection<Platform> _platforms;
        public ObservableCollection<Platform> Platforms
        {
            get { return _platforms; }
            set {
                _platforms = value;
               OnPropertyChanged();
            }
        }

        private ObservableCollection<Device>  _devices;
        public ObservableCollection<Device>  Devices
        {
            get { return _devices; }
            set {
                _devices = value;
                OnPropertyChanged();
            }
        }




        public Platform SelectedPlatform
        {
            get { return _selectedPlatform; }
            set
            {
                if (_selectedPlatform != value)
                {
                    _selectedPlatform = value;
                    LoadDevices();
                    OnPropertyChanged();
                    
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

                    if (SelectedDevice != null)
                    {
                        UpdateConfigDevicePage();

                    }
                    //else
                    //{
                    //    // Handle case when no device is selected
                    //    SelectedSpeedOption = null;
                    //    SelectedEffectOption = null;
                    //    SelectedLetterSizeOption = null;
                    //    SelectedIntensityOption = null;
                    //    TimeDelay = 0;
                    //    DataTimeout = 0;
                    //    ReverseVideo = false;
                    //    DeviceIsEnabled = false;

                    //}


                }
            }
        }

        private void UpdateConfigDevicePage()
        {
           
            SelectedSpeedOption = SpeedOptions.FirstOrDefault(s => s.ByteValue == SelectedDevice.SpeedByte);      
                      
            SelectedEffectOption = EffectOptions.FirstOrDefault(s => s.ByteValue == SelectedDevice.EffectByte);
           
            SelectedLetterSizeOption = LetterSizeOptions.FirstOrDefault(s => s.ByteValue == SelectedDevice.LetterSizeByte);

            SelectedIntensityOption = LetterSizeOptions.FirstOrDefault(s => s.ByteValue == SelectedDevice.IntensityByte);

            SelectedGapOption = LetterSizeOptions.FirstOrDefault(s => s.ByteValue == SelectedDevice.GapByte);

            TimeDelay = SelectedDevice.TimeDelayValueByte;

            DataTimeout = SelectedDevice.DataTimeoutValueByte;

            ReverseVideo = SelectedDevice.IsReverseVideo ;

            DeviceIsEnabled = SelectedDevice.IsEnabled ;


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
                

                if (Devices.Count != 0)
                {
                    //UpdateConfigDevicePage();
                    SelectedDevice = SelectedPlatform.Devices.FirstOrDefault(); // Select the first device by default
                }
                else
                {
                    SelectedDevice = null;
                    // Handle case when no device is selected
                    SelectedSpeedOption = null;
                    SelectedEffectOption = null;
                    SelectedLetterSizeOption = null;
                    SelectedIntensityOption = null;
                    SelectedGapOption = null;
                    TimeDelay = 0;
                    DataTimeout = 0;
                    ReverseVideo = false;
                    DeviceIsEnabled = false;

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

        public ObservableCollection<DeviceStatus> DeviceStatusList { get; set; } = new ObservableCollection<DeviceStatus>();

     
        public void UpdateDeviceStatusList(string jsonResponse)
        {
            try
            {
                var updatedPlatforms = JsonConvert.DeserializeObject<List<Platform>>(jsonResponse);
                DeviceStatusList.Clear();

                foreach (var platform in updatedPlatforms)
                {
                    foreach (var device in platform.Devices)
                    {
                        var deviceStatus = new DeviceStatus
                        {
                            PlatformNumber = platform.PlatformNumber,
                            DeviceId = device.Id,
                            DeviceType = device.DeviceType,
                            Description = device.Description,
                            IpAddress = device.IpAddress,
                            Status = device.Status,
                            LastStatusWhen = DateTime.Now, // Update to current time
                            IsEnabled = device.IsEnabled
                        };

                        DeviceStatusList.Add(deviceStatus);
                    }
                }

                OnPropertyChanged(nameof(DeviceStatusList));
            }
            catch (System.Text.Json.JsonException ex)
            {
                Console.WriteLine($"JSON Deserialization Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error while updating device statuses: {ex.Message}");
            }
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

        public void OnPropertyChangedSP([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
