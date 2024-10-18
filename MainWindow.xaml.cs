using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using Newtonsoft.Json;
using System.Net.Http;
using IpisCentralDisplayController.ntes;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Reflection;
using IpisCentralDisplayController.Managers;
using IpisCentralDisplayController.Helpers;
using IpisCentralDisplayController.models;
using IpisCentralDisplayController.managers;
using IpisCentralDisplayController.views;
using System.Drawing;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using FontFamily = System.Windows.Media.FontFamily;
using IpisCentralDisplayController.Models;
using System.IO.Compression;
using IpisCentralDisplayController.custom;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using System.Net.NetworkInformation;
using IpisCentralDisplayController.services;

namespace IpisCentralDisplayController
{
    public class Ticket
    {
        public string Timestamp { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
    }

    public class BackupItem
    {
        public string BackupDate { get; set; }
        public string Details { get; set; }
    }

    public class LogItem
    {
        public string Timestamp { get; set; }
        public string Message { get; set; }
        public string Severity { get; set; }
    }

    public class Alert
    {
        public string Message { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public bool AudioAvailable { get; set; }
        public string Urgency { get; set; }
        public string Severity { get; set; }
        public string Certainty { get; set; }
        public bool IsActive { get; set; }
    }

    public class CAPMediaFile
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string FilePath { get; set; }
    }

    public class Clip
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string FilePath { get; set; }
        public double StartTime { get; set; }
        public double EndTime { get; set; }
    }

    public class MediaRecorder
    {
        private DateTime startTime;
        private DateTime pauseTime;
        private TimeSpan elapsedTime;
        private bool isPaused;
        private bool isRecording;

        public void StartRecording()
        {
            startTime = DateTime.Now;
            isRecording = true;
            isPaused = false;
            elapsedTime = TimeSpan.Zero;
        }

        public void PauseRecording()
        {
            if (isRecording && !isPaused)
            {
                pauseTime = DateTime.Now;
                elapsedTime += pauseTime - startTime;
                isPaused = true;
            }
        }

        public void ResumeRecording()
        {
            if (isRecording && isPaused)
            {
                startTime = DateTime.Now;
                isPaused = false;
            }
        }

        public void StopRecording()
        {
            if (isRecording)
            {
                if (!isPaused)
                {
                    elapsedTime += DateTime.Now - startTime;
                }
                isRecording = false;
                isPaused = false;
            }
        }

        public string GetElapsedTime()
        {
            if (isRecording)
            {
                if (isPaused)
                {
                    return elapsedTime.ToString(@"mm\:ss");
                }
                else
                {
                    return (elapsedTime + (DateTime.Now - startTime)).ToString(@"mm\:ss");
                }
            }
            return "0:00";
        }

        public void SaveRecording(string fileName)
        {
            // Implement saving recorded audio to a file
        }
    }

    public class AudioPlayer
    {
        private MediaElement mediaElement;

        public AudioPlayer()
        {
            mediaElement = new MediaElement();
        }

        public void Play()
        {
            mediaElement.Play();
        }

        public void Pause()
        {
            mediaElement.Pause();
        }

        public void Stop()
        {
            mediaElement.Stop();
        }

        public void SetVolume(double volume)
        {
            mediaElement.Volume = volume;
        }
    }

    public class CgdbManager
    {
        public string DisplayId { get; set; }
        public string Status { get; set; }
    }

    //NTES API 
    

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string applicationTitle = "IP based IPIS | Central Display Controller";
        private static string softwareVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        private static string buildDateTime = RetrieveLinkerTimestamp().ToString("yyyy-MM-dd HH:mm:ss");

        private string _workspacePath;
        private readonly UserCategoryManager _userCategoryManager;

        private readonly UserManager _userManager;
        private readonly StationInfoManager _stationInfoManager;
        private PlatformDeviceManager _platformDeviceManager;

        private DisplayStyleManager _displayStyleManager;
        private AudioSettingsManager _audioSettingsManager;

        private RmsServerManager _rmsSettingsManager;
        private CAPServerSettingsManager _capServerSettingsManager;

        private readonly TrainStatusDisplayManager _trainStatusDisplayManager;

        private MediaManager _mediaManager;
        private TimelineManager _timelineManager;
        private StationManager _stationManager;
        private ActiveTrainManager _activeTrainsManager;
        private EventLogManager _eventLogManager;

        private MainViewModel _mainViewModel;

        private DispatcherTimer _mediaTimer;
        private double _imageZoom = 1.0;

        DispDataIPIS_t DispDataIPIS;
        DispDataIPISs_t DispDataIPISs;
        DispDataIPISrgb_t DispDataIPISrgb;
        config_t config;

        int height, width;
        uint dType = 0;

        CmdPacket_t CmdPacket;
        Canvas canvas;

        private NtesApiResponse951 ntesApiResponse951;
        public ObservableCollection<Ticket> Tickets { get; set; }
        public ObservableCollection<BackupItem> LocalBackups { get; set; }
        public ObservableCollection<BackupItem> CloudBackups { get; set; }       
        public ObservableCollection<User> Users { get; set; }
        public User CurrentUser { get; set; }


        //public ObservableCollection<MediaFile> MediaFiles { get; set; }
        public ObservableCollection<Clip> TimelineClips { get; set; }
        public int SelectedMediaIndex { get; set; }
        public string ActiveTab { get; set; }

        private MediaRecorder mediaRecorder;
        private AudioPlayer audioPlayer;
        private ObservableCollection<string> audioFiles;
        private ObservableCollection<string> playlist;
        private DispatcherTimer recordingTimer;

        private DispatcherTimer dateTimeTimer;

        private DispatcherTimer _autoFetchTimer;
        private bool _autoFetchEnabled;
        private int _queryTimeMinutes;
        private int _nextMins;

        private ServiceManager _serviceManager;
        private RmsServerSettings _rmsSettings;
        private RMSService _rmsService;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new ColorViewModel();

            _mediaTimer = new DispatcherTimer();
            _mediaTimer.Interval = TimeSpan.FromMilliseconds(500);
            _mediaTimer.Tick += MediaTimer_Tick;

            var jsonHelperAdapter = new SettingsJsonHelperAdapter();
            _userManager = new UserManager(jsonHelperAdapter);
            _userCategoryManager = new UserCategoryManager(jsonHelperAdapter);
            _stationInfoManager = new StationInfoManager(jsonHelperAdapter);
            _platformDeviceManager = new PlatformDeviceManager(jsonHelperAdapter);
            _displayStyleManager = new DisplayStyleManager(jsonHelperAdapter);
            _audioSettingsManager = new AudioSettingsManager(jsonHelperAdapter);
            _rmsSettingsManager = new RmsServerManager(jsonHelperAdapter);
            _capServerSettingsManager = new CAPServerSettingsManager(jsonHelperAdapter);
            _trainStatusDisplayManager = new TrainStatusDisplayManager(jsonHelperAdapter);
            _mediaManager = new MediaManager(jsonHelperAdapter);
            _timelineManager = new TimelineManager(jsonHelperAdapter);
            _stationManager = new StationManager(jsonHelperAdapter);
            _activeTrainsManager = new ActiveTrainManager(jsonHelperAdapter);
            _eventLogManager = new EventLogManager(jsonHelperAdapter);

            InitializeDateTimeUpdate();
          
            _mainViewModel = new MainViewModel();
            _mainViewModel.LoadUserCategories(_userCategoryManager.LoadUserCategories());
            _mainViewModel.LoadUsers(_userManager.LoadUsers());
            _mainViewModel.LoadAudioSettings(_audioSettingsManager.LoadAudioSettings());
            _mainViewModel.LoadRmsSettings(_rmsSettingsManager.LoadRmsServerSettings());
            _mainViewModel.LoadCAPServerSettings(_capServerSettingsManager.LoadCapServerSettings());
            CAPPassword.Password = _mainViewModel.CapSettings.Password;
            ApiKey.Password = _mainViewModel.CapSettings.ApiKey;
            _mainViewModel.LoadMediaFiles(_mediaManager.LoadMediaFiles());
            _mainViewModel.LoadTimelines(_timelineManager.LoadTimelines());
            _mainViewModel.LoadActiveTrains(_activeTrainsManager.LoadActiveTrains());

            //just today's logs 
            _mainViewModel.LoadEventLogs(_eventLogManager.LoadEventLogs());

            DataContext = _mainViewModel;
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;

            InitializeServices();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //WorkspaceManager.DeleteWorkspace();
            CheckWorkspacePath();
            LoadDisplaySettings();
            LoadAudioInterfaces();
            PopulateStationInfo();
            LoadPlatforms();
            InitializeUserCategories();

            CheckAndCreateDisplayStyles();
            InitializeNtesControls();
            CheckAndPromptForAdminUser();


            EnsureDefaultTimeline();
            tb_timeline.Text = _mainViewModel.SelectedTimeline.Name;

            var loginWindow = new LoginWindow(_userManager);
            bool? loginResult = loginWindow.ShowDialog();

            if (loginResult == true)
            {
                LogEvent($"User '{_userManager.CurrentUser.Name}' successfully logged in.", EventType.Information, "Login");

                CheckAndPromptForStationInfo();
                PopulateStatusFields();

                // await _viewModel.FetchAndDisplayTrains();
            }
            else
            {
                // Handle login cancellation or failure
                this.Close(); // Close the MainWindow if login is not successful
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OnAppExit(sender,e);
        }

        private void InitializeServices()
        {
            _rmsSettings = _rmsSettingsManager.LoadRmsServerSettings();
            if (AreRmsSettingsValid(_rmsSettings))
            {
                _rmsService = new RMSService(_rmsSettings);
                _rmsService.Start();
                Console.WriteLine("RMSService started successfully with valid settings.");
            }
            else
            {
                MessageBox.Show("Invalid RMS server settings. Please configure valid settings to start the service.", "Settings Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                Console.WriteLine("RMSService could not be started due to invalid settings.");
            }
            //var capService = new CAPService();
            //var backupService = new BackupService();
            //var displayService = new DisplayService();
            //var announcementService = new AnnouncementService();

            //    _serviceManager = new ServiceManager(new List<IService>
            //{
            //    rmsService/*, capService, backupService, displayService, announcementService*/
            //});

            //    _serviceManager.StartAllServices();

            //check for RMS
            TestConnection_Click(null, null);
        }

        private void OnAppExit(object sender, EventArgs e)
        {
            //_serviceManager.StopAllServices();
        }

        private bool AreRmsSettingsValid(RmsServerSettings settings)
        {
            if (settings == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(settings.ApiUrl))
            {
                return false;
            }

            return true;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Logout();
        }

        //private void Logout()
        //{
        //    tc_main.SelectedIndex = 0;
        //    var loginWindow = new LoginWindow(_userManager);
        //    bool? loginResult = loginWindow.ShowDialog();

        //    if (loginResult == true)
        //    {
        //        // Proceed with further initialization only after successful login
        //        CheckAndPromptForStationInfo();
        //        PopulateStatusFields();

        //        // await _viewModel.FetchAndDisplayTrains();
        //    }
        //    else
        //    {
        //        // Handle login cancellation or failure
        //        this.Close(); // Close the MainWindow if login is not successful
        //    }
        //}

        private void Logout()
        {
            // Log the user logout event
            LogEvent($"User '{_userManager.CurrentUser.Name}' logged out.", EventType.Information, "Logout");

            tc_main.SelectedIndex = 0;

            var loginWindow = new LoginWindow(_userManager);
            bool? loginResult = loginWindow.ShowDialog();

            if (loginResult == true)
            {
                // Log the successful login after logout
                LogEvent($"User '{_userManager.CurrentUser.Name}' successfully logged in after logout.", EventType.Information, "Login");

                CheckAndPromptForStationInfo();
                PopulateStatusFields();

                // await _viewModel.FetchAndDisplayTrains();
            }
            else
            {
                // Log the failed or cancelled login attempt
                LogEvent("User login failed or was cancelled after logout.", EventType.Error, "Login");

                this.Close();
            }
        }


        private void LogEvent(string description, EventType eventType, string source)
        {
            var newLog = new EventLog
            {
                Timestamp = DateTime.Now,
                EventID = new Random().Next(1000, 9999), // Random Event ID
                EventType = eventType,
                Source = source,
                Description = description,
                IsSentToServer = false
            };

            _eventLogManager.AddEventLog(newLog);
            _mainViewModel.EventLogs.Add(newLog);
            _rmsService.ReceiveEventLog(newLog);
        }


        //NTES Specific
        private void InitializeNtesControls()
        {
            // Initialize auto-fetch timer
            _autoFetchTimer = new DispatcherTimer();
            _autoFetchTimer.Tick += AutoFetchTimer_Tick;

            // Set default values
            _autoFetchEnabled = false;
            _queryTimeMinutes = 3; // Default query time interval
            _nextMins = 30; // Default next mins to fetch

            // Bind the UI controls to these default values
            //AutoFetchCheckBox.IsChecked = _autoFetchEnabled;
            QueryTimeUpDown.Value = _queryTimeMinutes;
            NextMinsComboBox.SelectedIndex = 0; // 30 min default
        }

        private void StartAutoFetch()
        {
            if (_autoFetchTimer == null)
            {
                _autoFetchTimer = new DispatcherTimer();
                _autoFetchTimer.Tick += AutoFetchTimer_Tick;
            }
            _autoFetchTimer.Interval = TimeSpan.FromMinutes(QueryTimeUpDown.Value ?? 3); // Default to 3 minutes if null
            _autoFetchTimer.Start();
        }

        private void StopAutoFetch()
        {
            _autoFetchTimer?.Stop();
        }

        private async void AutoFetchTimer_Tick(object sender, EventArgs e)
        {
            await FetchTrainsFromNtesAsync();
        }

        private void QueryTimeUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (QueryTimeUpDown.Value.HasValue)
            {
                _queryTimeMinutes = QueryTimeUpDown.Value.Value;
                if (_autoFetchEnabled)
                {
                    _autoFetchTimer.Interval = TimeSpan.FromMinutes(_queryTimeMinutes);
                }
            }
        }

        private void NextMinsComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            //var selectedItem = NextMinsComboBox.SelectedItem as ComboBoxItem;
            //if (selectedItem != null)
            //{
            //    if (int.TryParse(selectedItem.Content.ToString().Split(' ')[0], out int nextMins))
            //    {
            //        _nextMins = nextMins;
            //    }
            //}
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await FetchTrainsFromNtesAsync();
        }

        private async Task FetchTrainsFromNtesAsync()
        {
            try
            {
                var ntesApi = new NtesAPI951();
                var ntesResponse = await ntesApi.GetTrainsAsync(_stationInfoManager.CurrentStationInfo.StationCode, _nextMins); // Assume _nextMins is defined elsewhere

                if (ntesResponse == null)
                {
                    MessageBox.Show("No data received from NTES.", "No Data", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                bool replaceOnlyNTES = ReplaceTrainCombobox.SelectedIndex == 0;

                if (replaceOnlyNTES)
                {
                    // Remove only NTES trains from the list
                    var nonNTESTrains = _mainViewModel.ActiveTrains.Where(t => t.Ref != TrainSource.NTES).ToList();
                    _mainViewModel.ActiveTrains.Clear();
                    foreach (var train in nonNTESTrains)
                    {
                        _mainViewModel.ActiveTrains.Add(train);
                    }
                }
                else
                {
                    // Clear the entire ActiveTrains collection
                    _mainViewModel.ActiveTrains.Clear();
                }

                // Convert and add trains from each relevant list in the response directly to the ActiveTrains collection
                AddTrainsToActiveTrains(ntesResponse.VTrainList);
                AddTrainsToActiveTrains(ntesResponse.VRescheduledTrainList);
                AddTrainsToActiveTrains(ntesResponse.VCancelledTrainList);
                AddTrainsToActiveTrains(ntesResponse.VCancelTrainDueToCS);
                AddTrainsToActiveTrains(ntesResponse.VCancelTrainDueToCD);
                AddTrainsToActiveTrains(ntesResponse.VCancelTrainDueToDiversion);
                AddTrainsToActiveTrains(ntesResponse.VTrainListDueToCS);
                AddTrainsToActiveTrains(ntesResponse.VTrainListDueToCD);
                AddTrainsToActiveTrains(ntesResponse.VTrainListDueToDV);

                if (!_mainViewModel.ActiveTrains.Any())
                {
                    MessageBox.Show("No trains found for the specified station.", "No Data", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                _activeTrainsManager.SaveActiveTrains(_mainViewModel.ActiveTrains.ToList());
                MessageBox.Show("NTES data fetched and populated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while fetching data from NTES: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddTrainsToActiveTrains(IEnumerable<NtesTrain951> ntesTrainList)
        {
            if (ntesTrainList != null && ntesTrainList.Any())
            {
                foreach (var nt in ntesTrainList)
                {
                    _mainViewModel.ActiveTrains.Add(new ActiveTrain(nt));
                }
            }
        }


        //Color Display Related code
        private void LoadDisplaySettings()
        {
            // Load display settings from the manager
            var displaySettings = _trainStatusDisplayManager.LoadDisplaySettings();

            // Update the ViewModel with the loaded settings
            _mainViewModel.Theme = displaySettings.Theme;
            _mainViewModel.TrainTemplates = new System.Collections.ObjectModel.ObservableCollection<TrainDisplayTemplate>(displaySettings.TrainTemplates);
        }

        private void SaveDisplaySettings_Click(object sender, RoutedEventArgs e)
        {
            SaveDisplaySettings();
        }

        private void SaveDisplaySettings()
        {
            // Prompt the user to confirm saving settings
            var result = MessageBox.Show("Do you want to save the display settings?", "Save Settings", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Prepare the data to be saved
                    var trainTemplatesList = new List<TrainDisplayTemplate>(_mainViewModel.TrainTemplates);

                    // Save the current theme and train templates using the manager
                    _trainStatusDisplayManager.SaveDisplaySettings(_mainViewModel.Theme, trainTemplatesList);

                    // Notify the user of a successful save
                    MessageBox.Show("Display settings saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    // Notify the user if there was an error during the save process
                    MessageBox.Show($"An error occurred while saving the display settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        //Audio Interface Related Code
        private void LoadAudioInterfaces()
        {
            _mainViewModel.RefreshAudioInterfaces(_audioSettingsManager);
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _mainViewModel.SaveAudioSettings(_audioSettingsManager);
            MessageBox.Show("Audio settings have been saved successfully.", "Save Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RefreshAudioButton_Click(object sender, RoutedEventArgs e)
        {
            _mainViewModel.RefreshAudioSettings(_audioSettingsManager);
            MessageBox.Show("Audio settings and interfaces have been refreshed.", "Refresh Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void TestAudioToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var toggleButton = sender as ToggleButton;
            if (toggleButton != null)
            {
                if (toggleButton.IsChecked == true)
                {
                    MessageBoxResult result = MessageBox.Show("Are you sure you want to start the audio test? This will output test audio through the selected interface.", "Start Audio Test", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        _mainViewModel.StartMonitorAudioTest(_audioSettingsManager);
                    }
                    else
                    {
                        toggleButton.IsChecked = false; // Revert the toggle if user cancels
                    }
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Are you sure you want to stop the audio test?", "Stop Audio Test", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        _mainViewModel.StopMonitorAudioTest(_audioSettingsManager);
                    }
                    else
                    {
                        toggleButton.IsChecked = true; // Revert the toggle if user cancels
                    }
                }
            }
        }

        //Workspace Related Code
        private void CheckWorkspacePath()
        {
            string workspacePath = WorkspaceManager.GetWorkspacePath();
            _workspacePath = workspacePath;

            if (string.IsNullOrEmpty(workspacePath) || !WorkspaceManager.IsValidWorkspace(workspacePath))
            {
                MessageBoxResult result = MessageBox.Show("No valid workspace path found. Do you want to load an existing workspace or create a new one?", "Workspace Path", MessageBoxButton.YesNoCancel);

                if (result == MessageBoxResult.Yes)
                {
                    LoadExistingWorkspace();
                }
                else if (result == MessageBoxResult.No)
                {
                    CreateNewWorkspace();
                }
                else
                {
                    MessageBox.Show("Cannot Continue without a workspace.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }
            }
            else
            {
                SetWindowTitle(workspacePath);
            }
        }

        private void LoadExistingWorkspace()
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Select Workspace Directory"
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string selectedPath = dialog.FileName;

                if (WorkspaceManager.IsValidWorkspace(selectedPath))
                {
                    WorkspaceManager.SetWorkspacePath(selectedPath);
                    SetWindowTitle(selectedPath);
                }
                else
                {
                    MessageBox.Show("Invalid workspace selected. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    LoadExistingWorkspace();
                }
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private void CreateNewWorkspace()
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Select Directory to Create New Workspace"
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string selectedPath = dialog.FileName;

                if (!Directory.Exists(selectedPath))
                {
                    Directory.CreateDirectory(selectedPath);
                }

                WorkspaceManager.SetWorkspacePath(selectedPath);
                SetWindowTitle(selectedPath);
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        public void EnsureWorkspaceDirectories(string workspacePath)
        {
            // Ensure the Sounds/Stations directory structure
            string baseSoundPath = Path.Combine(workspacePath, "Sounds", "Stations");
            string[] languageFolders = new string[]
            {
        "ENGLISH", "HINDI", "ASSAMESE", "BANGLA", "DOGRI", "GUJARATI",
        "KANNADA", "KONKANI", "MALAYALAM", "MARATHI", "MANIPURI", "NEPALI",
        "ODIA", "PUNJABI", "SANSKRIT", "SINDHI", "TAMIL", "TELUGU", "URDU"
            };

            foreach (var folder in languageFolders)
            {
                string languagePath = Path.Combine(baseSoundPath, folder);
                if (!Directory.Exists(languagePath))
                {
                    Directory.CreateDirectory(languagePath);
                }
            }

            // Ensure other workspace directories (example placeholders)
            string mediaPath = Path.Combine(workspacePath, "Media");
            string imagesPath = Path.Combine(mediaPath, "Images");
            string videosPath = Path.Combine(mediaPath, "Videos");
            string projectsPath = Path.Combine(workspacePath, "Projects");

            string[] otherDirectories = new string[]
            {
        mediaPath, imagesPath, videosPath, projectsPath
            };

            foreach (var directory in otherDirectories)
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
        }

        private void SetWindowTitle(string workspacePath)
        {
            this.Title = $"{applicationTitle} | Workspace: {workspacePath} | v{softwareVersion} | Build {buildDateTime}";
        }

        private void CreateNewWorkspace_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Select Directory to Create New Workspace"
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string newPath = dialog.FileName;

                // Check if the directory is empty
                if (Directory.GetFiles(newPath).Length == 0 && Directory.GetDirectories(newPath).Length == 0)
                {
                    // Ensure the workspace directories exist
                    WorkspaceManager.EnsureWorkspaceDirectoriesExist(newPath);
                    MessageBox.Show("New workspace created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    var result = MessageBox.Show("Do you want to set this as the default workspace?", "Set Default Workspace", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        WorkspaceManager.SetWorkspacePath(newPath);
                        SetWindowTitle(newPath);
                        MessageBox.Show("Default workspace set successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("The selected directory is not empty. Please select an empty directory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ChangeWorkspace_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title = "Select Workspace Directory"
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string newPath = dialog.FileName;
                string metadataFilePath = Path.Combine(newPath, "workspace.json");

                if (!File.Exists(metadataFilePath))
                {
                    MessageBox.Show("The selected directory is not a valid workspace. Missing workspace metadata file.", "Invalid Workspace", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                try
                {
                    var metadataJson = File.ReadAllText(metadataFilePath);
                    var metadata = JsonConvert.DeserializeObject<WorkspaceMetadata>(metadataJson);

                    if (metadata == null || string.IsNullOrEmpty(metadata.WorkspaceName) || metadata.CreationDate == default || string.IsNullOrEmpty(metadata.Version))
                    {
                        MessageBox.Show("The workspace metadata file is invalid.", "Invalid Workspace", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while reading the workspace metadata file: {ex.Message}", "Invalid Workspace", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string[] requiredDirectories = {
                    "DB",
                    "Recordings",
                    "Audio",
                    "Sounds",
                    "Reports",
                    "Alerts",
                    "Media",
                    "Renders",
                    "Backup",
                    "Fonts",
                    "Internal"
                };

                var missingDirectories = requiredDirectories.Where(dir => !Directory.Exists(Path.Combine(newPath, dir))).ToList();

                if (missingDirectories.Any())
                {
                    string missingDirs = string.Join(", ", missingDirectories);
                    var result = MessageBox.Show($"The selected workspace is missing the following directories: {missingDirs}. Would you like to repair the workspace?", "Repair Workspace", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        WorkspaceManager.EnsureWorkspaceDirectoriesExist(newPath);
                        MessageBox.Show("Workspace repaired successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        return;
                    }
                }

                WorkspaceManager.SetWorkspacePath(newPath);
                _workspacePath = newPath;
                SetWindowTitle(newPath);
                MessageBox.Show("Workspace switched successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BrowseWorkspace_Click(object sender, RoutedEventArgs e)
        {
            string workspacePath = WorkspaceManager.GetWorkspacePath();
            if (WorkspaceManager.IsValidWorkspace(workspacePath))
            {
                try
                {
                    System.Diagnostics.Process.Start("explorer.exe", workspacePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while trying to open the workspace directory: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Invalid workspace directory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ExportWorkspace_Click(object sender, RoutedEventArgs e)
        {
            string workspacePath = WorkspaceManager.GetWorkspacePath();
            if (WorkspaceManager.IsValidWorkspace(workspacePath))
            {
                var dialog = new CommonSaveFileDialog
                {
                    Filters = { new CommonFileDialogFilter("ZIP Archive", "*.zip") },
                    Title = "Export Workspace"
                };

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    string zipPath = dialog.FileName;
                    var progressDialog = new ProgressDialog("Exporting workspace...");
                    progressDialog.Owner = this;
                    progressDialog.Show();

                    try
                    {
                        await Task.Run(() => CreateZipWithProgress(workspacePath, zipPath, progressDialog));
                        MessageBox.Show("Workspace exported successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        System.Diagnostics.Process.Start("explorer.exe", Path.GetDirectoryName(zipPath));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while exporting the workspace: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        progressDialog.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Invalid workspace directory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateZipWithProgress(string sourceDirectory, string destinationZipFile, ProgressDialog progressDialog)
        {
            var files = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);
            int totalFiles = files.Length;
            using (var zipArchive = ZipFile.Open(destinationZipFile, ZipArchiveMode.Create))
            {
                for (int i = 0; i < totalFiles; i++)
                {
                    string file = files[i];
                    string entryName = Path.GetRelativePath(sourceDirectory, file);
                    zipArchive.CreateEntryFromFile(file, entryName, CompressionLevel.Optimal);

                    double progress = ((double)(i + 1) / totalFiles) * 100;
                    progressDialog.Dispatcher.Invoke(() => progressDialog.UpdateProgress(progress));
                }
            }
        }

        private void RepairWorkspace_Click(object sender, RoutedEventArgs e)
        {
            string workspacePath = WorkspaceManager.GetWorkspacePath();
            if (WorkspaceManager.IsValidWorkspace(workspacePath))
            {
                try
                {
                    WorkspaceManager.EnsureWorkspaceDirectoriesExist(workspacePath);
                    MessageBox.Show("Workspace repaired successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while repairing the workspace: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Invalid workspace directory.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshWorkspace_Click(object sender, RoutedEventArgs e)
        {

        }

        private static DateTime RetrieveLinkerTimestamp()
        {
            string filePath = Assembly.GetExecutingAssembly().Location;
            const int cPeHeaderOffset = 60;
            const int cLinkerTimestampOffset = 8;
            byte[] b = new byte[2048];
            using (Stream s = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                s.Read(b, 0, 2048);
            }
            int i = BitConverter.ToInt32(b, cPeHeaderOffset);
            int secondsSince1970 = BitConverter.ToInt32(b, i + cLinkerTimestampOffset);
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0);
            DateTime linkTimeUtc = epoch.AddSeconds(secondsSince1970);
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, TimeZoneInfo.Local);
            return localTime;
        }

        //RMS Server specific code
        private void SaveRmsSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _rmsSettingsManager.SaveRmsServerSettings(_mainViewModel.RmsSettings);
                MessageBox.Show("Settings saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void TestConnection_Click(object sender, RoutedEventArgs e)
        {
            var rmsServerSettings = _mainViewModel.RmsSettings;

            if (rmsServerSettings == null)
            {
                MessageBox.Show("Server settings not found.");
                return;
            }

            if (string.IsNullOrEmpty(rmsServerSettings.ApiUrl))
            {
                MessageBox.Show("API URL is empty.");
                return;
            }

            // Step 1: Ping the server IP
            var apiUrl = new Uri(rmsServerSettings.ApiUrl);
            var host = apiUrl.Host;
            bool isPingable = await PingHostAsync(host);
            if (!isPingable)
            {
                MessageBox.Show("Ping failed. Server is unreachable.");
                RMSStatus.Text = "NOT REACHABLE";
                return;
            }

            // Step 2: Test HTTP GET /api/ext/test
            bool isHttpSuccess = await TestHttpGetAsync(rmsServerSettings.ApiUrl);
            if (isHttpSuccess)
            {
                RMSStatus.Text = "CONNECTED";
                MessageBox.Show("Connection test successful.");

                //send stationInfo here
               //await _rmsService.UpdateStationInfoAsync(_stationInfoManager.CurrentStationInfo);
            }
            else
            {
                RMSStatus.Text = "NOT RESPONDING";
                MessageBox.Show("HTTP GET test failed.");
            }
        }


        private async Task<bool> PingHostAsync(string host)
        {
            using (var ping = new Ping())
            {
                try
                {
                    PingReply reply = await ping.SendPingAsync(host);
                    return reply.Status == IPStatus.Success;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        private async Task<bool> TestHttpGetAsync(string apiUrl)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var testUrl = $"{apiUrl}/api/ext/test";
                    var response = await client.GetAsync(testUrl);
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode && responseContent == "OK")
                    {
                        LogEvent("Hi from CDC. This is a test event log generated after successful connection test.", EventType.Information, "CDC");
                        return true;
                    }

                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        // Example method to generate a unique event ID
        private int GenerateEventId()
        {
            Random rand = new Random();
            return rand.Next(1000, 9999);  // Random 4-digit event ID
        }


        //CAP Specific code
        private void ApiKey_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                _mainViewModel.CapSettings.ApiKey = passwordBox.Password;
            }
        }

        private void CAPPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                _mainViewModel.CapSettings.Password = passwordBox.Password;
            }
        }

        private void SaveCapSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _mainViewModel.CapSettings.Password = CAPPassword.Password;
                _mainViewModel.CapSettings.ApiKey = ApiKey.Password;

                _capServerSettingsManager.SaveCapServerSettings(_mainViewModel.CapSettings);

                MessageBox.Show("CAP settings saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save CAP settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void TestCapConnection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool isConnected = _capServerSettingsManager.TestConnection(_mainViewModel.CapSettings);
                if (isConnected)
                {
                    MessageBox.Show("CAP server connection successful.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Failed to connect to CAP server.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to test CAP server connection: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        //User Categories
        private void InitializeUserCategories()
        {
            var categories = _userCategoryManager.LoadUserCategories();
            if (categories.Count == 0)
            {
                CreateDefaultCategories();
            }
            else
            {
                UpdateStatusBar("User categories loaded successfully.");
            }
        }

        private void CreateDefaultCategories()
        {
            var adminCategory = new UserCategory
            {
                Id = Guid.NewGuid(),
                Name = "Admin",
                Rights = new List<UserRights>((UserRights[])Enum.GetValues(typeof(UserRights)))
            };

            _userCategoryManager.AddUserCategory(adminCategory);

            UpdateStatusBar("Default admin category created.");
        }

        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new UserCategoryDialog();
            if (dialog.ShowDialog() == true)
            {
                _userCategoryManager.AddUserCategory(dialog.UserCategory);
                _mainViewModel.UserCategories.Add(dialog.UserCategory);
            }
        }

        private void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            if (UserCategoriesDataGrid.SelectedItem is UserCategory selectedCategory)
            {
                var dialog = new UserCategoryDialog(selectedCategory);
                if (dialog.ShowDialog() == true)
                {
                    _userCategoryManager.UpdateUserCategory(dialog.UserCategory);
                    int index = _mainViewModel.UserCategories.IndexOf(selectedCategory);
                    _mainViewModel.UserCategories[index] = dialog.UserCategory;
                }
            }
            else
            {
                MessageBox.Show("Please select a category to edit.", "Edit Category", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (UserCategoriesDataGrid.SelectedItem is UserCategory selectedCategory)
            {
                var result = MessageBox.Show($"Are you sure you want to delete the category '{selectedCategory.Name}'?", "Delete Category", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    _userCategoryManager.DeleteUserCategory(selectedCategory.Name);
                    _mainViewModel.UserCategories.Remove(selectedCategory);
                }
            }
            else
            {
                MessageBox.Show("Please select a category to delete.", "Delete Category", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // User Management
        private void AddUser_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new UserDialog(_userCategoryManager);
            if (dialog.ShowDialog() == true)
            {
                _userManager.AddUser(dialog.User);
                _mainViewModel.Users.Add(dialog.User);
            }
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is User selectedUser)
            {
                var dialog = new UserDialog(_userCategoryManager, selectedUser);
                if (dialog.ShowDialog() == true)
                {
                    _userManager.UpdateUser(dialog.User);
                    int index = _mainViewModel.Users.IndexOf(selectedUser);
                    _mainViewModel.Users[index] = dialog.User;
                }
            }
            else
            {
                MessageBox.Show("Please select a user to edit.", "Edit User", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is User selectedUser)
            {
                var result = MessageBox.Show($"Are you sure you want to delete the user '{selectedUser.Email}'?", "Delete User", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    _userManager.DeleteUser(selectedUser.Email);
                    _mainViewModel.Users.Remove(selectedUser);
                }
            }
            else
            {
                MessageBox.Show("Please select a user to delete.", "Delete User", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        private void CheckAndCreateDisplayStyles()
        {
            _displayStyleManager.DeleteAllDisplayStyles();
            var displayStyles = _displayStyleManager.LoadDisplayStyles();

            var defaultStyles = new List<DisplayStyle>
            {
                new DisplayStyle { StyleName = "English Default", Language = RegionalLanguage.ENGLISH, Font = new FontFamily(new Uri("pack://application:,,,/"), "./fonts/#Arial"), FontSize = 16, FontWeight = 0, FontStyle = 0, MarginTop = 0, MarginLeft = 0, AlignmentH = 1, AlignmentV = 1 },
                new DisplayStyle { StyleName = "Hindi Default", Language = RegionalLanguage.HINDI, Font = new FontFamily(new Uri("pack://application:,,,/"), "./fonts/#Nirmala UI"), FontSize = 16, FontWeight = 0, FontStyle = 0, MarginTop = 0, MarginLeft = 0, AlignmentH = 1, AlignmentV = 1 },
                new DisplayStyle { StyleName = "Assamese Default", Language = RegionalLanguage.ASSAMESE, Font = new FontFamily(new Uri("pack://application:,,,/"), "./fonts/#Nirmala UI"), FontSize = 16, FontWeight = 0, FontStyle = 0, MarginTop = 0, MarginLeft = 0, AlignmentH = 1, AlignmentV = 1 },
                new DisplayStyle { StyleName = "Bangla Default", Language = RegionalLanguage.BANGLA, Font = new FontFamily(new Uri("pack://application:,,,/"), "./fonts/#Nirmala UI"), FontSize = 16, FontWeight = 0, FontStyle = 0, MarginTop = 0, MarginLeft = 0, AlignmentH = 1, AlignmentV = 1 },
                new DisplayStyle { StyleName = "Bodo Default", Language = RegionalLanguage.BODO, Font = new FontFamily(new Uri("pack://application:,,,/"), "./fonts/#Nirmala UI"), FontSize = 16, FontWeight = 0, FontStyle = 0, MarginTop = 0, MarginLeft = 0, AlignmentH = 1, AlignmentV = 1 },
                new DisplayStyle { StyleName = "Dogri Default", Language = RegionalLanguage.DOGRI, Font = new FontFamily(new Uri("pack://application:,,,/"), "./fonts/#Nirmala UI"), FontSize = 16, FontWeight = 0, FontStyle = 0, MarginTop = 0, MarginLeft = 0, AlignmentH = 1, AlignmentV = 1 },
                new DisplayStyle { StyleName = "Gujarati Default", Language = RegionalLanguage.GUJARATI, Font = new FontFamily(new Uri("pack://application:,,,/"), "./fonts/#Nirmala UI"), FontSize = 16, FontWeight = 0, FontStyle = 0, MarginTop = 0, MarginLeft = 0, AlignmentH = 1, AlignmentV = 1 },
                new DisplayStyle { StyleName = "Kannada Default", Language = RegionalLanguage.KANNADA, Font = new FontFamily(new Uri("pack://application:,,,/"), "./fonts/#Nirmala UI"), FontSize = 16, FontWeight = 0, FontStyle = 0, MarginTop = 0, MarginLeft = 0, AlignmentH = 1, AlignmentV = 1 },
                new DisplayStyle { StyleName = "Kashmiri Default", Language = RegionalLanguage.KASHMIRI, Font = new FontFamily(new Uri("pack://application:,,,/"), "./fonts/#Arial"), FontSize = 16, FontWeight = 0, FontStyle = 0, MarginTop = 0, MarginLeft = 0, AlignmentH = 1, AlignmentV = 1 },
                new DisplayStyle { StyleName = "Konkani Default", Language = RegionalLanguage.KONKANI, Font = new FontFamily(new Uri("pack://application:,,,/"), "./fonts/#Nirmala UI"), FontSize = 16, FontWeight = 0, FontStyle = 0, MarginTop = 0, MarginLeft = 0, AlignmentH = 1, AlignmentV = 1 },
                new DisplayStyle { StyleName = "Malayalam Default", Language = RegionalLanguage.MALAYALAM, Font = new FontFamily(new Uri("pack://application:,,,/"), "./fonts/#Nirmala UI"), FontSize = 16, FontWeight = 0, FontStyle = 0, MarginTop = 0, MarginLeft = 0, AlignmentH = 1, AlignmentV = 1 },
                new DisplayStyle { StyleName = "Marathi Default", Language = RegionalLanguage.MARATHI, Font = new FontFamily(new Uri("pack://application:,,,/"), "./fonts/#Nirmala UI"), FontSize = 16, FontWeight = 0, FontStyle = 0, MarginTop = 0, MarginLeft = 0, AlignmentH = 1, AlignmentV = 1 },
                new DisplayStyle { StyleName = "Manipuri Default", Language = RegionalLanguage.MANIPURI, Font = new FontFamily(new Uri("pack://application:,,,/"), "./fonts/#Nirmala UI"), FontSize = 16, FontWeight = 0, FontStyle = 0, MarginTop = 0, MarginLeft = 0, AlignmentH = 1, AlignmentV = 1 },
                new DisplayStyle { StyleName = "Nepali Default", Language = RegionalLanguage.NEPALI, Font = new FontFamily(new Uri("pack://application:,,,/"), "./fonts/#Nirmala UI"), FontSize = 16, FontWeight = 0, FontStyle = 0, MarginTop = 0, MarginLeft = 0, AlignmentH = 1, AlignmentV = 1 },
                new DisplayStyle { StyleName = "Odia Default", Language = RegionalLanguage.ODIA, Font = new FontFamily(new Uri("pack://application:,,,/"), "./fonts/#Nirmala UI"), FontSize = 16, FontWeight = 0, FontStyle = 0, MarginTop = 0, MarginLeft = 0, AlignmentH = 1, AlignmentV = 1 },
                new DisplayStyle { StyleName = "Punjabi Default", Language = RegionalLanguage.PUNJABI, Font = new FontFamily(new Uri("pack://application:,,,/"), "./fonts/#Nirmala UI"), FontSize = 16, FontWeight = 0, FontStyle = 0, MarginTop = 0, MarginLeft = 0, AlignmentH = 1, AlignmentV = 1 },
                new DisplayStyle { StyleName = "Sanskrit Default", Language = RegionalLanguage.SANSKRIT, Font = new FontFamily(new Uri("pack://application:,,,/"), "./fonts/#Nirmala UI"), FontSize = 16, FontWeight = 0, FontStyle = 0, MarginTop = 0, MarginLeft = 0, AlignmentH = 1, AlignmentV = 1 },
                new DisplayStyle { StyleName = "Santhali Default", Language = RegionalLanguage.SANTHALI, Font = new FontFamily(new Uri("pack://application:,,,/"), "./fonts/#Nirmala UI"), FontSize = 16, FontWeight = 0, FontStyle = 0, MarginTop = 0, MarginLeft = 0, AlignmentH = 1, AlignmentV = 1 },
                new DisplayStyle { StyleName = "Sindhi Default", Language = RegionalLanguage.SINDHI, Font = new FontFamily(new Uri("pack://application:,,,/"), "./fonts/#Arial"), FontSize = 16, FontWeight = 0, FontStyle = 0, MarginTop = 0, MarginLeft = 0, AlignmentH = 1, AlignmentV = 1 },
                new DisplayStyle { StyleName = "Tamil Default", Language = RegionalLanguage.TAMIL, Font = new FontFamily(new Uri("pack://application:,,,/"), "./fonts/#Nirmala UI"), FontSize = 16, FontWeight = 0, FontStyle = 0, MarginTop = 0, MarginLeft = 0, AlignmentH = 1, AlignmentV = 1 },
                new DisplayStyle { StyleName = "Telugu Default", Language = RegionalLanguage.TELUGU, Font = new FontFamily(new Uri("pack://application:,,,/"), "./fonts/#Nirmala UI"), FontSize = 16, FontWeight = 0, FontStyle = 0, MarginTop = 0, MarginLeft = 0, AlignmentH = 1, AlignmentV = 1 },
                new DisplayStyle { StyleName = "Urdu Default", Language = RegionalLanguage.URDU, Font = new FontFamily(new Uri("pack://application:,,,/"), "./fonts/#Arial"), FontSize = 16, FontWeight = 0, FontStyle = 0, MarginTop = 0, MarginLeft = 0, AlignmentH = 1, AlignmentV = 1 }
            };

            foreach (var style in defaultStyles)
            {
                if (!displayStyles.Any(ds => ds.Language == style.Language))
                {
                    displayStyles.Add(style);
                }
            }

            _displayStyleManager.SaveDisplayStyles(displayStyles);
            _mainViewModel.DisplayStyles.Clear();
            foreach (var style in displayStyles)
            {
                _mainViewModel.DisplayStyles.Add(style);
            }

            if (_mainViewModel.DisplayStyles.Any())
            {
                _mainViewModel.SelectedStyle = _mainViewModel.DisplayStyles.First();
            }
        }

        private void UpdateStatusBar(string message)
        {
            tb_sb.Text = message;
        }

        private void CheckAndPromptForAdminUser()
        {
            //_userManager.DeleteAllUsers();
            _userManager.CheckAndPromptForAdminUser(_userCategoryManager, _mainViewModel);
        }

        private void CheckAndPromptForStationInfo()
        {
            if (!_stationInfoManager.IsStationInfoAvailable())
            {
                MessageBox.Show("Station information not found. Please enter the station information.", "Station Information", MessageBoxButton.OK, MessageBoxImage.Information);
                var stationInfoWindow = new StationInfoWindow(_stationInfoManager);
                bool? result = stationInfoWindow.ShowDialog();
                if (result == true)
                {

                }
                else
                {
                    MessageBox.Show("Station information is required to proceed.", "Station Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                    this.Close(); // Close the MainWindow if station info is not provided
                }
            }
            else
            {
                //_stationInfoManager.CurrentStationInfo = _stationInfoManager.GetStationInfo();
            }
            PopulateStationInfo();
        }

        private void PopulateStatusFields()
        {
            var currentUser = _userManager.CurrentUser;
            if (currentUser != null)
            {
                UserEmailStatus.Text = currentUser.Email;
                UsernameStatus.Text = currentUser.Name;
                UsertypeStatus.Text = currentUser.Category?.Name ?? "Unknown";
                // Set other fields (IpStatus, ConnectedDevicesStatus, NtesStatus) as needed
            }
        }

        private void PopulateStationInfo()
        {
            var stationInfo = _stationInfoManager.CurrentStationInfo;
            if (stationInfo != null)
            {
                //StationInfoTextBlock.Text = $"{stationInfo.StationCode} - {stationInfo.StationNameEnglish}";

                StationCodeTextBox.Text = stationInfo.StationCode;
                RegLanguageComboBox.SelectedItem = stationInfo.RegionalLanguage;
                StationNameEnTextBox.Text = stationInfo.StationNameEnglish;
                StationNameHiTextBox.Text = stationInfo.StationNameHindi;
                StationNameRLTextBox.Text = stationInfo.StationNameRegional;
                StationLatTextBox.Value = (decimal?)stationInfo.Latitude;
                StationLongTextBox.Value = (decimal?)stationInfo.Longitude;
                StationAltTextBox.Value = (decimal?)stationInfo.Altitude;
                StationPlatformsTextBox.Value = stationInfo.NumberOfPlatforms;
                NumberOfSplPlatformsTextBox.Value = stationInfo.NumberOfSplPlatforms;
                NumberOfStationEntrancesTextBox.Value = stationInfo.NumberOfStationEntrances;
                NumberOfPlatformBridgesTextBox.Value = stationInfo.NumberOfPlatformBridges;
            }
        }

        private void LoadPlatforms()
        {
            var platforms = _platformDeviceManager.LoadPlatforms();
            _mainViewModel.Platforms.Clear();
            foreach (var platform in platforms)
            {
                _mainViewModel.Platforms.Add(platform);
            }
        }

        private void AddPlatformButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new PlatformDialog();
            if (dialog.ShowDialog() == true)
            {
                var platform = new Platform
                {
                    PlatformNumber = dialog.PlatformNumber,
                    PlatformType = dialog.PlatformType,
                    Description = dialog.Description,
                    Subnet = dialog.Subnet
                };
                _platformDeviceManager.AddPlatform(platform);
                _mainViewModel.Platforms.Add(platform);
            }
        }

        private void EditPlatformButton_Click(object sender, RoutedEventArgs e)
        {
            if (_mainViewModel.SelectedPlatform != null)
            {
                var dialog = new PlatformDialog(_mainViewModel.SelectedPlatform);
                if (dialog.ShowDialog() == true)
                {
                    _mainViewModel.SelectedPlatform.PlatformNumber = dialog.PlatformNumber;
                    _mainViewModel.SelectedPlatform.PlatformType = dialog.PlatformType;
                    _mainViewModel.SelectedPlatform.Description = dialog.Description;
                    _mainViewModel.SelectedPlatform.Subnet = dialog.Subnet;
                    _platformDeviceManager.UpdatePlatform(_mainViewModel.SelectedPlatform);
                }
            }
            else
            {
                MessageBox.Show("Please select a platform to edit.", "Edit Platform", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeletePlatformButton_Click(object sender, RoutedEventArgs e)
        {
            if (_mainViewModel.SelectedPlatform != null)
            {
                var result = MessageBox.Show($"Are you sure you want to delete the platform '{_mainViewModel.SelectedPlatform.PlatformNumber}'?", "Delete Platform", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    _platformDeviceManager.DeletePlatform(_mainViewModel.SelectedPlatform.PlatformNumber);
                    _mainViewModel.Platforms.Remove(_mainViewModel.SelectedPlatform);
                }
            }
            else
            {
                MessageBox.Show("Please select a platform to delete.", "Delete Platform", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AddDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            if (_mainViewModel.SelectedPlatform != null)
            {
                var dialog = new DeviceDialog(_mainViewModel.SelectedPlatform.PlatformNumber, _mainViewModel.SelectedPlatform.Subnet);
                if (dialog.ShowDialog() == true)
                {
                    var newDevice = new Device
                    {
                        Id = GenerateDeviceId(),
                        DeviceType = dialog.DeviceType,
                        IpAddress = dialog.IpAddress,
                        IsEnabled = dialog.IsEnabled,
                        Description = dialog.Description,
                        Created = DateTime.Now,
                        Updated = DateTime.Now
                    };
                    if (_platformDeviceManager.AddDevice(_mainViewModel.SelectedPlatform.PlatformNumber, newDevice))
                    {
                        _mainViewModel.Devices.Add(newDevice);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a platform to add a device.", "Add Device", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private int GenerateDeviceId()
        {
            // Generate a new GUID and convert it to an integer hash code.
            return Guid.NewGuid().GetHashCode();
        }

        private void EditDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            if (_mainViewModel.SelectedDevice != null && _mainViewModel.SelectedPlatform != null)
            {
                var dialog = new DeviceDialog(_mainViewModel.SelectedDevice, _mainViewModel.SelectedPlatform.PlatformNumber, _mainViewModel.SelectedPlatform.Subnet);
                if (dialog.ShowDialog() == true)
                {
                    _mainViewModel.SelectedDevice.DeviceType = dialog.DeviceType;
                    _mainViewModel.SelectedDevice.IpAddress = dialog.IpAddress;
                    _mainViewModel.SelectedDevice.IsEnabled = dialog.IsEnabled;
                    _mainViewModel.SelectedDevice.Description = dialog.Description;
                    _platformDeviceManager.UpdateDevice(_mainViewModel.SelectedPlatform.PlatformNumber, _mainViewModel.SelectedDevice);
                }
            }
            else
            {
                MessageBox.Show("Please select a device to edit.", "Edit Device", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteDeviceButton_Click(object sender, RoutedEventArgs e)
        {
            if (_mainViewModel.SelectedDevice != null && _mainViewModel.SelectedPlatform != null)
            {
                var result = MessageBox.Show($"Are you sure you want to delete the device '{_mainViewModel.SelectedDevice.DeviceType}'?", "Delete Device", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    _platformDeviceManager.DeleteDevice(_mainViewModel.SelectedPlatform.PlatformNumber, _mainViewModel.SelectedDevice.Id);
                    _mainViewModel.Devices.Remove(_mainViewModel.SelectedDevice);
                }
            }
            else
            {
                MessageBox.Show("Please select a device to delete.", "Delete Device", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExportMapping_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonSaveFileDialog
            {
                Title = "Export Platform-Device Mapping",
                DefaultExtension = "json",
                Filters = { new CommonFileDialogFilter("JSON Files", "*.json") }
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string filePath = dialog.FileName;
                var platforms = _platformDeviceManager.LoadPlatforms();
                string json = JsonConvert.SerializeObject(platforms, Formatting.Indented);
                File.WriteAllText(filePath, json);
                MessageBox.Show("Platform-Device mapping exported successfully.", "Export Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ImportMapping_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                Title = "Import Platform-Device Mapping",
                Filters = { new CommonFileDialogFilter("JSON Files", "*.json") }
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string filePath = dialog.FileName;
                string json = File.ReadAllText(filePath);
                var platforms = JsonConvert.DeserializeObject<List<Platform>>(json);

                if (platforms != null)
                {
                    var result = MessageBox.Show("Do you want to overwrite the existing platform-device mapping?", "Confirm Overwrite", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        _platformDeviceManager.SavePlatforms(platforms);
                        LoadPlatforms();
                        MessageBox.Show("Platform-Device mapping imported and overwritten successfully.", "Import Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Invalid file format. Please select a valid JSON file.", "Import Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // code for dashboard
        private void InitializeDashboardComponents()
        {
            //trainList = new ObservableCollection<NtesTrain951>();
            //cgdbManagerList = new ObservableCollection<CgdbManager>();
            //refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(1) };
            //refreshTimer.Tick += RefreshTimer_Tick;
        }

        private void InitializeDateTimeUpdate()
        {
            dateTimeTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            dateTimeTimer.Tick += DateTimeTimer_Tick;
            dateTimeTimer.Start();
        }

        private void DateTimeTimer_Tick(object sender, EventArgs e)
        {
            DateTimeStatus.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        // code for pa system 
        private void InitializePAComponents()
        {
            mediaRecorder = new MediaRecorder();
            audioPlayer = new AudioPlayer();
            recordingTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            recordingTimer.Tick += RecordingTimer_Tick;
        }

        private void StartRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            //mediaRecorder.StartRecording();
            //StartRecordingButton.IsEnabled = false;
            //PauseRecordingButton.IsEnabled = true;
            //StopRecordingButton.IsEnabled = true;
            //ResumeRecordingButton.IsEnabled = false;
            //recordingTimer.Start();
        }

        private void PauseRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            //mediaRecorder.PauseRecording();
            //PauseRecordingButton.IsEnabled = false;
            //ResumeRecordingButton.IsEnabled = true;
        }

        private void ResumeRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            //mediaRecorder.ResumeRecording();
            //ResumeRecordingButton.IsEnabled = false;
            //PauseRecordingButton.IsEnabled = true;
        }

        private void StopRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            //mediaRecorder.StopRecording();
            //StartRecordingButton.IsEnabled = true;
            //PauseRecordingButton.IsEnabled = false;
            //StopRecordingButton.IsEnabled = false;
            //ResumeRecordingButton.IsEnabled = false;
            //recordingTimer.Stop();
        }

        private void RecordingTimer_Tick(object sender, EventArgs e)
        {
            ElapsedTimeTextBlock.Text = mediaRecorder.GetElapsedTime();
        }

        private void PlayAudioButton_Click(object sender, RoutedEventArgs e)
        {
            //audioPlayer.Play();
            //PlayAudioButton.IsEnabled = false;
            //PauseAudioButton.IsEnabled = true;
            //StopAudioButton.IsEnabled = true;
            //SaveAudioButton.IsEnabled = true;
        }

        private void PauseAudioButton_Click(object sender, RoutedEventArgs e)
        {
            //audioPlayer.Pause();
            //PlayAudioButton.IsEnabled = true;
            //PauseAudioButton.IsEnabled = false;
        }

        private void StopAudioButton_Click(object sender, RoutedEventArgs e)
        {
            //audioPlayer.Stop();
            //PlayAudioButton.IsEnabled = true;
            //PauseAudioButton.IsEnabled = false;
            //StopAudioButton.IsEnabled = false;
        }

        private void SaveAudioButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "WAV Files (*.wav)|*.wav|MP3 Files (*.mp3)|*.mp3"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                mediaRecorder.SaveRecording(saveFileDialog.FileName);
            }
        }

        private void AudioSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            audioPlayer.SetVolume(AudioSlider.Value);
        }

        private void MicVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Implement microphone volume adjustment
        }

        private void MicMuteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Implement microphone mute functionality
        }

        private void MicMuteCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Implement microphone unmute functionality
        }

        private void PaVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Implement PA speaker volume adjustment
        }

        private void PaMuteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Implement PA speaker mute functionality
        }

        private void PaMuteCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Implement PA speaker unmute functionality
        }

        private void LocalVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Implement local speaker volume adjustment
        }

        private void LocalMuteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Implement local speaker mute functionality
        }

        private void LocalMuteCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Implement local speaker unmute functionality
        }

        // code for multimedia
        private void AddImageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Image Files|*.jpg;*.png;*.bmp;*.gif"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var fileName in openFileDialog.FileNames)
                {
                    var imageFile = new ImageFile
                    {
                        Id = fileName,
                        Name = System.IO.Path.GetFileName(fileName),
                        FilePath = fileName,
                        Resolution = GetImageResolution(fileName)
                    };

                    // Add to MediaManager and ViewModel
                    try
                    {
                        _mediaManager.AddMediaFile(imageFile);
                        _mainViewModel.ImageFiles.Add(imageFile);
                        _mainViewModel.MediaFiles.Add(imageFile);
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                // Save the updated collection
                _mediaManager.SaveMediaFiles(_mainViewModel.MediaFiles.ToList());
            }
        }

        private void AddVideoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Video Files|*.mp4;*.avi;*.mkv;*.mov"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var fileName in openFileDialog.FileNames)
                {
                    var videoFile = new VideoFile
                    {
                        Id = fileName,
                        Name = System.IO.Path.GetFileName(fileName),
                        FilePath = fileName,
                        Resolution = GetVideoResolution(fileName),
                        Duration = GetVideoDuration(fileName),
                        ThumbnailPath = GenerateThumbnail(fileName, _workspacePath)
                    };

                    // Add to MediaManager and ViewModel
                    try
                    {
                        _mediaManager.AddMediaFile(videoFile);
                        _mainViewModel.VideoFiles.Add(videoFile);
                        _mainViewModel.MediaFiles.Add(videoFile);
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                // Save the updated collection
                //_mediaManager.SaveMediaFiles(_mainViewModel.MediaFiles.ToList());
            }
        }

        private void RemoveImageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ImageListView.SelectedItem is ImageFile selectedImage)
            {
                // Remove from MediaManager and ViewModel
                _mediaManager.DeleteMediaFile(selectedImage.Id);
                _mainViewModel.ImageFiles.Remove(selectedImage);
                _mainViewModel.MediaFiles.Remove(selectedImage);

                // Save the updated collection
                _mediaManager.SaveMediaFiles(_mainViewModel.MediaFiles.ToList());
            }
        }

        private void RemoveVideoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (VideoListView.SelectedItem is VideoFile selectedVideo)
            {
                // Remove from MediaManager and ViewModel
                _mediaManager.DeleteMediaFile(selectedVideo.Id);
                _mainViewModel.VideoFiles.Remove(selectedVideo);
                _mainViewModel.MediaFiles.Remove(selectedVideo);

                // Save the updated collection
                _mediaManager.SaveMediaFiles(_mainViewModel.MediaFiles.ToList());
            }
        }

        private void ImageListView_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var file in files)
                {
                    if (System.IO.Path.GetExtension(file).ToLower() is ".jpg" or ".png" or ".bmp" or ".gif")
                    {
                        var imageFile = new ImageFile
                        {
                            Id = file,
                            Name = System.IO.Path.GetFileName(file),
                            FilePath = file,
                            Resolution = GetImageResolution(file)
                        };

                        // Add to MediaManager and ViewModel
                        try
                        {
                            _mediaManager.AddMediaFile(imageFile);
                            _mainViewModel.ImageFiles.Add(imageFile);
                            _mainViewModel.MediaFiles.Add(imageFile);
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show(exp.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }

                // Save the updated collection
                _mediaManager.SaveMediaFiles(_mainViewModel.MediaFiles.ToList());
            }
        }

        private void VideoListView_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var file in files)
                {
                    if (System.IO.Path.GetExtension(file).ToLower() is ".mp4" or ".avi" or ".mkv" or ".mov")
                    {
                        var videoFile = new VideoFile
                        {
                            Id = file,
                            Name = System.IO.Path.GetFileName(file),
                            FilePath = file,
                            Resolution = GetVideoResolution(file),
                            Duration = GetVideoDuration(file)
                        };

                        // Add to MediaManager and ViewModel
                        try
                        {
                            _mediaManager.AddMediaFile(videoFile);
                            _mainViewModel.VideoFiles.Add(videoFile);
                            _mainViewModel.MediaFiles.Add(videoFile);
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show(exp.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }

                // Save the updated collection
                _mediaManager.SaveMediaFiles(_mainViewModel.MediaFiles.ToList());
            }
        }

        private void AddAudioMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Audio Files|*.mp3;*.wav"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var fileName in openFileDialog.FileNames)
                {
                    var audioFile = new AudioFile
                    {
                        Id = fileName,
                        Name = System.IO.Path.GetFileName(fileName),
                        FilePath = fileName,
                        Duration = GetAudioDuration(fileName),
                        BitRate = GetAudioBitRate(fileName)
                    };

                    // Add to MediaManager and ViewModel
                    try
                    {
                        _mediaManager.AddMediaFile(audioFile);
                        _mainViewModel.AudioFiles.Add(audioFile);
                        _mainViewModel.MediaFiles.Add(audioFile);
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                // Save the updated collection
                _mediaManager.SaveMediaFiles(_mainViewModel.MediaFiles.ToList());
            }
        }

        private void RemoveAudioMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (AudioListView.SelectedItem is AudioFile selectedAudio)
            {
                // Remove from MediaManager and ViewModel
                _mediaManager.DeleteMediaFile(selectedAudio.Id);
                _mainViewModel.AudioFiles.Remove(selectedAudio);
                _mainViewModel.MediaFiles.Remove(selectedAudio);

                // Save the updated collection
                _mediaManager.SaveMediaFiles(_mainViewModel.MediaFiles.ToList());
            }
        }

        private void AudioListView_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var file in files)
                {
                    if (System.IO.Path.GetExtension(file).ToLower() is ".mp3" or ".wav")
                    {
                        var audioFile = new AudioFile
                        {
                            Id = file,
                            Name = System.IO.Path.GetFileName(file),
                            FilePath = file,
                            Duration = GetAudioDuration(file),
                            BitRate = GetAudioBitRate(file)
                        };

                        // Add to MediaManager and ViewModel
                        try
                        {
                            _mediaManager.AddMediaFile(audioFile);
                            _mainViewModel.AudioFiles.Add(audioFile);
                            _mainViewModel.MediaFiles.Add(audioFile);
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show(exp.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }

                // Save the updated collection
                _mediaManager.SaveMediaFiles(_mainViewModel.MediaFiles.ToList());
            }
        }

        private TimeSpan GetAudioDuration(string filePath)
        {
            // Implement logic to get audio duration
            return TimeSpan.Zero; // Placeholder
        }

        private string GetAudioBitRate(string filePath)
        {
            // Implement logic to get audio bit rate
            return "Unknown"; // Placeholder
        }

        public TimeSpan GetVideoDuration(string filePath)
        {
            try
            {
                var inputFile = new MediaToolkit.Model.MediaFile { Filename = filePath };
                using (var engine = new Engine())
                {
                    engine.GetMetadata(inputFile);
                }
                return inputFile.Metadata.Duration;
            }
            catch
            {
                return TimeSpan.Zero;
            }
        }

        public string GetVideoResolution(string filePath)
        {
            try
            {
                var inputFile = new MediaToolkit.Model.MediaFile { Filename = filePath };
                using (var engine = new Engine())
                {
                    engine.GetMetadata(inputFile);
                }
                return $"{inputFile.Metadata.VideoData.FrameSize}";
            }
            catch
            {
                return "Unknown";
            }
        }

        public string GenerateThumbnail(string videoFilePath, string workspacePath, double frameTime = 1.0)
        {
            string internalDir = Path.Combine(workspacePath, "Internal");
            Directory.CreateDirectory(internalDir);
            string thumbnailPath = Path.Combine(internalDir, Path.GetFileNameWithoutExtension(videoFilePath) + ".jpg");

            try
            {
                var inputFile = new MediaToolkit.Model.MediaFile { Filename = videoFilePath };
                var outputFile = new MediaToolkit.Model.MediaFile { Filename = thumbnailPath };

                using (var engine = new Engine())
                {
                    engine.GetMetadata(inputFile);

                    var options = new ConversionOptions
                    {
                        Seek = TimeSpan.FromSeconds(frameTime),
                        CustomWidth = 160,
                        CustomHeight = 120
                    };

                    engine.GetThumbnail(inputFile, outputFile, options);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating thumbnail: {ex.Message}");
                return null;
            }

            return thumbnailPath;
        }

        private string GetImageResolution(string filePath)
        {
            try
            {
                var bitmap = new BitmapImage(new Uri(filePath));
                return $"{bitmap.PixelWidth}x{bitmap.PixelHeight}";
            }
            catch
            {
                return "Unknown";
            }
        }

        private void UseAudioMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UseVideoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (VideoListView.SelectedItem is VideoFile selectedVideo)
            {
                var newTimelineItem = new TimelineItem
                {
                    Name = selectedVideo.Name,
                    ItemType = TimelineItemType.Video,
                    FilePath = selectedVideo.FilePath,
                    Resolution = selectedVideo.Resolution,
                    Duration = selectedVideo.Duration,
                    ThumbnailPath = selectedVideo.ThumbnailPath, // Assuming thumbnail is already generated
                    Offset = CalculateNextItemOffset(), // Calculate start time
                    Position = _mainViewModel.TimelineItems.Count // Position at the end of the timeline
                };

                _mainViewModel.TimelineItems.Add(newTimelineItem);
                _mainViewModel.SelectedTimeline.Items.Add(newTimelineItem);
            }
        }


        private void UseImageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ImageListView.SelectedItem is ImageFile selectedImage)
            {
                var newTimelineItem = new TimelineItem
                {
                    Name = selectedImage.Name,
                    ItemType = TimelineItemType.Image,
                    FilePath = selectedImage.FilePath,
                    Resolution = selectedImage.Resolution,
                    ThumbnailPath = selectedImage.FilePath, // Assuming thumbnail is the image itself
                    Duration = TimeSpan.FromSeconds(5), // Default duration, can be adjusted
                    Offset = CalculateNextItemOffset(), // Calculate start time
                    Position = _mainViewModel.TimelineItems.Count // Position at the end of the timeline
                };

                _mainViewModel.TimelineItems.Add(newTimelineItem);
                _mainViewModel.SelectedTimeline.Items.Add(newTimelineItem);
            }
        }

        private void UseTextSlideMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (TextSlideListView.SelectedItem is TextSlideFile selectedTextSlide)
            {
                var newTimelineItem = new TimelineItem
                {
                    Name = selectedTextSlide.Name,
                    ItemType = TimelineItemType.TextSlide,
                    FilePath = selectedTextSlide.FilePath, // Assuming it was saved as an image
                    Resolution = "Unknown", // You can calculate this if needed
                    Duration = TimeSpan.FromSeconds(5), // Default duration, can be adjusted
                    Offset = CalculateNextItemOffset(), // Calculate start time
                    Position = _mainViewModel.TimelineItems.Count // Position at the end of the timeline
                };

                _mainViewModel.TimelineItems.Add(newTimelineItem);
                _mainViewModel.SelectedTimeline.Items.Add(newTimelineItem);
            }
        }

        private TimeSpan CalculateNextItemOffset()
        {
            TimeSpan offset = TimeSpan.Zero;

            foreach (var item in _mainViewModel.TimelineItems)
            {
                offset += item.Duration;
            }

            return offset;
        }

        private void EnsureDefaultTimeline()
        {
            if (_mainViewModel.SelectedTimeline == null)
            {
                var newTimeline = new Timeline
                {
                    Name = "New Timeline",
                    Created = DateTime.Now,
                    Updated = DateTime.Now
                };

                _mainViewModel.Timelines.Add(newTimeline);
                _mainViewModel.SelectedTimeline = newTimeline;
            }
        }


        private void AddTextSlideMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var ivdOvdHeight = _mainViewModel.IvdOvdHeight;
            var addTextSlideWindow = new AddTextSlideWindow(_workspacePath, 432, ivdOvdHeight);
            if (addTextSlideWindow.ShowDialog() == true)
            {
                var newTextSlide = addTextSlideWindow.TextSlide;

                _mediaManager.AddMediaFile(newTextSlide);
                _mainViewModel.TextSlideFiles.Add(newTextSlide);
            }
        }

        private void RemoveTextSlideMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (TextSlideListView.SelectedItem is TextSlideFile selectedTextSlide)
            {
                var result = MessageBox.Show("Are you sure you want to remove this text slide?", "Confirm Removal", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    _mainViewModel.TextSlideFiles.Remove(selectedTextSlide);
                    _mediaManager.DeleteMediaFile(selectedTextSlide.Id);
                }
            }
        }

        private void TextSlideListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TextSlideListView.SelectedItem is TextSlideFile selectedTextSlide)
            {
                DisplayImageInMediaElement(selectedTextSlide.FilePath);
            }
        }

        private void ImageListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ImageListView.SelectedItem is ImageFile selectedImage)
            {
                DisplayImageInMediaElement(selectedImage.FilePath);
            }
        }

        private void VideoListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VideoListView.SelectedItem is VideoFile selectedVideo)
            {
                DisplayVideoInMediaElement(selectedVideo.FilePath);
            }
        }

        private void AudioListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AudioListView.SelectedItem is AudioFile selectedAudio)
            {
                DisplayVideoInMediaElement(selectedAudio.FilePath);
            }
        }

        private void DisplayImageInMediaElement(string filePath)
        {
            PreviewMediaElement.Visibility = Visibility.Visible;
            MediaControlsPanel.Visibility = Visibility.Collapsed;
            ImageControlsPanel.Visibility = Visibility.Visible;

            // Create a BitmapImage and set it to MediaElement's Source
            BitmapImage bitmap = new BitmapImage(new Uri(filePath));
            PreviewMediaElement.Source = null;
            PreviewMediaElement.LoadedBehavior = MediaState.Manual;
            PreviewMediaElement.UnloadedBehavior = MediaState.Manual;
            PreviewMediaElement.Stretch = System.Windows.Media.Stretch.Uniform;

            // MediaElement does not directly support BitmapImage, so we need to use a trick
            PreviewMediaElement.BeginInit();
            PreviewMediaElement.Source = bitmap.UriSource;
            PreviewMediaElement.EndInit();

            ResetImageZoom();
        }

        private void DisplayVideoInMediaElement(string filePath)
        {
            PreviewMediaElement.Visibility = Visibility.Visible;
            MediaControlsPanel.Visibility = Visibility.Visible;
            ImageControlsPanel.Visibility = Visibility.Collapsed;

            PreviewMediaElement.Source = new Uri(filePath);
            PreviewMediaElement.LoadedBehavior = MediaState.Manual;
            PreviewMediaElement.UnloadedBehavior = MediaState.Manual;
            PreviewMediaElement.Stretch = System.Windows.Media.Stretch.Uniform;
            PreviewMediaElement.Play();
            _mediaTimer.Start();
        }

        //private void PlayButton_Click(object sender, RoutedEventArgs e)
        //{
        //    PreviewMediaElement.Play();
        //}

        //private void PauseButton_Click(object sender, RoutedEventArgs e)
        //{
        //    PreviewMediaElement.Pause();
        //}

        //private void StopButton_Click(object sender, RoutedEventArgs e)
        //{
        //    PreviewMediaElement.Stop();
        //    _mediaTimer.Stop();
        //}

        private void MediaTimer_Tick(object sender, EventArgs e)
        {
            if (PreviewMediaElement.Source != null && PreviewMediaElement.NaturalDuration.HasTimeSpan)
            {
                MediaSlider.Minimum = 0;
                MediaSlider.Maximum = PreviewMediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                MediaSlider.Value = PreviewMediaElement.Position.TotalSeconds;
                PlayedDurationText.Text = PreviewMediaElement.Position.ToString(@"hh\:mm\:ss");
                TotalDurationText.Text = PreviewMediaElement.NaturalDuration.TimeSpan.ToString(@"hh\:mm\:ss");
            }
        }

        private void MediaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PreviewMediaElement.Source != null)
            {
                PreviewMediaElement.Position = TimeSpan.FromSeconds(MediaSlider.Value);
            }
        }

        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            _imageZoom += 0.1;
            PreviewMediaElement.LayoutTransform = new ScaleTransform(_imageZoom, _imageZoom);
        }

        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            if (_imageZoom > 0.1)
            {
                _imageZoom -= 0.1;
                PreviewMediaElement.LayoutTransform = new ScaleTransform(_imageZoom, _imageZoom);
            }
        }

        private void ResetImageZoom()
        {
            _imageZoom = 1.0;
            PreviewMediaElement.LayoutTransform = new ScaleTransform(_imageZoom, _imageZoom);
        }

        // code for rms server


        // code for platform manager
        private void AddDisplayButton_Click(object sender, RoutedEventArgs e)
        {
            //var newDisplay = new Display
            //{
            //    Id = DateTime.Now.Ticks.ToString(),
            //    Created = DateTime.Now,
            //    Updated = DateTime.Now,
            //    Type = "SLDB",
            //    Lines = 1,
            //    IpAddr = CalculateIpAddr("SLDB", Displays.Count),
            //    Enabled = true,
            //    Pingable = false,
            //    LastPingOk = "",
            //    LastPingAttempt = "",
            //    Displaying = false,
            //    LastDisplayingOk = "",
            //    LastDisplayingAttempt = ""
            //};
            //Displays.Add(newDisplay);
            //UpdateDisplayFormGroup();
            //ShowSnackbar("Display added.");
        }

        private void RemoveDisplayButton_Click(object sender, RoutedEventArgs e)
        {
            //if (SelectedDisplay != null)
            //{
            //    Displays.Remove(SelectedDisplay);
            //    SelectedDisplay = null;
            //    UpdateDisplayFormGroup();
            //    ShowSnackbar("Display removed successfully.");
            //}
        }

        private void SaveDisplaysButton_Click(object sender, RoutedEventArgs e)
        {
            // Implement logic to save displays to the backend
            MessageBox.Show("Platform information saved successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private string CalculateIpAddr(string type, int index)
        {
            string baseIp = "192.168.0.";
            return baseIp + (160 + index).ToString();
        }

        // code for station info
        private void SaveStationInfoButton_Click(object sender, RoutedEventArgs e)
        {
            //var stationInfo = new StationInfo
            //{
            //    StationCode = StationCodeTextBox.Text,
            //    RegLanguage = (RegLanguageComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
            //    StationNameEn = StationNameEnTextBox.Text,
            //    StationNameHi = StationNameHiTextBox.Text,
            //    StationNameRL = StationNameRLTextBox.Text,
            //    StationLat = StationLatTextBox.Text,
            //    StationLong = StationLongTextBox.Text,
            //    StationAlt = StationAltTextBox.Text,
            //    StationPlatforms = StationPlatformsTextBox.Text
            //};

            // Implement logic to save station info to the backend
            MessageBox.Show("Station information saved successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // code for user management section
        //private void AddUserButton_Click(object sender, RoutedEventArgs e)
        //{
        //    CurrentUser = null;
        //    ShowUserForm();
        //}

        private void EditUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is User user)
            {
                CurrentUser = user;
                ShowUserForm();
            }
        }

        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is User user)
            {
                Users.Remove(user);
                // Implement logic to delete user from the backend
                MessageBox.Show($"User {user.Email} deleted successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SaveUserButton_Click(object sender, RoutedEventArgs e)
        {
            //if (CurrentUser == null)
            //{
            //    // Add new user
            //    var newUser = new User
            //    {
            //        Email = EmailTextBox.Text,
            //        Name = NameTextBox.Text,
            //        Phone = PhoneTextBox.Text,
            //        Designation = DesignationTextBox.Text,
            //        Type = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
            //        Password = PasswordTextBox.Text,
            //        IsActive = IsActiveCheckBox.IsChecked ?? false,
            //        CreatedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            //        LastLogin = ""
            //    };

            //    Users.Add(newUser);
            //    // Implement logic to save user to the backend
            //    MessageBox.Show("User added successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            //}
            //else
            //{
            //    // Update existing user
            //    CurrentUser.Email = EmailTextBox.Text;
            //    CurrentUser.Name = NameTextBox.Text;
            //    CurrentUser.Phone = PhoneTextBox.Text;
            //    CurrentUser.Designation = DesignationTextBox.Text;
            //    CurrentUser.Type = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            //    CurrentUser.Password = PasswordTextBox.Text;
            //    CurrentUser.IsActive = IsActiveCheckBox.IsChecked ?? false;

            //    // Implement logic to update user in the backend
            //    MessageBox.Show("User updated successfully.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            //}

            //HideUserForm();
        }

        private void ToggleActiveCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is User user)
            {
                user.IsActive = checkBox.IsChecked ?? false;
                // Implement logic to update user's active status in the backend
                MessageBox.Show($"User {user.Email} active status updated.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ShowUserForm()
        {
            //UserFormGrid.Visibility = Visibility.Visible;
            //if (CurrentUser != null)
            //{
            //    EmailTextBox.Text = CurrentUser.Email;
            //    NameTextBox.Text = CurrentUser.Name;
            //    PhoneTextBox.Text = CurrentUser.Phone;
            //    DesignationTextBox.Text = CurrentUser.Designation;
            //    TypeComboBox.SelectedItem = TypeComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(item => item.Content.ToString() == CurrentUser.Type);
            //    PasswordTextBox.Text = CurrentUser.Password;
            //    IsActiveCheckBox.IsChecked = CurrentUser.IsActive;
            //}
            //else
            //{
            //    EmailTextBox.Clear();
            //    NameTextBox.Clear();
            //    PhoneTextBox.Clear();
            //    DesignationTextBox.Clear();
            //    TypeComboBox.SelectedIndex = 0;
            //    PasswordTextBox.Clear();
            //    IsActiveCheckBox.IsChecked = true;
            //}
        }

        //// code for CAP section

        private void SaveConfigurationButton_Click(object sender, RoutedEventArgs e)
        {
            //string apiEndpoint = ApiEndpointTextBox.Text;
            //int alertDisplayTime = int.TryParse(AlertDisplayTimeTextBox.Text, out var displayTime) ? displayTime : 30;
            //int alertRepetitionInterval = int.TryParse(AlertRepetitionIntervalTextBox.Text, out var repetitionInterval) ? repetitionInterval : 60;
            //int messageLength = int.TryParse(MessageLengthTextBox.Text, out var maxLength) ? maxLength : 306;
            //string language = LanguageTextBox.Text;

            //// Save configuration settings (backend implementation to be done later)
            //MessageBox.Show($"Configuration Saved:\nAPI Endpoint: {apiEndpoint}\nAlert Display Time: {alertDisplayTime} seconds\nAlert Repetition Interval: {alertRepetitionInterval} seconds\nMaximum Message Length: {messageLength} characters\nDisplay Language: {language}", "CAP Setup", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //code for logs section


        //code for backup
        private void BackupLocationRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                if (radioButton.Content.ToString() == "Local Storage")
                {
                    BackupPathTextBox.IsEnabled = true;
                    BackupUrlTextBox.IsEnabled = false;
                }
                else if (radioButton.Content.ToString() == "Cloud Storage")
                {
                    BackupPathTextBox.IsEnabled = false;
                    BackupUrlTextBox.IsEnabled = true;
                }
            }
        }

        private void BrowseBackupLocationButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                CheckFileExists = false,
                CheckPathExists = true,
                ValidateNames = false,
                FileName = "Select Folder"
            };
            if (dialog.ShowDialog() == true)
            {
                BackupPathTextBox.Text = System.IO.Path.GetDirectoryName(dialog.FileName);
            }
        }

        private void BackupNowButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BackupPathTextBox.Text) && string.IsNullOrWhiteSpace(BackupUrlTextBox.Text))
            {
                MessageBox.Show("Please specify a backup location.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Implement the backup logic here

            MessageBox.Show("Backup completed successfully.", "Backup", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SetScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            if (FrequencyComboBox.SelectedItem == null || BackupTimePicker.Value == null)
            {
                MessageBox.Show("Please specify the frequency and backup time.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get selected time
            string selectedTime = BackupTimePicker.Value.Value.ToString("HH:mm");

            // Implement the scheduling logic here

            MessageBox.Show("Backup schedule set successfully.", "Schedule", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            //if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            //{
            //    MessageBox.Show("Please specify an email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    return;
            //}

            //// Implement the notification settings logic here

            //MessageBox.Show("Notification settings saved successfully.", "Settings", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //code for restore option
        private void RestoreNowButton_Click(object sender, RoutedEventArgs e)
        {
            if (RestorePointComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a restore point.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Implement the restore logic here
            MessageBox.Show("Restore completed successfully.", "Restore", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DownloadLocalBackupButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var backupItem = button?.DataContext as BackupItem;

            // Implement the download logic here
            MessageBox.Show($"Downloaded local backup: {backupItem?.BackupDate}", "Download", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteLocalBackupButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var backupItem = button?.DataContext as BackupItem;

            // Implement the delete logic here
            if (backupItem != null)
            {
                LocalBackups.Remove(backupItem);
                MessageBox.Show($"Deleted local backup: {backupItem.BackupDate}", "Delete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DownloadCloudBackupButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var backupItem = button?.DataContext as BackupItem;

            // Implement the download logic here
            MessageBox.Show($"Downloaded cloud backup: {backupItem?.BackupDate}", "Download", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteCloudBackupButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var backupItem = button?.DataContext as BackupItem;

            // Implement the delete logic here
            if (backupItem != null)
            {
                CloudBackups.Remove(backupItem);
                MessageBox.Show($"Deleted cloud backup: {backupItem.BackupDate}", "Delete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }



        //code for ticketing system
        private void BrowseFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                // Handle the file path (e.g., save it for later use when submitting the ticket)
            }
        }

        private void SubmitTicketButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleTextBox.Text;
            string description = DescriptionTextBox.Text;
            string category = (CategoryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            // Validate inputs
            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Please enter a title.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                MessageBox.Show("Please enter a description.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(category))
            {
                MessageBox.Show("Please select a category.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Create a new ticket and add it to the collection
            Tickets.Add(new Ticket
            {
                Title = title,
                Description = description,
                Category = category,
                Status = "New", // Initial status
                Timestamp = DateTime.Now.ToString("g") // Add timestamp
            });

            // Clear the form
            TitleTextBox.Clear();
            DescriptionTextBox.Clear();
            CategoryComboBox.SelectedIndex = -1;
        }

        private void UpdateStationInfoButton_Click(object sender, RoutedEventArgs e)
        {
            var stationInfo = new StationInfo
            {
                StationCode = StationCodeTextBox.Text,
                RegionalLanguage = (RegionalLanguage)RegLanguageComboBox.SelectedItem,
                StationNameEnglish = StationNameEnTextBox.Text,
                StationNameHindi = StationNameHiTextBox.Text,
                StationNameRegional = StationNameRLTextBox.Text,
                Latitude = (double)(StationLatTextBox.Value ?? 0),
                Longitude = (double)(StationLongTextBox.Value ?? 0),
                Altitude = (double)(StationAltTextBox.Value ?? 0),
                NumberOfPlatforms = StationPlatformsTextBox.Value ?? 0,
                NumberOfSplPlatforms = NumberOfSplPlatformsTextBox.Value ?? 0,
                NumberOfStationEntrances = NumberOfStationEntrancesTextBox.Value ?? 0,
                NumberOfPlatformBridges = NumberOfPlatformBridgesTextBox.Value ?? 0
            };

            _stationInfoManager.SaveStationInfo(stationInfo);
            MessageBox.Show("Station information updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SettingsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SettingsListBox.SelectedItem is ListBoxItem selectedItem)
            {
                string selectedSetting = selectedItem.Content.ToString();
                string settingContent = string.Empty;

                switch (selectedSetting)
                {
                    case "workspacePath":
                        settingContent = _workspacePath;
                        break;
                    case "userCategories":
                        settingContent = JsonConvert.SerializeObject(_userCategoryManager.LoadUserCategories(), Formatting.None);
                        break;
                    case "users":
                        settingContent = JsonConvert.SerializeObject(_userManager.LoadUsers(), Formatting.Indented);
                        break;
                    case "stationInfo":
                        settingContent = JsonConvert.SerializeObject(_stationInfoManager.CurrentStationInfo, Formatting.Indented);
                        break;
                    case "platformInfo":
                        settingContent = JsonConvert.SerializeObject(_platformDeviceManager.CurrentPlatformInfo, Formatting.Indented);
                        break;
                    case "displayStyles":
                        settingContent = JsonConvert.SerializeObject(_displayStyleManager.LoadDisplayStyles(), Formatting.Indented);
                        break;
                    case "displaySettings":
                        settingContent = JsonConvert.SerializeObject(_trainStatusDisplayManager.LoadDisplaySettings(), Formatting.Indented);
                        break;
                    case "audioSettings":
                        settingContent = JsonConvert.SerializeObject(_audioSettingsManager.LoadAudioSettings(), Formatting.Indented);
                        break;
                    case "rmsSettings":
                        settingContent = JsonConvert.SerializeObject(_rmsSettingsManager.LoadRmsServerSettings(), Formatting.Indented);
                        break;
                    case "mediaFiles":
                        settingContent = JsonConvert.SerializeObject(_mediaManager.LoadMediaFiles(), Formatting.Indented);
                        break;
                    case "timelines":
                        settingContent = JsonConvert.SerializeObject(_timelineManager.LoadTimelines(), Formatting.Indented);
                        break;
                    case "stations":
                        settingContent = JsonConvert.SerializeObject(_stationManager.LoadStations(), Formatting.Indented);
                        break;

                }

                SettingsContentTextBox.Text = settingContent;
            }
        }

        //Test Tool Specific Code
        private void cb_cgdb_checked(object sender, RoutedEventArgs e)
        {
            cb_pfdb.IsChecked = false;
            cb_agdb.IsChecked = false;
            cb_sldb.IsChecked = false;
            cb_sldb_rgb.IsChecked = false;
            height = 16;
            width = 96;
            canvas = lb_canvas_cgdb;
            dType = 1;
        }

        private void cb_pfdb_checked(object sender, RoutedEventArgs e)
        {
            cb_cgdb.IsChecked = false;
            cb_agdb.IsChecked = false;
            cb_sldb.IsChecked = false;
            cb_sldb_rgb.IsChecked = false;
            height = 32;
            width = 192;
            canvas = lb_canvas_pfdb;
            dType = 2;
        }

        private void cb_agdb_checked(object sender, RoutedEventArgs e)
        {
            cb_pfdb.IsChecked = false;
            cb_cgdb.IsChecked = false;
            cb_sldb.IsChecked = false;
            cb_sldb_rgb.IsChecked = false;
            height = 32;
            width = 192;
            canvas = lb_canvas_agdb;
            dType = 2;
        }

        private void cb_sldb_checked(object sender, RoutedEventArgs e)
        {
            cb_cgdb.IsChecked = false;
            cb_pfdb.IsChecked = false;
            cb_agdb.IsChecked = false;
            cb_sldb_rgb.IsChecked = false;
            height = 16;
            width = 336;
            canvas = lb_canvas_sldb;
            dType = 0;
        }

        private void cb_sldb_rgb_checked(object sender, RoutedEventArgs e)
        {
            cb_cgdb.IsChecked = false;
            cb_pfdb.IsChecked = false;
            cb_agdb.IsChecked = false;
            height = 16;
            width = 336;
            canvas = lb_canvas_sldb_rgb;
            dType = 9;
        }

        object ByteArrayToStructure(byte[] bytearray, object structureObj, int position)
        {
            int length = Marshal.SizeOf(structureObj);
            IntPtr ptr = Marshal.AllocHGlobal(length);
            Marshal.Copy(bytearray, 0, ptr, length);
            structureObj = Marshal.PtrToStructure(Marshal.UnsafeAddrOfPinnedArrayElement(bytearray, position), structureObj.GetType());
            Marshal.FreeHGlobal(ptr);
            return structureObj;
        }

        byte[] StructureToByteArray(object obj)
        {
            int len = Marshal.SizeOf(obj);
            byte[] arr = new byte[len];
            IntPtr ptr = Marshal.AllocHGlobal(len);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, len);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        public object RunOnUiThread(Delegate method)
        {
            return Dispatcher.Invoke(DispatcherPriority.Normal, method);
        }

        private void bt_connect_send(object sender, RoutedEventArgs e)
        {
            bt_connect_sendAsync(sender, e);
        }

        private async Task bt_connect_sendAsync(object sender, RoutedEventArgs e)
        {
            if (tb_ipAddr.Text == "")
            {
                return;
            }
            else
            if (cb_sldb.IsChecked == false && cb_agdb.IsChecked == false && cb_pfdb.IsChecked == false && cb_cgdb.IsChecked == false)
            {
                return;
            }
            else
            {
                Mouse.OverrideCursor = Cursors.Wait;
                bool error;
                try
                {
                    SessionVars.ipAddr = tb_ipAddr.Text;
                    SessionVars.tcpPort = (Int32)ud_tcpPort.Value;
                    SessionVars.tcpClient = new TcpClient(SessionVars.ipAddr, SessionVars.tcpPort);
                    SessionVars.nStream = SessionVars.tcpClient.GetStream();


                    if (cb_effects.SelectedIndex == 1)
                    {
                        //scroll
                        error = false;
                        DispDataIPISs = new DispDataIPISs_t();
                        DispDataIPISs.header = 0xCAFEBABE;
                        DispDataIPISs.disp_width = (UInt32)width;
                        DispDataIPISs.disp_height = (UInt32)height;
                        DispDataIPISs.disp_type = dType;

                        DispDataIPISs.transition = (uint)cb_transitions.SelectedIndex;
                        DispDataIPISs.transition_speed = (uint)cb_transition_speed.SelectedIndex;
                        DispDataIPISs.effect = (uint)cb_effects.SelectedIndex;
                        DispDataIPISs.effect_speed = (uint)cb_effect_speed.SelectedIndex;

                        DispDataIPISs.brghtLevel = (UInt32)sl_brght.Value;

                        RenderTargetBitmap rtb = new RenderTargetBitmap((int)canvas.RenderSize.Width,
                        (int)canvas.RenderSize.Height, 96d, 96d, PixelFormats.Pbgra32);
                        canvas.Measure(new System.Windows.Size((int)width, (int)height));
                        canvas.Arrange(new Rect(new System.Windows.Size((int)width, (int)height)));

                        rtb.Render(canvas);

                        BitmapEncoder bitmapEncoder = new BmpBitmapEncoder();
                        bitmapEncoder.Frames.Add(BitmapFrame.Create(rtb));

                        Bitmap bitmap;
                        using (MemoryStream outStream = new MemoryStream())
                        {
                            bitmapEncoder.Save(outStream);
                            bitmap = new Bitmap(outStream);
                        }

                        int wd = (int)width;
                        int ht = (int)height;

                        //TBD
                        //extract pixel data, encode in bitstream
                        byte[] r_buf = new byte[wd * ht / 8];
                        int bufIdx = 0;
                        int bufIdxCtr = 7;
                        byte r_temp;

                        ushort[] r_buf16 = new ushort[r_buf.Length / 2];
                        bufIdxCtr = 15;
                        //extract row wise data

                        //parallel
                        //for (int y = 0; y < ht; y++)
                        //{
                        //    for (int x = 0; x < wd; x++)
                        //    {
                        //        if (bitmap.GetPixel(x, y).R != 0 || bitmap.GetPixel(x, y).G != 0 || bitmap.GetPixel(x, y).B != 0)
                        //        {
                        //            r_temp = 1;
                        //        }
                        //        else
                        //        {
                        //            r_temp = 0;
                        //        }

                        //        r_buf16[bufIdx] |= (ushort)(r_temp << bufIdxCtr);
                        //        bufIdxCtr--;
                        //        if (bufIdxCtr < 0)
                        //        {
                        //            bufIdxCtr = 15;
                        //            bufIdx++;
                        //            Console.WriteLine("x: " + x + "y: " + y + " Index: " + bufIdx);
                        //        }
                        //    }
                        //}

                        //zigzag
                        for (int y = 0; y < 16; y++)
                        {
                            for (int z = 16 * ((ht / 16) - 1); z >= 0; z -= 16)
                            {
                                for (int x = 0; x < wd; x++)
                                {
                                    //if ((bitmap.GetPixel(x, z + y).R) == 255)
                                    if (bitmap.GetPixel(x, z + y).R != 0 || bitmap.GetPixel(x, z + y).G != 0 || bitmap.GetPixel(x, z + y).B != 0)
                                    {
                                        r_temp = 1;
                                    }
                                    else
                                    {
                                        r_temp = 0;
                                    }

                                    r_buf16[bufIdx] |= (ushort)(r_temp << bufIdxCtr);

                                    bufIdxCtr--;
                                    if (bufIdxCtr < 0)
                                    {
                                        bufIdxCtr = 15;
                                        bufIdx++;
                                    }
                                }
                            }
                        }

                        //copy
                        for (int i = 0; i < r_buf16.Length; i++)
                        {
                            byte[] bytes = BitConverter.GetBytes(r_buf16[i]);
                            if (BitConverter.IsLittleEndian)
                            {
                                r_buf[i * 2] = bytes[0]; // Lower byte
                                r_buf[i * 2 + 1] = bytes[1]; // Upper byte
                            }
                            else
                            {
                                r_buf[i * 2] = bytes[1]; // Upper byte
                                r_buf[i * 2 + 1] = bytes[0]; // Lower byte
                            }
                        }

                        DispDataIPISs.dp_buf = new byte[19200];

                        for (int idx = 0; idx < (wd * ht / 8); idx++)
                        {
                            DispDataIPISs.dp_buf[idx] = r_buf[idx];
                        }

                        if (true)
                        {
                            byte[] blobData = StructureToByteArray(DispDataIPISs);

                            CmdPacket = new CmdPacket_t();
                            CmdPacket.header = 0x5566AABB;
                            CmdPacket.size = (UInt32)blobData.Length;
                            CmdPacket.cmd = 0x0000000D;
                            CmdPacket.info = 1;
                            CmdPacket.crc = CmdPacket.size ^ CmdPacket.cmd ^ CmdPacket.info;
                            byte[] blobCmd = StructureToByteArray(CmdPacket);
                            SessionVars.nStream.Write(blobCmd, 0, blobCmd.Length);
                            await Task.Delay(TimeSpan.FromSeconds(1));
                            byte[] blobRep = new byte[4];
                            SessionVars.nStream.Read(blobRep, 0, blobRep.Length);

                            if (Encoding.ASCII.GetString(blobRep) == "#OK*")
                            {
                                int offset = 0;
                                int bytes;
                                while (offset < blobData.Length)
                                {
                                    await Task.Delay(TimeSpan.FromSeconds(0.5));

                                    if (blobData.Length - offset >= 1024)
                                    {
                                        bytes = 1024;
                                    }
                                    else
                                    {
                                        bytes = blobData.Length - offset;
                                    }
                                    SessionVars.nStream.Write(blobData, offset, bytes);
                                    offset += bytes;
                                }

                                SessionVars.nStream.Read(blobRep, 0, blobRep.Length);

                                if (Encoding.ASCII.GetString(blobRep) == "#OK*")
                                {
                                    //good to go

                                }
                                else
                                if (Encoding.ASCII.GetString(blobRep) == "#ER*")
                                {
                                    //good to go

                                }
                            }
                            else
                            {
                                //some issue
                                MessageBox.Show(" Communication Error while Flash Image Sent to Display", "Steady Image", MessageBoxButton.OK, MessageBoxImage.Error);
                                error = true;
                                Mouse.OverrideCursor = null;
                            }
                        }

                        SessionVars.nStream.Close();
                        SessionVars.tcpClient.Close();
                        Mouse.OverrideCursor = null;
                    }
                    else
                    {
                        if (dType == 9)
                        {
                            //steady or flashing
                            error = false;
                            DispDataIPISrgb = new DispDataIPISrgb_t();
                            DispDataIPISrgb.header = 0xCAFEBABE;
                            DispDataIPISrgb.disp_width = (UInt32)width;
                            DispDataIPISrgb.disp_height = (UInt32)height;
                            DispDataIPISrgb.disp_type = dType;

                            DispDataIPISrgb.transition = (uint)cb_transitions.SelectedIndex;
                            DispDataIPISrgb.transition_speed = (uint)cb_transition_speed.SelectedIndex;
                            DispDataIPISrgb.effect = (uint)cb_effects.SelectedIndex;
                            DispDataIPISrgb.effect_speed = (uint)cb_effect_speed.SelectedIndex;

                            DispDataIPISrgb.brghtLevel = (UInt32)sl_brght.Value;

                            RenderTargetBitmap rtb = new RenderTargetBitmap((int)canvas.RenderSize.Width,
                            (int)canvas.RenderSize.Height, 96d, 96d, PixelFormats.Pbgra32);
                            canvas.Measure(new System.Windows.Size((int)width, (int)height));
                            canvas.Arrange(new Rect(new System.Windows.Size((int)width, (int)height)));

                            rtb.Render(canvas);

                            BitmapEncoder bitmapEncoder = new BmpBitmapEncoder();
                            bitmapEncoder.Frames.Add(BitmapFrame.Create(rtb));

                            Bitmap bitmap;
                            using (MemoryStream outStream = new MemoryStream())
                            {
                                bitmapEncoder.Save(outStream);
                                bitmap = new Bitmap(outStream);
                            }

                            int wd = (int)width;
                            int ht = (int)height;

                            //TBD
                            //extract pixel data, encode in bitstream
                            byte[] r_buf = new byte[wd * ht / 8];
                            int bufIdx = 0;
                            int bufIdxCtr = 7;
                            byte r_temp;

                            ushort[] r_buf16 = new ushort[r_buf.Length / 2];
                            bufIdxCtr = 15;
                            //extract row wise data
                            for (int y = 0; y < ht; y++)
                            {
                                for (int x = 0; x < wd; x++)
                                {
                                    if (bitmap.GetPixel(x, y).R != 0 || bitmap.GetPixel(x, y).G != 0 || bitmap.GetPixel(x, y).B != 0)
                                    {
                                        r_temp = 1;
                                    }
                                    else
                                    {
                                        r_temp = 0;
                                    }

                                    r_buf16[bufIdx] |= (ushort)(r_temp << bufIdxCtr);
                                    bufIdxCtr--;
                                    if (bufIdxCtr < 0)
                                    {
                                        bufIdxCtr = 15;
                                        bufIdx++;
                                        Console.WriteLine("x: " + x + "y: " + y + " Index: " + bufIdx);
                                    }
                                }
                            }

                            //copy
                            for (int i = 0; i < r_buf16.Length; i++)
                            {
                                byte[] bytes = BitConverter.GetBytes(r_buf16[i]);
                                if (BitConverter.IsLittleEndian)
                                {
                                    r_buf[i * 2] = bytes[0]; // Lower byte
                                    r_buf[i * 2 + 1] = bytes[1]; // Upper byte
                                }
                                else
                                {
                                    r_buf[i * 2] = bytes[1]; // Upper byte
                                    r_buf[i * 2 + 1] = bytes[0]; // Lower byte
                                }
                            }

                            DispDataIPISrgb.dp_buf = new UInt64[6912];

                            for (int idx = 0; idx < (wd * ht / 8); idx++)
                            {
                                DispDataIPISrgb.dp_buf[idx] = r_buf[idx];
                            }

                            if (true)
                            {
                                byte[] blobData = StructureToByteArray(DispDataIPISrgb);

                                CmdPacket = new CmdPacket_t();
                                CmdPacket.header = 0x5566AABB;
                                CmdPacket.size = (UInt32)blobData.Length;
                                CmdPacket.cmd = 0x0000000C;
                                CmdPacket.info = 1;
                                CmdPacket.crc = CmdPacket.size ^ CmdPacket.cmd ^ CmdPacket.info;
                                byte[] blobCmd = StructureToByteArray(CmdPacket);
                                SessionVars.nStream.Write(blobCmd, 0, blobCmd.Length);
                                await Task.Delay(TimeSpan.FromSeconds(1));
                                byte[] blobRep = new byte[4];
                                SessionVars.nStream.Read(blobRep, 0, blobRep.Length);

                                if (Encoding.ASCII.GetString(blobRep) == "#OK*")
                                {
                                    int offset = 0;
                                    int bytes;
                                    while (offset < blobData.Length)
                                    {
                                        await Task.Delay(TimeSpan.FromSeconds(0.5));

                                        if (blobData.Length - offset >= 1024)
                                        {
                                            bytes = 1024;
                                        }
                                        else
                                        {
                                            bytes = blobData.Length - offset;
                                        }
                                        SessionVars.nStream.Write(blobData, offset, bytes);
                                        offset += bytes;
                                    }

                                    SessionVars.nStream.Read(blobRep, 0, blobRep.Length);

                                    if (Encoding.ASCII.GetString(blobRep) == "#OK*")
                                    {
                                        //good to go

                                    }
                                    else
                                    if (Encoding.ASCII.GetString(blobRep) == "#ER*")
                                    {
                                        //good to go

                                    }
                                }
                                else
                                {
                                    //some issue
                                    MessageBox.Show(" Communication Error while Flash Image Sent to Display", "Steady Image", MessageBoxButton.OK, MessageBoxImage.Error);
                                    error = true;
                                    Mouse.OverrideCursor = null;
                                }
                            }

                            SessionVars.nStream.Close();
                            SessionVars.tcpClient.Close();
                            Mouse.OverrideCursor = null;
                        }
                        else
                        {
                            //steady or flashing
                            error = false;
                            DispDataIPIS = new DispDataIPIS_t();
                            DispDataIPIS.header = 0xCAFEBABE;
                            DispDataIPIS.disp_width = (UInt32)width;
                            DispDataIPIS.disp_height = (UInt32)height;
                            DispDataIPIS.disp_type = dType;

                            DispDataIPIS.transition = (uint)cb_transitions.SelectedIndex;
                            DispDataIPIS.transition_speed = (uint)cb_transition_speed.SelectedIndex;
                            DispDataIPIS.effect = (uint)cb_effects.SelectedIndex;
                            DispDataIPIS.effect_speed = (uint)cb_effect_speed.SelectedIndex;

                            DispDataIPIS.brghtLevel = (UInt32)sl_brght.Value;

                            RenderTargetBitmap rtb = new RenderTargetBitmap((int)canvas.RenderSize.Width,
                            (int)canvas.RenderSize.Height, 96d, 96d, PixelFormats.Pbgra32);
                            canvas.Measure(new System.Windows.Size((int)width, (int)height));
                            canvas.Arrange(new Rect(new System.Windows.Size((int)width, (int)height)));

                            rtb.Render(canvas);

                            BitmapEncoder bitmapEncoder = new BmpBitmapEncoder();
                            bitmapEncoder.Frames.Add(BitmapFrame.Create(rtb));

                            Bitmap bitmap;
                            using (MemoryStream outStream = new MemoryStream())
                            {
                                bitmapEncoder.Save(outStream);
                                bitmap = new Bitmap(outStream);
                            }

                            int wd = (int)width;
                            int ht = (int)height;

                            //TBD
                            //extract pixel data, encode in bitstream
                            byte[] r_buf = new byte[wd * ht / 8];
                            int bufIdx = 0;
                            int bufIdxCtr = 7;
                            byte r_temp;

                            ushort[] r_buf16 = new ushort[r_buf.Length / 2];
                            bufIdxCtr = 15;
                            //extract row wise data
                            //for (int y = 0; y < ht; y++)
                            //{
                            //    for (int x = 0; x < wd; x++)
                            //    {
                            //        if (bitmap.GetPixel(x, y).R != 0 || bitmap.GetPixel(x, y).G != 0 || bitmap.GetPixel(x, y).B != 0)
                            //        {
                            //            r_temp = 1;
                            //        }
                            //        else
                            //        {
                            //            r_temp = 0;
                            //        }

                            //        r_buf16[bufIdx] |= (ushort)(r_temp << bufIdxCtr);
                            //        bufIdxCtr--;
                            //        if (bufIdxCtr < 0)
                            //        {
                            //            bufIdxCtr = 15;
                            //            bufIdx++;
                            //            Console.WriteLine("x: " + x + "y: " + y + " Index: " + bufIdx);
                            //        }
                            //    }
                            //}

                            //zigzag
                            for (int y = 0; y < 16; y++)
                            {
                                for (int z = 16 * ((ht / 16) - 1); z >= 0; z -= 16)
                                {
                                    for (int x = 0; x < wd; x++)
                                    {
                                        //if ((bitmap.GetPixel(x, z + y).R) == 255)
                                        if (bitmap.GetPixel(x, z + y).R != 0 || bitmap.GetPixel(x, z + y).G != 0 || bitmap.GetPixel(x, z + y).B != 0)
                                        {
                                            r_temp = 1;
                                        }
                                        else
                                        {
                                            r_temp = 0;
                                        }

                                        r_buf16[bufIdx] |= (ushort)(r_temp << bufIdxCtr);

                                        bufIdxCtr--;
                                        if (bufIdxCtr < 0)
                                        {
                                            bufIdxCtr = 15;
                                            bufIdx++;
                                        }
                                    }
                                }
                            }

                            //copy
                            for (int i = 0; i < r_buf16.Length; i++)
                            {
                                byte[] bytes = BitConverter.GetBytes(r_buf16[i]);
                                if (BitConverter.IsLittleEndian)
                                {
                                    r_buf[i * 2] = bytes[0]; // Lower byte
                                    r_buf[i * 2 + 1] = bytes[1]; // Upper byte
                                }
                                else
                                {
                                    r_buf[i * 2] = bytes[1]; // Upper byte
                                    r_buf[i * 2 + 1] = bytes[0]; // Lower byte
                                }
                            }

                            DispDataIPIS.dp_buf = new byte[1024];

                            for (int idx = 0; idx < (wd * ht / 8); idx++)
                            {
                                DispDataIPIS.dp_buf[idx] = r_buf[idx];
                            }

                            if (true)
                            {
                                byte[] blobData = StructureToByteArray(DispDataIPIS);

                                CmdPacket = new CmdPacket_t();
                                CmdPacket.header = 0x5566AABB;
                                CmdPacket.size = (UInt32)blobData.Length;
                                CmdPacket.cmd = 0x0000000C;
                                CmdPacket.info = 1;
                                CmdPacket.crc = CmdPacket.size ^ CmdPacket.cmd ^ CmdPacket.info;
                                byte[] blobCmd = StructureToByteArray(CmdPacket);
                                SessionVars.nStream.Write(blobCmd, 0, blobCmd.Length);
                                await Task.Delay(TimeSpan.FromSeconds(1));
                                byte[] blobRep = new byte[4];
                                SessionVars.nStream.Read(blobRep, 0, blobRep.Length);

                                if (Encoding.ASCII.GetString(blobRep) == "#OK*")
                                {
                                    int offset = 0;
                                    int bytes;
                                    while (offset < blobData.Length)
                                    {
                                        await Task.Delay(TimeSpan.FromSeconds(0.5));

                                        if (blobData.Length - offset >= 1024)
                                        {
                                            bytes = 1024;
                                        }
                                        else
                                        {
                                            bytes = blobData.Length - offset;
                                        }
                                        SessionVars.nStream.Write(blobData, offset, bytes);
                                        offset += bytes;
                                    }

                                    SessionVars.nStream.Read(blobRep, 0, blobRep.Length);

                                    if (Encoding.ASCII.GetString(blobRep) == "#OK*")
                                    {
                                        //good to go

                                    }
                                    else
                                    if (Encoding.ASCII.GetString(blobRep) == "#ER*")
                                    {
                                        //good to go

                                    }
                                }
                                else
                                {
                                    //some issue
                                    MessageBox.Show(" Communication Error while Flash Image Sent to Display", "Steady Image", MessageBoxButton.OK, MessageBoxImage.Error);
                                    error = true;
                                    Mouse.OverrideCursor = null;
                                }
                            }

                            SessionVars.nStream.Close();
                            SessionVars.tcpClient.Close();
                            Mouse.OverrideCursor = null;
                        }
                    }

                    {

                    }
                }
                catch (Exception exp)
                {
                    Mouse.OverrideCursor = null;
                    SessionVars.nStream.Close();
                    SessionVars.tcpClient.Close();
                    MessageBox.Show(exp.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void read_config(object sender, RoutedEventArgs e)
        {
            //send data
            if (tb_ipAddr.Text == "")
            {
                MessageBox.Show("IP Address is not Entered!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool connected = false;

            this.Dispatcher.Invoke((Action)(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;

                tb_status.Text = "";
            }));

            try
            {
                SessionVars.ipAddr = tb_ipAddr.Text;
                SessionVars.tcpPort = (Int32)ud_tcpPort.Value;
                SessionVars.tcpClient = new TcpClient(SessionVars.ipAddr, SessionVars.tcpPort);
                SessionVars.nStream = SessionVars.tcpClient.GetStream();

                connected = true;

                {
                    CmdPacket = new CmdPacket_t();
                    CmdPacket.header = 0x5566AABB;
                    CmdPacket.size = 0;
                    CmdPacket.cmd = 0x00000016;
                    CmdPacket.info = 0;
                    CmdPacket.crc = CmdPacket.size ^ CmdPacket.cmd ^ CmdPacket.info;
                    byte[] blobCmd = StructureToByteArray(CmdPacket);
                    SessionVars.nStream.Write(blobCmd, 0, blobCmd.Length);

                    byte[] okblobRep = new byte[4];
                    SessionVars.nStream.Read(okblobRep, 0, okblobRep.Length);

                    if (Encoding.ASCII.GetString(okblobRep) == "#OK*")
                    {
                        await Task.Delay(TimeSpan.FromSeconds(0.25));
                        config = new config_t();
                        int sz = Marshal.SizeOf(config);

                        byte[] blobRep = new byte[sz];
                        SessionVars.nStream.Read(blobRep, 0, blobRep.Length);

                        config = (config_t)ByteArrayToStructure(blobRep, config, 0);

                        RunOnUiThread((Action)delegate
                        {
                            tb_status.Text = "Data Received";

                            tb_device_id.Text = Encoding.Default.GetString(config.deviceId).ToString();

                            ud_emac_1.Value = (int)config.mac_address[0];
                            ud_emac_2.Value = (int)config.mac_address[1];
                            ud_emac_3.Value = (int)config.mac_address[2];
                            ud_emac_4.Value = (int)config.mac_address[3];
                            ud_emac_5.Value = (int)config.mac_address[4];
                            ud_emac_6.Value = (int)config.mac_address[5];

                            tb_ip_addr.Text = Encoding.Default.GetString(config.staticIpAddr).ToString();
                            tb_net_mask.Text = Encoding.Default.GetString(config.netMask).ToString();
                            tb_gateway.Text = Encoding.Default.GetString(config.gateway).ToString();
                            tb_domain.Text = Encoding.Default.GetString(config.dns).ToString();

                            cb_ipMode.SelectedIndex = (int)config.ip_mode;

                            ud_tcp_port.Value = (int)config.port;

                        });
                    }
                    SessionVars.nStream.Close();
                    SessionVars.tcpClient.Close();
                }

                this.Dispatcher.Invoke((Action)(() =>
                {
                    Mouse.OverrideCursor = null;
                }));
            }
            catch (Exception exp)
            {
                if (connected)
                {
                    SessionVars.nStream.Close();
                    SessionVars.tcpClient.Close();
                }

                MessageBox.Show(exp.Message, "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.Dispatcher.Invoke((Action)(() =>
            {
                Mouse.OverrideCursor = null;
            }));
        }

        private async void write_config(object sender, RoutedEventArgs e)
        {
            if (tb_ipAddr.Text == "")
            {
                MessageBox.Show("IP Address is not Entered!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool connected = false;

            this.Dispatcher.Invoke((Action)(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;

                tb_status.Text = "";
            }));

            try
            {
                SessionVars.ipAddr = tb_ipAddr.Text;
                SessionVars.tcpPort = (Int32)ud_tcpPort.Value;
                SessionVars.tcpClient = new TcpClient(SessionVars.ipAddr, SessionVars.tcpPort);
                SessionVars.nStream = SessionVars.tcpClient.GetStream();

                connected = true;

                config = new config_t();
                config.header = (UInt32)0xCAFEBABE;

                config.deviceId = Encoding.ASCII.GetBytes(tb_device_id.Text.ToString().PadRight(20, '\0'));

                config.ip_mode = (ip_mode_e)cb_ipMode.SelectedIndex;
                config.staticIpAddr = Encoding.ASCII.GetBytes(tb_ip_addr.Text.ToString().PadRight(20, '\0'));
                config.netMask = Encoding.ASCII.GetBytes(tb_net_mask.Text.ToString().PadRight(20, '\0'));
                config.gateway = Encoding.ASCII.GetBytes(tb_gateway.Text.ToString().PadRight(20, '\0'));
                config.dns = Encoding.ASCII.GetBytes(tb_domain.Text.ToString().PadRight(20, '\0'));

                byte[] macAddressBytes = new byte[8]; // Create a byte array of size 8
                macAddressBytes[0] = (byte)ud_emac_1.Value;
                macAddressBytes[1] = (byte)ud_emac_2.Value;
                macAddressBytes[2] = (byte)ud_emac_3.Value;
                macAddressBytes[3] = (byte)ud_emac_4.Value;
                macAddressBytes[4] = (byte)ud_emac_5.Value;
                macAddressBytes[5] = (byte)ud_emac_6.Value;
                config.mac_address = macAddressBytes;

                config.port = (UInt32)ud_tcp_port.Value;

                byte[] blobData = StructureToByteArray(config);

                {
                    CmdPacket = new CmdPacket_t();
                    CmdPacket.header = 0x5566AABB;
                    CmdPacket.size = (UInt32)blobData.Length;
                    CmdPacket.cmd = 0x00000017;
                    CmdPacket.info = 0;
                    CmdPacket.crc = CmdPacket.size ^ CmdPacket.cmd ^ CmdPacket.info;
                    byte[] blobCmd = StructureToByteArray(CmdPacket);
                    SessionVars.nStream.Write(blobCmd, 0, blobCmd.Length);

                    byte[] blobRep = new byte[4];
                    SessionVars.nStream.Read(blobRep, 0, blobRep.Length);
                    await Task.Delay(TimeSpan.FromSeconds(0.5));

                    if (Encoding.ASCII.GetString(blobRep) == "#OK*")
                    {
                        SessionVars.nStream.Write(blobData, 0, blobData.Length);
                        await Task.Delay(TimeSpan.FromSeconds(0.25));
                        SessionVars.nStream.Read(blobRep, 0, blobRep.Length);

                        if (Encoding.ASCII.GetString(blobRep) == "#OK*")
                        {
                            //good to go
                            MessageBox.Show(" Successfully sent Configuration to Display", "Configuration", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        if (Encoding.ASCII.GetString(blobRep) == "#ER*")
                        {
                            //good to go
                            MessageBox.Show(" Config Sent but Error Code received from card", "Configuration", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        //some issue
                        MessageBox.Show(" Communication Error while sending Configuration to Display", "Configuration", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    SessionVars.nStream.Close();
                    SessionVars.tcpClient.Close();
                }

                this.Dispatcher.Invoke((Action)(() =>
                {
                    Mouse.OverrideCursor = null;
                }));
            }
            catch (Exception exp)
            {
                if (connected)
                {
                    SessionVars.nStream.Close();
                    SessionVars.tcpClient.Close();
                }

                MessageBox.Show(exp.Message, "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Dispatcher.Invoke((Action)(() =>
                {
                    Mouse.OverrideCursor = null;
                }));
            }
        }

        private async void bt_ota(object sender, RoutedEventArgs e)
        {
            if (tb_ipAddr.Text == "")
            {
                MessageBox.Show("IP Address is not Entered!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool connected = false;

            this.Dispatcher.Invoke((Action)(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;

                tb_status.Text = "";
            }));

            try
            {
                SessionVars.ipAddr = tb_ipAddr.Text;
                SessionVars.tcpPort = (Int32)ud_tcpPort.Value;
                SessionVars.tcpClient = new TcpClient(SessionVars.ipAddr, SessionVars.tcpPort);
                SessionVars.nStream = SessionVars.tcpClient.GetStream();

                connected = true;

                {
                    CmdPacket = new CmdPacket_t();
                    CmdPacket.header = 0x5566AABB;
                    CmdPacket.size = 0;
                    CmdPacket.cmd = 0x0000000F;
                    CmdPacket.info = 0;
                    CmdPacket.crc = CmdPacket.size ^ CmdPacket.cmd ^ CmdPacket.info;
                    byte[] blobCmd = StructureToByteArray(CmdPacket);
                    SessionVars.nStream.Write(blobCmd, 0, blobCmd.Length);
                    await Task.Delay(TimeSpan.FromSeconds(0.5));

                    byte[] okblobRep = new byte[4];
                    SessionVars.nStream.Read(okblobRep, 0, okblobRep.Length);

                    await Task.Delay(TimeSpan.FromSeconds(0.25));

                    if (Encoding.ASCII.GetString(okblobRep) == "#OK*")
                    {
                        RunOnUiThread((Action)delegate
                        {
                            MessageBox.Show("Restart Command Successfully Sent ", "Remote Start", MessageBoxButton.OK, MessageBoxImage.Information);
                        });
                    }
                    else
                    {
                        RunOnUiThread((Action)delegate
                        {
                            MessageBox.Show("Restart Command Failed ", "Remote Start", MessageBoxButton.OK, MessageBoxImage.Error);
                        });
                    }
                    SessionVars.nStream.Close();
                    SessionVars.tcpClient.Close();
                }

                this.Dispatcher.Invoke((Action)(() =>
                {
                    Mouse.OverrideCursor = null;
                }));
            }
            catch (Exception exp)
            {
                if (connected)
                {
                    SessionVars.nStream.Close();
                    SessionVars.tcpClient.Close();
                }

                MessageBox.Show(exp.Message, "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Dispatcher.Invoke((Action)(() =>
                {
                    Mouse.OverrideCursor = null;
                }));
            }
        }

        private async void bt_start(object sender, RoutedEventArgs e)
        {
            if (tb_ipAddr.Text == "")
            {
                MessageBox.Show("IP Address is not Entered!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool connected = false;

            this.Dispatcher.Invoke((Action)(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;

                tb_status.Text = "";
            }));

            try
            {
                SessionVars.ipAddr = tb_ipAddr.Text;
                SessionVars.tcpPort = (Int32)ud_tcpPort.Value;
                SessionVars.tcpClient = new TcpClient(SessionVars.ipAddr, SessionVars.tcpPort);
                SessionVars.nStream = SessionVars.tcpClient.GetStream();

                connected = true;

                {
                    CmdPacket = new CmdPacket_t();
                    CmdPacket.header = 0x5566AABB;
                    CmdPacket.size = 0;
                    CmdPacket.cmd = 0x0000001C;
                    CmdPacket.info = 0;
                    CmdPacket.crc = CmdPacket.size ^ CmdPacket.cmd ^ CmdPacket.info;
                    byte[] blobCmd = StructureToByteArray(CmdPacket);
                    SessionVars.nStream.Write(blobCmd, 0, blobCmd.Length);
                    await Task.Delay(TimeSpan.FromSeconds(0.5));

                    byte[] okblobRep = new byte[4];
                    SessionVars.nStream.Read(okblobRep, 0, okblobRep.Length);

                    await Task.Delay(TimeSpan.FromSeconds(0.25));

                    if (Encoding.ASCII.GetString(okblobRep) == "#OK*")
                    {
                        RunOnUiThread((Action)delegate
                        {
                            MessageBox.Show("Restart Command Successfully Sent ", "Remote Start", MessageBoxButton.OK, MessageBoxImage.Information);
                        });
                    }
                    else
                    {
                        RunOnUiThread((Action)delegate
                        {
                            MessageBox.Show("Restart Command Failed ", "Remote Start", MessageBoxButton.OK, MessageBoxImage.Error);
                        });
                    }
                    SessionVars.nStream.Close();
                    SessionVars.tcpClient.Close();
                }

                this.Dispatcher.Invoke((Action)(() =>
                {
                    Mouse.OverrideCursor = null;
                }));
            }
            catch (Exception exp)
            {
                if (connected)
                {
                    SessionVars.nStream.Close();
                    SessionVars.tcpClient.Close();
                }

                MessageBox.Show(exp.Message, "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Dispatcher.Invoke((Action)(() =>
                {
                    Mouse.OverrideCursor = null;
                }));
            }
        }

        private async void bt_stop(object sender, RoutedEventArgs e)
        {
            if (tb_ipAddr.Text == "")
            {
                MessageBox.Show("IP Address is not Entered!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool connected = false;

            this.Dispatcher.Invoke((Action)(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;

                tb_status.Text = "";
            }));

            try
            {
                SessionVars.ipAddr = tb_ipAddr.Text;
                SessionVars.tcpPort = (Int32)ud_tcpPort.Value;
                SessionVars.tcpClient = new TcpClient(SessionVars.ipAddr, SessionVars.tcpPort);
                SessionVars.nStream = SessionVars.tcpClient.GetStream();

                connected = true;

                {
                    CmdPacket = new CmdPacket_t();
                    CmdPacket.header = 0x5566AABB;
                    CmdPacket.size = 0;
                    CmdPacket.cmd = 0x0000002C;
                    CmdPacket.info = 0;
                    CmdPacket.crc = CmdPacket.size ^ CmdPacket.cmd ^ CmdPacket.info;
                    byte[] blobCmd = StructureToByteArray(CmdPacket);
                    SessionVars.nStream.Write(blobCmd, 0, blobCmd.Length);
                    await Task.Delay(TimeSpan.FromSeconds(0.5));

                    byte[] okblobRep = new byte[4];
                    SessionVars.nStream.Read(okblobRep, 0, okblobRep.Length);

                    await Task.Delay(TimeSpan.FromSeconds(0.25));

                    if (Encoding.ASCII.GetString(okblobRep) == "#OK*")
                    {
                        RunOnUiThread((Action)delegate
                        {
                            MessageBox.Show("Restart Command Successfully Sent ", "Remote Start", MessageBoxButton.OK, MessageBoxImage.Information);
                        });
                    }
                    else
                    {
                        RunOnUiThread((Action)delegate
                        {
                            MessageBox.Show("Restart Command Failed ", "Remote Start", MessageBoxButton.OK, MessageBoxImage.Error);
                        });
                    }
                    SessionVars.nStream.Close();
                    SessionVars.tcpClient.Close();
                }

                this.Dispatcher.Invoke((Action)(() =>
                {
                    Mouse.OverrideCursor = null;
                }));
            }
            catch (Exception exp)
            {
                if (connected)
                {
                    SessionVars.nStream.Close();
                    SessionVars.tcpClient.Close();
                }

                MessageBox.Show(exp.Message, "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Dispatcher.Invoke((Action)(() =>
                {
                    Mouse.OverrideCursor = null;
                }));
            }
        }

        private void TogglePasswordVisibility(object sender, RoutedEventArgs e)
        {

        }

        private async void bt_rst(object sender, RoutedEventArgs e)
        {
            if (tb_ipAddr.Text == "")
            {
                MessageBox.Show("IP Address is not Entered!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool connected = false;

            this.Dispatcher.Invoke((Action)(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;

                tb_status.Text = "";
            }));

            try
            {
                SessionVars.ipAddr = tb_ipAddr.Text;
                SessionVars.tcpPort = (Int32)ud_tcpPort.Value;
                SessionVars.tcpClient = new TcpClient(SessionVars.ipAddr, SessionVars.tcpPort);
                SessionVars.nStream = SessionVars.tcpClient.GetStream();

                connected = true;

                {
                    CmdPacket = new CmdPacket_t();
                    CmdPacket.header = 0x5566AABB;
                    CmdPacket.size = 0;
                    CmdPacket.cmd = 0x00000019;
                    CmdPacket.info = 0;
                    CmdPacket.crc = CmdPacket.size ^ CmdPacket.cmd ^ CmdPacket.info;
                    byte[] blobCmd = StructureToByteArray(CmdPacket);
                    SessionVars.nStream.Write(blobCmd, 0, blobCmd.Length);
                    await Task.Delay(TimeSpan.FromSeconds(0.5));

                    byte[] okblobRep = new byte[4];
                    SessionVars.nStream.Read(okblobRep, 0, okblobRep.Length);

                    await Task.Delay(TimeSpan.FromSeconds(0.25));

                    if (Encoding.ASCII.GetString(okblobRep) == "#OK*")
                    {
                        RunOnUiThread((Action)delegate
                        {
                            MessageBox.Show("Restart Command Successfully Sent ", "Remote Start", MessageBoxButton.OK, MessageBoxImage.Information);
                        });
                    }
                    else
                    {
                        RunOnUiThread((Action)delegate
                        {
                            MessageBox.Show("Restart Command Failed ", "Remote Start", MessageBoxButton.OK, MessageBoxImage.Error);
                        });
                    }
                    SessionVars.nStream.Close();
                    SessionVars.tcpClient.Close();
                }

                this.Dispatcher.Invoke((Action)(() =>
                {
                    Mouse.OverrideCursor = null;
                }));
            }
            catch (Exception exp)
            {
                if (connected)
                {
                    SessionVars.nStream.Close();
                    SessionVars.tcpClient.Close();
                }

                MessageBox.Show(exp.Message, "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Dispatcher.Invoke((Action)(() =>
                {
                    Mouse.OverrideCursor = null;
                }));
            }
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            // Implement the logic for Auto mode
            ((ToggleButton)sender).Content = "Auto";
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            // Implement the logic for Manual mode
            ((ToggleButton)sender).Content = "Manual";
        }

        private void ViewAlertButton_Click(object sender, RoutedEventArgs e)
        {
            // Implement the logic to view the alert in a popup window
        }

        private void OverrideAlertButton_Click(object sender, RoutedEventArgs e)
        {
            // Implement the logic to override the alert
        }

        private void PauseResumeAlertButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int duration = PauseResumeDuration.Value ?? 600; // Default to 600 if null

            if (button.Content.ToString() == "Pause Alert")
            {
                // Implement the logic to pause the alert with the specified duration
                button.Content = "Resume Alert";
            }
            else
            {
                // Implement the logic to resume the alert with the specified duration
                button.Content = "Pause Alert";
            }
        }

        private void AutomationToggleButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void AutomationToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void cb_ivd_ovd_selection_changed(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                int selectedIndex = comboBox.SelectedIndex;

                Console.WriteLine($"Selected Index: {selectedIndex}");
            }
        }


        private void MoveLeftButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = TimelineListBox.SelectedIndex;
            if (selectedIndex > 0)
            {
                var item = _mainViewModel.TimelineItems[selectedIndex];
                _mainViewModel.TimelineItems.Move(selectedIndex, selectedIndex - 1);
            }
        }

        private void MoveRightButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = TimelineListBox.SelectedIndex;
            if (selectedIndex < _mainViewModel.TimelineItems.Count - 1)
            {
                var item = _mainViewModel.TimelineItems[selectedIndex];
                _mainViewModel.TimelineItems.Move(selectedIndex, selectedIndex + 1);
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = TimelineListBox.SelectedIndex;
            if (selectedIndex >= 0)
            {
                _mainViewModel.TimelineItems.RemoveAt(selectedIndex);
            }
        }

        private void DuplicateButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = TimelineListBox.SelectedItem as TimelineItem;
            if (selectedItem != null)
            {
                var duplicateItem = new TimelineItem
                {
                    Name = selectedItem.Name + " (Copy)",
                    FilePath = selectedItem.FilePath,
                    ItemType = selectedItem.ItemType,
                    Duration = selectedItem.Duration,
                    Offset = CalculateNextItemOffset(), // Set the offset for the duplicated item
                    Resolution = selectedItem.Resolution,
                    ThumbnailPath = selectedItem.ThumbnailPath,
                    Position = selectedItem.Position + 1 // Increment the position for the duplicate
                };

                _mainViewModel.TimelineItems.Insert(TimelineListBox.SelectedIndex + 1, duplicateItem);
                _mainViewModel.SelectedTimeline.Items.Insert(TimelineListBox.SelectedIndex + 1, duplicateItem);
            }
        }


        private void PlayFromHereButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = TimelineListBox.SelectedItem as TimelineItem;
            if (selectedItem != null)
            {
                // Implement logic to start playback from the selected item
            }
        }

        private void ClearTimelineButton_Click(object sender, RoutedEventArgs e)
        {
            _mainViewModel.TimelineItems.Clear();
        }

        private void SaveTimelineButton_Click(object sender, RoutedEventArgs e)
        {
            if (_mainViewModel.SelectedTimeline != null)
            {
                _timelineManager.SaveTimeline(_mainViewModel.SelectedTimeline);
                MessageBox.Show("Timeline saved successfully!", "Save Timeline", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("No timeline selected.", "Save Timeline", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SaveAsTimelineButton_Click(object sender, RoutedEventArgs e)
        {
            if (_mainViewModel.SelectedTimeline != null)
            {
                InputDialog inputDialog = new InputDialog("Enter new timeline name:");
                if (inputDialog.ShowDialog() == true)
                {
                    string newTimelineName = inputDialog.ResponseText;
                    var newTimeline = _mainViewModel.SelectedTimeline.Clone(); // Assuming Clone() creates a deep copy
                    newTimeline.Name = newTimelineName;

                    _timelineManager.SaveTimelineAs(newTimeline);
                    _mainViewModel.Timelines.Add(newTimeline);
                    MessageBox.Show("Timeline saved as successfully!", "Save As Timeline", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("No timeline selected.", "Save As Timeline", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        //timelines
        private void CreateTimelineMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var inputDialog = new InputDialog("Enter Timeline Name:");
            if (inputDialog.ShowDialog() == true)
            {
                var timelineName = inputDialog.ResponseText;
                if (!string.IsNullOrWhiteSpace(timelineName))
                {
                    var newTimeline = new Timeline { Name = timelineName };
                    _mainViewModel.Timelines.Add(newTimeline);
                    _timelineManager.SaveTimelines(_mainViewModel.Timelines.ToList());
                }
            }
        }


        private void OpenTimelineMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_mainViewModel.SelectedTimeline != null)
            {
                // Logic to open the selected timeline
                MessageBox.Show($"Opening Timeline: {_mainViewModel.SelectedTimeline.Name}");
            }
            else
            {
                MessageBox.Show("Please select a timeline to open.", "No Timeline Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteTimelineMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_mainViewModel.SelectedTimeline != null)
            {
                var result = MessageBox.Show($"Are you sure you want to delete the timeline '{_mainViewModel.SelectedTimeline.Name}'?",
                                             "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    _mainViewModel.Timelines.Remove(_mainViewModel.SelectedTimeline);
                    _timelineManager.SaveTimelines(_mainViewModel.Timelines.ToList());
                }
            }
            else
            {
                MessageBox.Show("Please select a timeline to delete.", "No Timeline Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void TimelineListBox_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void TimelineListBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void TimelineListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = TimelineListBox.SelectedItem as TimelineItem;
            if (selectedItem != null)
            {
                // Set the SelectedTimelineItem in the ViewModel
                _mainViewModel.SelectedTimelineItem = selectedItem;

                // Optionally, trigger loading of the item in the preview window
                LoadItemInPreview(selectedItem);
            }
        }

        private void LoadItemInPreview(TimelineItem item)
        {
            //switch (item.ItemType)
            //{
            //    case TimelineItemType.Image:
            //    case TimelineItemType.TextSlide:
            //        DisplayImage(item.FilePath);
            //        break;
            //    case TimelineItemType.Video:
            //        DisplayVideoInMediaElement(item.FilePath);
            //        break;
            //    case TimelineItemType.Audio:
            //        DisplayAudioInMediaElement(item.FilePath);
            //        break;
            //        // Handle other item types like transitions, effects here if necessary
            //}
        }

        private void StationMaster_Click(object sender, RoutedEventArgs e)
        {
            StationDbWindow stationDbWindow = new StationDbWindow();
            stationDbWindow.Owner = this;
            stationDbWindow.ShowDialog();
        }

        private void TrainsMaster_Click(object sender, RoutedEventArgs e)
        {
            TrainMasterDbWindow trainMasterDbWindow = new TrainMasterDbWindow();
            trainMasterDbWindow.Owner = this;
            trainMasterDbWindow.ShowDialog();
        }

        //private void AddNewActiveTrain_Click(object sender, RoutedEventArgs e)
        //{
        //    var newTrain = new ActiveTrain();

        //    var window = new ActiveTrainWindow(newTrain);
        //    if (window.ShowDialog() == true)
        //    {
        //        //ActiveTrains.Add(newTrain);
        //    }
        //}

        //private void EditActiveTrain_Click(object sender, RoutedEventArgs e)
        //{
        //    if (ActiveTrainDataGrid.SelectedItem is ActiveTrain selectedTrain)
        //    {
        //        var window = new ActiveTrainWindow(selectedTrain);
        //        if (window.ShowDialog() == true)
        //        {

        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show("Please select a train to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
        //    }
        //}

        private void AddNewActiveTrain_Click(object sender, RoutedEventArgs e)
        {
            var newTrain = new ActiveTrain();

            var window = new ActiveTrainWindow(newTrain);
            if (window.ShowDialog() == true)
            {
                _mainViewModel.ActiveTrains.Add(newTrain);
            }
        }

        private void EditActiveTrain_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveTrainDataGrid.SelectedItem is ActiveTrain selectedTrain)
            {
                var trainCopy = selectedTrain.DeepClone();

                var window = new ActiveTrainWindow(trainCopy);
                if (window.ShowDialog() == true)
                {
                    selectedTrain.UpdateFrom(trainCopy);
                }
            }
            else
            {
                MessageBox.Show("Please select a train to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddFromDB_Click(object sender, RoutedEventArgs e)
        {
            var trainMasterDbWindow = new TrainMasterDbWindow();

            if (trainMasterDbWindow.ShowDialog() == true)
            {
                var viewModel = trainMasterDbWindow.DataContext as TrainMasterViewModel;

                if (viewModel != null)
                {
                    var selectedTrain = viewModel.SelectedTrain;

                    if (selectedTrain != null)
                    {
                        var newActiveTrain = new ActiveTrain(selectedTrain);
                        _mainViewModel.ActiveTrains.Add(newActiveTrain);
                        _activeTrainsManager.SaveActiveTrains(_mainViewModel.ActiveTrains.ToList());
                    }
                    else
                    {
                        MessageBox.Show("No train selected.", "Selection Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Failed to load train data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteActiveTrain_Click(object sender, RoutedEventArgs e)
        {
            var selectedTrains = _mainViewModel.ActiveTrains.Where(train => train.IsSelected).ToList();

            if (selectedTrains.Any())
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete {selectedTrains.Count} selected train(s)?",
                    "Confirm Deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    foreach (var train in selectedTrains)
                    {
                        _mainViewModel.ActiveTrains.Remove(train);
                    }
                    _activeTrainsManager.SaveActiveTrains(_mainViewModel.ActiveTrains.ToList());
                }
            }
            else
            {
                MessageBox.Show("Please select one or more trains to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var train in _mainViewModel.ActiveTrains)
            {
                train.IsSelected = true;
            }
        }

        private void SelectNTES_Click(object sender, RoutedEventArgs e)
        {
            foreach (var train in _mainViewModel.ActiveTrains)
            {
                train.IsSelected = train.Ref == TrainSource.NTES;
            }
        }

        private void SelectDB_Click(object sender, RoutedEventArgs e)
        {
            foreach (var train in _mainViewModel.ActiveTrains)
            {
                train.IsSelected = train.Ref == TrainSource.TRAIN_DB;
            }
        }

        private void SelectManual_Click(object sender, RoutedEventArgs e)
        {
            foreach (var train in _mainViewModel.ActiveTrains)
            {
                train.IsSelected = train.Ref == TrainSource.USER;
            }
        }

        private void UndoSelect_Click(object sender, RoutedEventArgs e)
        {
            foreach (var train in _mainViewModel.ActiveTrains)
            {
                train.IsSelected = false;
            }
        }

        private void SaveToDB_Click(object sender, RoutedEventArgs e)
        {
            var jsonHelperAdapter = new SettingsJsonHelperAdapter();
            var trainMasterManager = new TrainMasterManager(jsonHelperAdapter);

            // Iterate through the selected ActiveTrains and save them to TrainMasterDB
            var selectedTrains = _mainViewModel.ActiveTrains.Where(train => train.IsSelected).ToList();

            if (!selectedTrains.Any())
            {
                MessageBox.Show("No trains selected for saving.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Create a list to hold the TrainMaster objects
            var trainMasters = new List<TrainMaster>();

            foreach (var activeTrain in selectedTrains)
            {
                var trainMaster = new TrainMaster(activeTrain);
                trainMasters.Add(trainMaster);
                trainMasterManager.AddTrainMaster(trainMaster);
            }

            trainMasterManager.SaveTrainMasters(trainMasters);
            MessageBox.Show("Selected trains have been saved to the TrainMasterDB.", "Save Successful", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void TimelineListBox_Drop(object sender, DragEventArgs e)
        {
            //if (e.Data.GetDataPresent(DataFormats.FileDrop))
            //{
            //    var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            //    foreach (var file in files)
            //    {
            //        // Add logic to determine file type and add as TimelineItem
            //        // Example for Image
            //        if (file.EndsWith(".jpg") || file.EndsWith(".png"))
            //        {
            //            var imageItem = new TimelineItem
            //            {
            //                Name = System.IO.Path.GetFileName(file),
            //                FilePath = file,
            //                MediaType = MediaType.Image,
            //                StartTime = TimeSpan.Zero // Update start time based on where it should be placed in the timeline
            //            };
            //            _mainViewModel.TimelineItems.Add(imageItem);
            //        }
            //        // Add logic for other types (Video, TextSlide)
            //    }
            //}
        }

        // Playlist DataGrid Context Menu Actions
        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement functionality to move the selected playlist item up
            MessageBox.Show("Move Up Clicked");
        }

        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement functionality to move the selected playlist item down
            MessageBox.Show("Move Down Clicked");
        }

        private void RemoveFromPlaylist_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement functionality to remove the selected item from the playlist
            MessageBox.Show("Remove From Playlist Clicked");
        }

        // Audio Files DataGrid Context Menu Actions
        private void OpenInPlayer_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement functionality to open the selected audio file in the player
            MessageBox.Show("Open in Player Clicked");
        }

        private void AddToPlaylistFromBank_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement functionality to add the selected audio file from the bank to the playlist
            MessageBox.Show("Add to Playlist Clicked");
        }

        private void ImportAudio_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement functionality to import an audio file to the audio bank
            MessageBox.Show("Import Audio Clicked");
        }

        private void DeleteAudioFromBank_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement functionality to delete the selected audio file from the audio bank
            MessageBox.Show("Delete Audio Clicked");
        }

        // Voice Recording DataGrid Context Menu Actions
        private void PlayRecording_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement functionality to play the selected recording
            MessageBox.Show("Play Recording Clicked");
        }

        private void RenameRecording_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement functionality to rename the selected recording
            MessageBox.Show("Rename Recording Clicked");
        }

        private void AddToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement functionality to add the selected recording to the playlist
            MessageBox.Show("Add to Playlist Clicked");
        }

        private void DeleteRecording_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement functionality to delete the selected recording
            MessageBox.Show("Delete Recording Clicked");
        }

        // Audio Playback and Recording Controls
        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement functionality to start recording
            MessageBox.Show("Record Button Clicked");
        }

        //private void PlayButton_Click(object sender, RoutedEventArgs e)
        //{
        //    // TODO: Implement functionality to play the audio
        //    MessageBox.Show("Play Button Clicked");
        //}

        //private void PauseButton_Click(object sender, RoutedEventArgs e)
        //{
        //    // TODO: Implement functionality to pause the audio
        //    MessageBox.Show("Pause Button Clicked");
        //}

        //private void StopButton_Click(object sender, RoutedEventArgs e)
        //{
        //    // TODO: Implement functionality to stop the audio or recording
        //    MessageBox.Show("Stop Button Clicked");
        //}

        //private void SaveButton_Click(object sender, RoutedEventArgs e)
        //{
        //    // TODO: Implement functionality to save the recording
        //    MessageBox.Show("Save Button Clicked");
        //}

        //private void AudioSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    // TODO: Implement functionality to handle audio slider value change
        //    MessageBox.Show("Audio Slider Value Changed");
        //}


        //Announcement Backend
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            // Stub: Announcement Started
            MessageBox.Show("Announcement Started");
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            // Stub: Announcement Paused
            MessageBox.Show("Announcement Paused");
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            // Stub: Announcement Stopped
            MessageBox.Show("Announcement Stopped");
        }

        private void MuteButton_Checked(object sender, RoutedEventArgs e)
        {
            // Stub: Announcement Muted
            MessageBox.Show("Announcement Muted");
        }

        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DownloadLogsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ClearLogsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MuteButton_Unchecked(object sender, RoutedEventArgs e)
        {
            // Stub: Announcement Unmuted
            MessageBox.Show("Announcement Unmuted");
        }
    }
}
