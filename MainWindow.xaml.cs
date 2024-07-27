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
//using System.Windows.Shapes;
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
//using System.Windows.Forms;

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

    public class MediaFile
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

    //public class User
    //{
    //    public string Name { get; set; }
    //    public string Email { get; set; }
    //}

    //public class Train
    //{
    //    public string TrainNumber { get; set; }
    //    public string TrainName { get; set; }
    //    public string ArrivalTime { get; set; }
    //    public string DepartureTime { get; set; }
    //}

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

        private RmsSettingsManager _rmsSettingsManager;

        private readonly TrainStatusDisplayManager _trainStatusDisplayManager;

        private MainViewModel _mainViewModel;

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
        public ObservableCollection<LogItem> Logs { get; set; }
        public ObservableCollection<LogItem> FilteredLogs { get; set; }

        public ObservableCollection<Alert> Alerts { get; set; }
        public ObservableCollection<Alert> ActiveAlerts { get; set; }

        public ObservableCollection<User> Users { get; set; }
        public User CurrentUser { get; set; }

        //public ObservableCollection<Display> Displays { get; set; }
        //public ObservableCollection<DisplaySummary> DisplaySummaries { get; set; }
        //public Display SelectedDisplay { get; set; }

        public ObservableCollection<MediaFile> MediaFiles { get; set; }
        public ObservableCollection<Clip> TimelineClips { get; set; }
        public int SelectedMediaIndex { get; set; }
        public string ActiveTab { get; set; }

        private MediaRecorder mediaRecorder;
        private AudioPlayer audioPlayer;
        private ObservableCollection<string> audioFiles;
        private ObservableCollection<string> playlist;
        private DispatcherTimer recordingTimer;

        private ObservableCollection<NtesTrain951> trainList;
        private ObservableCollection<CgdbManager> cgdbManagerList;
        private DispatcherTimer refreshTimer;
        private DispatcherTimer dateTimeTimer;

        public ObservableCollection<TrainViewModel> Trains { get; set; }
        private TrainListViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new ColorViewModel();

            var jsonHelperAdapter = new SettingsJsonHelperAdapter();
            _userManager = new UserManager(jsonHelperAdapter);
            _userCategoryManager = new UserCategoryManager(jsonHelperAdapter);
            _stationInfoManager = new StationInfoManager(jsonHelperAdapter);
            _platformDeviceManager = new PlatformDeviceManager(jsonHelperAdapter);
            _displayStyleManager = new DisplayStyleManager(jsonHelperAdapter);
            _audioSettingsManager = new AudioSettingsManager(jsonHelperAdapter);
            _rmsSettingsManager = new RmsSettingsManager(jsonHelperAdapter);
            _trainStatusDisplayManager = new TrainStatusDisplayManager(jsonHelperAdapter);

            //Tickets = new ObservableCollection<Ticket>();
            //TicketsDataGrid.ItemsSource = Tickets;

            //LocalBackups = new ObservableCollection<BackupItem>();
            //CloudBackups = new ObservableCollection<BackupItem>();

            //// Bind data grids to collections
            //LocalBackupHistoryDataGrid.ItemsSource = LocalBackups;
            //CloudBackupHistoryDataGrid.ItemsSource = CloudBackups;

            //Logs = new ObservableCollection<LogItem>
            //{
            //    new LogItem { Timestamp = "2024-06-01 10:00:00", Message = "IPIS system started", Severity = "info" },
            //    new LogItem { Timestamp = "2024-06-01 10:05:00", Message = "Train 1234 arrived at platform 1", Severity = "info" },
            //    new LogItem { Timestamp = "2024-06-01 10:10:00", Message = "Announcement made for train 1234", Severity = "info" },
            //    new LogItem { Timestamp = "2024-06-01 10:15:00", Message = "Minor delay in announcement", Severity = "warning" },
            //    new LogItem { Timestamp = "2024-06-01 11:00:00", Message = "IPIS system error", Severity = "error" },
            //    new LogItem { Timestamp = "2024-06-02 09:00:00", Message = "IPIS system maintenance", Severity = "info" },
            //    new LogItem { Timestamp = "2024-06-02 10:15:00", Message = "Train 5678 departed from platform 2", Severity = "info" },
            //    new LogItem { Timestamp = "2024-06-02 11:00:00", Message = "Platform 3 speaker malfunction", Severity = "error" },
            //    new LogItem { Timestamp = "2024-06-03 08:00:00", Message = "Morning announcements started", Severity = "info" },
            //    new LogItem { Timestamp = "2024-06-03 08:30:00", Message = "Train 7890 delayed by 5 minutes", Severity = "warning" }
            //};
            //FilteredLogs = new ObservableCollection<LogItem>(Logs);

            //LogsDataGrid.ItemsSource = FilteredLogs;

            //Alerts = new ObservableCollection<Alert>
            //{
            //    new Alert { Message = "Flood warning in the area", StartTime = "2024-06-18T08:00:00Z", EndTime = "2024-06-18T12:00:00Z", AudioAvailable = true, Urgency = "Immediate", Severity = "Severe", Certainty = "Observed", IsActive = true },
            //    new Alert { Message = "Cyclone alert", StartTime = "2024-06-18T09:00:00Z", EndTime = "2024-06-18T15:00:00Z", AudioAvailable = false, Urgency = "Expected", Severity = "Extreme", Certainty = "Likely", IsActive = true },
            //    new Alert { Message = "Heat wave alert", StartTime = "2024-06-17T10:00:00Z", EndTime = "2024-06-17T18:00:00Z", AudioAvailable = false, Urgency = "Expected", Severity = "Moderate", Certainty = "Possible", IsActive = false }
            //};

            //ActiveAlerts = new ObservableCollection<Alert>(Alerts.Where(alert => alert.IsActive));

            //AllAlertsDataGrid.ItemsSource = Alerts;
            //ActiveAlertsDataGrid.ItemsSource = ActiveAlerts;

            //Users = new ObservableCollection<User>();
            //UsersDataGrid.ItemsSource = Users;

            //Displays = new ObservableCollection<Display>();
            //DisplaySummaries = new ObservableCollection<DisplaySummary>();
            //DisplaySummaryDataGrid.ItemsSource = DisplaySummaries;

            //MediaFiles = new ObservableCollection<MediaFile>();
            //TimelineClips = new ObservableCollection<Clip>();
            //ActiveTab = "media";

            //MediaFilesListBox.ItemsSource = MediaFiles;
            //TimelineListBox.ItemsSource = TimelineClips;

            //InitializePAComponents();

            //// Bind audio interfaces, microphones, and speakers
            //AudioInterfaceComboBox.ItemsSource = new string[] { "Interface 1", "Interface 2", "Interface 3" };
            //MicrophoneComboBox.ItemsSource = new string[] { "Mic 1", "Mic 2", "Mic 3" };
            //PaSpeakerComboBox.ItemsSource = new string[] { "Speaker 1", "Speaker 2", "Speaker 3" };
            //LocalSpeakerComboBox.ItemsSource = new string[] { "Speaker 1", "Speaker 2", "Speaker 3" };

            //// Bind audio files and playlist
            //audioFiles = new ObservableCollection<string> { "file1.mp3", "file2.mp3", "file3.mp3" };
            //playlist = new ObservableCollection<string>();
            //AudioFilesListBox.ItemsSource = audioFiles;
            //PlaylistListBox.ItemsSource = playlist;

            //InitializeDashboardComponents();
            InitializeDateTimeUpdate();

            //// Bind user information and status
            //UsernameStatus.Text = GetCurrentUser().Name;
            //UsertypeStatus.Text = "Admin"; // Mock user type
            //IpStatus.Text = "No conflicts";
            //ConnectedDevicesStatus.Text = "0";
            //NtesStatus.Text = "CONNECTED";


            //// Bind train list and CGDB manager list
            //TrainListBox.ItemsSource = trainList;
            ////CgdbManagerListBox.ItemsSource = cgdbManagerList;

            //Trains = new ObservableCollection<TrainViewModel>();
            //_viewModel = (TrainListViewModel)DataContext;
            

            _mainViewModel = new MainViewModel();
            _mainViewModel.LoadUserCategories(_userCategoryManager.LoadUserCategories());
            _mainViewModel.LoadUsers(_userManager.LoadUsers());
            _mainViewModel.LoadAudioSettings(_audioSettingsManager.LoadAudioSettings());
            _mainViewModel.LoadRmsSettings(_rmsSettingsManager.LoadRmsSettings());
            DataContext = _mainViewModel;
            Loaded += MainWindow_Loaded;
            //Loaded += onWindowLoaded;

            //// Start refreshing data
            //refreshTimer.Start();

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
            CheckAndPromptForAdminUser();

            //var loginWindow = new LoginWindow(_userManager);
            //bool? loginResult = loginWindow.ShowDialog();

            //if (loginResult == true)
            //{
            //    // Proceed with further initialization only after successful login
            //    CheckAndPromptForStationInfo();
            //    PopulateStatusFields();

            //    // await _viewModel.FetchAndDisplayTrains();
            //    // Your further initialization code here
            //}
            //else
            //{
            //    // Handle login cancellation or failure
            //    this.Close(); // Close the MainWindow if login is not successful
            //}

            //await _viewModel.FetchAndDisplayTrains();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Logout();
        }

        private void Logout()
        {
            tc_main.SelectedIndex = 0;
            var loginWindow = new LoginWindow(_userManager);
            bool? loginResult = loginWindow.ShowDialog();

            if (loginResult == true)
            {
                // Proceed with further initialization only after successful login
                CheckAndPromptForStationInfo();
                PopulateStatusFields();

                // await _viewModel.FetchAndDisplayTrains();
                // Your further initialization code here
            }
            else
            {
                // Handle login cancellation or failure
                this.Close(); // Close the MainWindow if login is not successful
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

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
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
        private void SaveSystemConfigButton_Click(object sender, RoutedEventArgs e)
        {
            _rmsSettingsManager.SaveRmsSettings(_mainViewModel.RmsSettings);
            MessageBox.Show("RMS settings saved successfully.");
        }

        private void TestServer1ConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            bool isConnected = _rmsSettingsManager.TestConnection(
                _mainViewModel.RmsSettings.Server1Ip,
                _mainViewModel.RmsSettings.Server1ApiEndpoint,
                _mainViewModel.RmsSettings.Server1ApiKey
            );
            MessageBox.Show(isConnected ? "Server 1 connected successfully." : "Failed to connect to Server 1.");
        }

        private void TestServer2ConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            bool isConnected = _rmsSettingsManager.TestConnection(
                _mainViewModel.RmsSettings.Server2Ip,
                _mainViewModel.RmsSettings.Server2ApiEndpoint,
                _mainViewModel.RmsSettings.Server2ApiKey
            );
            MessageBox.Show(isConnected ? "Server 2 connected successfully." : "Failed to connect to Server 2.");
        }

        private void SaveServer1SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            string server1Ip = Server1IpTextBox.Text;
            string server1ApiEndpoint = Server1ApiEndpointTextBox.Text;
            string server1ApiKey = Server1ApiKeyPasswordBox.Password;

            if (string.IsNullOrEmpty(server1Ip) || string.IsNullOrEmpty(server1ApiEndpoint) || string.IsNullOrEmpty(server1ApiKey))
            {
                MessageBox.Show("Please fill in all the fields for Server 1.", "Incomplete Data", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Save these values as needed
            MessageBox.Show("Server 1 settings saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SaveServer2SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            string server2Ip = Server2IpTextBox.Text;
            string server2ApiEndpoint = Server2ApiEndpointTextBox.Text;
            string server2ApiKey = Server2ApiKeyPasswordBox.Password;

            if (string.IsNullOrEmpty(server2Ip) || string.IsNullOrEmpty(server2ApiEndpoint) || string.IsNullOrEmpty(server2ApiKey))
            {
                MessageBox.Show("Please fill in all the fields for Server 2.", "Incomplete Data", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Save these values as needed
            MessageBox.Show("Server 2 settings saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
                StationInfoTextBlock.Text = $"{stationInfo.StationCode} - {stationInfo.StationNameEnglish}";

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
            trainList = new ObservableCollection<NtesTrain951>();
            cgdbManagerList = new ObservableCollection<CgdbManager>();
            refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(1) };
            refreshTimer.Tick += RefreshTimer_Tick;
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

        private User GetCurrentUser()
        {
            // Mock implementation for user information
            return new User
            {
                Name = "John Doe",
                Email = "john.doe@example.com"
            };
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            LoadTrainData();
            LoadCgdbManagerData();
        }

        private async void LoadTrainData()
        {
            try
            {
                // Mock implementation for loading train data
                trainList.Clear();
                //trainList.Add(new Train { TrainNumber = "12345", TrainName = "Express Train", ArrivalTime = "10:30", DepartureTime = "10:40" });
                //trainList.Add(new Train { TrainNumber = "67890", TrainName = "Local Train", ArrivalTime = "11:00", DepartureTime = "11:10" });

                // Update UI with new data
                TrainListBox.ItemsSource = trainList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading train data: {ex.Message}");
            }
        }

        private async void LoadCgdbManagerData()
        {
            //try
            //{
            //    // Mock implementation for loading CGDB manager data
            //    cgdbManagerList.Clear();
            //    cgdbManagerList.Add(new CgdbManager { DisplayId = "CGDB1", Status = "Online" });
            //    cgdbManagerList.Add(new CgdbManager { DisplayId = "CGDB2", Status = "Offline" });

            //    // Update UI with new data
            //    CgdbManagerListBox.ItemsSource = cgdbManagerList;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Error loading CGDB manager data: {ex.Message}");
            //}
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
            mediaRecorder.StartRecording();
            StartRecordingButton.IsEnabled = false;
            PauseRecordingButton.IsEnabled = true;
            StopRecordingButton.IsEnabled = true;
            ResumeRecordingButton.IsEnabled = false;
            recordingTimer.Start();
        }

        private void PauseRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            mediaRecorder.PauseRecording();
            PauseRecordingButton.IsEnabled = false;
            ResumeRecordingButton.IsEnabled = true;
        }

        private void ResumeRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            mediaRecorder.ResumeRecording();
            ResumeRecordingButton.IsEnabled = false;
            PauseRecordingButton.IsEnabled = true;
        }

        private void StopRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            mediaRecorder.StopRecording();
            StartRecordingButton.IsEnabled = true;
            PauseRecordingButton.IsEnabled = false;
            StopRecordingButton.IsEnabled = false;
            ResumeRecordingButton.IsEnabled = false;
            recordingTimer.Stop();
        }

        private void RecordingTimer_Tick(object sender, EventArgs e)
        {
            ElapsedTimeTextBlock.Text = mediaRecorder.GetElapsedTime();
        }

        private void PlayAudioButton_Click(object sender, RoutedEventArgs e)
        {
            audioPlayer.Play();
            PlayAudioButton.IsEnabled = false;
            PauseAudioButton.IsEnabled = true;
            StopAudioButton.IsEnabled = true;
            SaveAudioButton.IsEnabled = true;
        }

        private void PauseAudioButton_Click(object sender, RoutedEventArgs e)
        {
            audioPlayer.Pause();
            PlayAudioButton.IsEnabled = true;
            PauseAudioButton.IsEnabled = false;
        }

        private void StopAudioButton_Click(object sender, RoutedEventArgs e)
        {
            audioPlayer.Stop();
            PlayAudioButton.IsEnabled = true;
            PauseAudioButton.IsEnabled = false;
            StopAudioButton.IsEnabled = false;
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
        private void AddClipToTimeline(Clip clip)
        {
            TimelineClips.Add(clip);
        }

        private void RemoveClipFromTimeline(int index)
        {
            if (index >= 0 && index < TimelineClips.Count)
            {
                TimelineClips.RemoveAt(index);
            }
        }

        private void SelectMedia(int index)
        {
            if (index >= 0 && index < MediaFiles.Count)
            {
                SelectedMediaIndex = index;
                MediaFilesListBox.SelectedIndex = index;
                PreviewMedia();
            }
        }

        private void PreviewMedia()
        {
            if (SelectedMediaIndex >= 0 && SelectedMediaIndex < MediaFiles.Count)
            {
                var selectedMedia = MediaFiles[SelectedMediaIndex];
                // Implement media preview logic
            }
        }

        private void HandleTabChange(object sender, SelectionChangedEventArgs e)
        {
            if (MediaTabControl.SelectedItem is TabItem selectedTab)
            {
                ActiveTab = selectedTab.Header.ToString().ToLower();
                RenderActiveComponent();
            }
        }

        private void RenderActiveComponent()
        {
            // Implement logic to render the active component based on ActiveTab
        }

        private void ImportMediaButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Filter = "Media Files|*.mp4;*.mp3;*.jpg;*.png;*.avi;*.wav"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var fileName in openFileDialog.FileNames)
                {
                    var mediaFile = new MediaFile
                    {
                        Id = fileName,
                        Name = System.IO.Path.GetFileName(fileName),
                        Type = GetMediaType(fileName),
                        FilePath = fileName
                    };
                    MediaFiles.Add(mediaFile);
                }
            }
        }

        private string GetMediaType(string fileName)
        {
            var extension = System.IO.Path.GetExtension(fileName).ToLower();
            return extension switch
            {
                ".mp4" => "video",
                ".avi" => "video",
                ".mp3" => "audio",
                ".wav" => "audio",
                ".jpg" => "image",
                ".png" => "image",
                _ => "unknown",
            };
        }

        private void MediaFilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectMedia(MediaFilesListBox.SelectedIndex);
        }

        //private void SaveButton_Click(object sender, RoutedEventArgs e)
        //{
        //    MessageBox.Show("Media and Timeline data saved.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        //    // Implement logic to save media and timeline data
        //}

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

        private void UpdateDisplayFormGroup()
        {
            //DisplayFormGroup.Children.Clear();
            //foreach (var display in Displays)
            //{
            //    var formGroup = new StackPanel
            //    {
            //        Margin = new Thickness(5),
            //        Background = display == SelectedDisplay ? new SolidColorBrush(Colors.LightGray) : new SolidColorBrush(Colors.Transparent),
            //        //BorderBrush = new SolidColorBrush(Colors.Gray),
            //        //BorderThickness = new Thickness(1),
            //        //Padding = new Thickness(10)
            //    };

            //    formGroup.Children.Add(new TextBlock { Text = "Type:", Height = 30 });
            //    var typeComboBox = new ComboBox { Height = 30, ItemsSource = new[] { "SLDB", "MLDB", "IVD", "OVD", "PFDB", "AAGDB", "CGDB", "DM" }, SelectedItem = display.Type };
            //    typeComboBox.SelectionChanged += (s, e) => { display.Type = (string)typeComboBox.SelectedItem; display.IpAddr = CalculateIpAddr(display.Type, Displays.IndexOf(display)); };
            //    formGroup.Children.Add(typeComboBox);

            //    formGroup.Children.Add(new TextBlock { Text = "Lines:", Height = 30 });
            //    var linesTextBox = new TextBox { Height = 30, Text = display.Lines.ToString() };
            //    linesTextBox.TextChanged += (s, e) => display.Lines = int.Parse(linesTextBox.Text);
            //    formGroup.Children.Add(linesTextBox);

            //    formGroup.Children.Add(new TextBlock { Text = "IP Address:", Height = 30 });
            //    var ipAddrTextBox = new TextBox { Height = 30, Text = display.IpAddr };
            //    ipAddrTextBox.TextChanged += (s, e) => display.IpAddr = ipAddrTextBox.Text;
            //    formGroup.Children.Add(ipAddrTextBox);

            //    var enabledCheckBox = new CheckBox { Content = "Enabled", IsChecked = display.Enabled };
            //    enabledCheckBox.Checked += (s, e) => display.Enabled = enabledCheckBox.IsChecked ?? false;
            //    enabledCheckBox.Unchecked += (s, e) => display.Enabled = enabledCheckBox.IsChecked ?? false;
            //    formGroup.Children.Add(enabledCheckBox);

            //    formGroup.MouseLeftButtonUp += (s, e) =>
            //    {
            //        SelectedDisplay = display;
            //        UpdateDisplayFormGroup();
            //    };

            //    DisplayFormGroup.Children.Add(formGroup);
            //}
            //UpdateDisplaySummary();
        }

        private void UpdateDisplaySummary()
        {
            //DisplaySummaries.Clear();
            //var groupedDisplays = Displays.GroupBy(d => d.Type);
            //foreach (var group in groupedDisplays)
            //{
            //    DisplaySummaries.Add(new DisplaySummary
            //    {
            //        Type = group.Key,
            //        Quantity = group.Count(),
            //        Lines = group.Average(d => d.Lines).ToString("0"),
            //        Enabled = group.Count(d => d.Enabled)
            //    });
            //}
        }

        //private void ShowSnackbar(string message)
        //{
        //    Snackbar.Visibility = Visibility.Visible;
        //    SnackbarMessage.Text = message;
        //    var timer = new System.Timers.Timer(3000);
        //    timer.Elapsed += (s, e) =>
        //    {
        //        Snackbar.Dispatcher.Invoke(() => Snackbar.Visibility = Visibility.Collapsed);
        //        timer.Stop();
        //    };
        //    timer.Start();
        //}

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

        // code for CAP section
        private void OverrideAlertButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Override Alert clicked", "CAP Status", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void PauseAlertButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Pause Alert clicked", "CAP Status", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SaveConfigurationButton_Click(object sender, RoutedEventArgs e)
        {
            string apiEndpoint = ApiEndpointTextBox.Text;
            int alertDisplayTime = int.TryParse(AlertDisplayTimeTextBox.Text, out var displayTime) ? displayTime : 30;
            int alertRepetitionInterval = int.TryParse(AlertRepetitionIntervalTextBox.Text, out var repetitionInterval) ? repetitionInterval : 60;
            int messageLength = int.TryParse(MessageLengthTextBox.Text, out var maxLength) ? maxLength : 306;
            string language = LanguageTextBox.Text;

            // Save configuration settings (backend implementation to be done later)
            MessageBox.Show($"Configuration Saved:\nAPI Endpoint: {apiEndpoint}\nAlert Display Time: {alertDisplayTime} seconds\nAlert Repetition Interval: {alertRepetitionInterval} seconds\nMaximum Message Length: {messageLength} characters\nDisplay Language: {language}", "CAP Setup", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //code for logs section
        private void DownloadLogsButton_Click(object sender, RoutedEventArgs e)
        {
            // Implement download logs logic here
            MessageBox.Show("Logs downloaded", "Download", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ClearLogsButton_Click(object sender, RoutedEventArgs e)
        {
            FilteredLogs.Clear();
            MessageBox.Show("All logs have been cleared.", "Clear Logs", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ArchiveLogsButton_Click(object sender, RoutedEventArgs e)
        {
            // Implement archiving logic here
            MessageBox.Show("Logs have been archived.", "Archive Logs", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SelectedDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedDatePicker.SelectedDate.HasValue)
            {
                var selectedDate = SelectedDatePicker.SelectedDate.Value.Date;
                FilterLogsByDate(selectedDate);
            }
        }

        private void FromDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // Date range filtering logic can be implemented here if needed
        }

        private void ToDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // Date range filtering logic can be implemented here if needed
        }

        private void YesterdayLogsButton_Click(object sender, RoutedEventArgs e)
        {
            var yesterday = DateTime.Now.AddDays(-1).Date;
            SelectedDatePicker.SelectedDate = yesterday;
        }

        private void DownloadSelectedDateLogsButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedDatePicker.SelectedDate.HasValue)
            {
                var selectedDate = SelectedDatePicker.SelectedDate.Value.Date;
                var logsToDownload = Logs.Where(log => DateTime.Parse(log.Timestamp).Date == selectedDate);
                DownloadLogs(logsToDownload);
            }
        }

        private void DownloadDateRangeLogsButton_Click(object sender, RoutedEventArgs e)
        {
            if (FromDatePicker.SelectedDate.HasValue && ToDatePicker.SelectedDate.HasValue)
            {
                var fromDate = FromDatePicker.SelectedDate.Value.Date;
                var toDate = ToDatePicker.SelectedDate.Value.Date;
                var logsToDownload = Logs.Where(log => DateTime.Parse(log.Timestamp).Date >= fromDate && DateTime.Parse(log.Timestamp).Date <= toDate);
                DownloadLogs(logsToDownload);
            }
        }

        private void FilterLogsByDate(DateTime selectedDate)
        {
            var filteredLogs = Logs.Where(log => DateTime.Parse(log.Timestamp).Date == selectedDate);
            FilteredLogs.Clear();
            foreach (var log in filteredLogs)
            {
                FilteredLogs.Add(log);
            }
        }

        private void DownloadLogs(IEnumerable<LogItem> logs)
        {
            var csvContent = "Timestamp,Message,Severity\n" + string.Join("\n", logs.Select(log => $"{log.Timestamp},{log.Message},{log.Severity}"));
            var dialog = new SaveFileDialog
            {
                FileName = "logs.csv",
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*"
            };
            if (dialog.ShowDialog() == true)
            {
                System.IO.File.WriteAllText(dialog.FileName, csvContent);
                MessageBox.Show("Logs downloaded", "Download", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

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

        //NTES API 951 implementation
        //public async Task FetchAndDisplayTrains()
        //{
        //    try
        //    {
        //        var trainService = new NtesAPI951();
        //        var trainsResponse = await trainService.GetTrainsAsync("NDLS", 30);

        //        if (trainsResponse != null)
        //        {
        //            Console.WriteLine("Scheduled Trains:");
        //            if (trainsResponse.VTrainList != null)
        //            {
        //                foreach (var train in trainsResponse.VTrainList)
        //                {
        //                    Console.WriteLine($"{train.TrainNo} - {train.TrainName}");
        //                }
        //            }
        //            else
        //            {
        //                Console.WriteLine("No scheduled trains found.");
        //            }

        //            Console.WriteLine("\nRescheduled Trains:");
        //            if (trainsResponse.VRescheduledTrainList != null)
        //            {
        //                foreach (var rescheduledTrain in trainsResponse.VRescheduledTrainList)
        //                {
        //                    Console.WriteLine($"Rescheduled: {rescheduledTrain.TrainNo} - {rescheduledTrain.TrainName}");
        //                }
        //            }
        //            else
        //            {
        //                Console.WriteLine("No rescheduled trains found.");
        //            }

        //            Console.WriteLine("\nCancelled Trains:");
        //            if (trainsResponse.VCancelledTrainList != null)
        //            {
        //                foreach (var cancelledTrain in trainsResponse.VCancelledTrainList)
        //                {
        //                    Console.WriteLine($"Cancelled: {cancelledTrain.TrainNo} - {cancelledTrain.TrainName}");
        //                }
        //            }
        //            else
        //            {
        //                Console.WriteLine("No cancelled trains found.");
        //            }

        //            // Handle other lists as needed
        //        }
        //        else
        //        {
        //            Console.WriteLine("No data received from the API.");
        //        }
        //    }
        //    catch (HttpRequestException httpEx)
        //    {
        //        Console.WriteLine($"HTTP Request Error: {httpEx.Message}");
        //    }
        //    catch (JsonSerializationException jsonEx)
        //    {
        //        Console.WriteLine($"JSON Serialization Error: {jsonEx.Message}");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        //    }
        //}

        public async Task FetchAndDisplayTrains()
        {
            try
            {
                var trainService = new NtesAPI951();
                ntesApiResponse951 = await trainService.GetTrainsAsync("NDLS", 30);

                if (ntesApiResponse951 != null)
                {
                    Console.WriteLine("Scheduled Trains:");
                    if (ntesApiResponse951.VTrainList != null)
                    {
                        foreach (var train in ntesApiResponse951.VTrainList)
                        {
                            Console.WriteLine($"{train.TrainNo} - {train.TrainName}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No scheduled trains found.");
                    }

                    Console.WriteLine("\nRescheduled Trains:");
                    if (ntesApiResponse951.VRescheduledTrainList != null)
                    {
                        foreach (var rescheduledTrain in ntesApiResponse951.VRescheduledTrainList)
                        {
                            Console.WriteLine($"Rescheduled: {rescheduledTrain.TrainNo} - {rescheduledTrain.TrainName}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No rescheduled trains found.");
                    }

                    Console.WriteLine("\nCancelled Trains:");
                    if (ntesApiResponse951.VCancelledTrainList != null)
                    {
                        foreach (var cancelledTrain in ntesApiResponse951.VCancelledTrainList)
                        {
                            Console.WriteLine($"Cancelled: {cancelledTrain.TrainNo} - {cancelledTrain.TrainName}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No cancelled trains found.");
                    }

                    // Handle other lists as needed
                    Console.WriteLine("\nTrains Cancelled Due To CS:");
                    if (ntesApiResponse951.VCancelTrainDueToCS != null)
                    {
                        foreach (var train in ntesApiResponse951.VCancelTrainDueToCS)
                        {
                            Console.WriteLine($"{train.TrainNo} - {train.TrainName}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No trains cancelled due to CS found.");
                    }

                    Console.WriteLine("\nTrains Cancelled Due To CD:");
                    if (ntesApiResponse951.VCancelTrainDueToCD != null)
                    {
                        foreach (var train in ntesApiResponse951.VCancelTrainDueToCD)
                        {
                            Console.WriteLine($"{train.TrainNo} - {train.TrainName}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No trains cancelled due to CD found.");
                    }

                    Console.WriteLine("\nTrains Cancelled Due To Diversion:");
                    if (ntesApiResponse951.VCancelTrainDueToDiversion != null)
                    {
                        foreach (var train in ntesApiResponse951.VCancelTrainDueToDiversion)
                        {
                            Console.WriteLine($"{train.TrainNo} - {train.TrainName}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No trains cancelled due to diversion found.");
                    }

                    Console.WriteLine("\nTrains Due To CS:");
                    if (ntesApiResponse951.VTrainListDueToCS != null)
                    {
                        foreach (var train in ntesApiResponse951.VTrainListDueToCS)
                        {
                            Console.WriteLine($"{train.TrainNo} - {train.TrainName}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No trains due to CS found.");
                    }

                    Console.WriteLine("\nTrains Due To CD:");
                    if (ntesApiResponse951.VTrainListDueToCD != null)
                    {
                        foreach (var train in ntesApiResponse951.VTrainListDueToCD)
                        {
                            Console.WriteLine($"{train.TrainNo} - {train.TrainName}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No trains due to CD found.");
                    }

                    Console.WriteLine("\nTrains Due To DV:");
                    if (ntesApiResponse951.VTrainListDueToDV != null)
                    {
                        foreach (var train in ntesApiResponse951.VTrainListDueToDV)
                        {
                            Console.WriteLine($"{train.TrainNo} - {train.TrainName}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No trains due to DV found.");
                    }

                    // Handle RestServiceMessage and CacheUpdateTime if needed
                    if (ntesApiResponse951.RestServiceMessage != null)
                    {
                        Console.WriteLine($"\nService Message: {ntesApiResponse951.RestServiceMessage.ServiceMessage}");
                    }

                    if (ntesApiResponse951.CacheUpdateTime != null)
                    {
                        Console.WriteLine($"\nCache Update Time: {ntesApiResponse951.CacheUpdateTime.DD}/{ntesApiResponse951.CacheUpdateTime.MM}/{ntesApiResponse951.CacheUpdateTime.YYYY} " +
                                          $"{ntesApiResponse951.CacheUpdateTime.HH}:{ntesApiResponse951.CacheUpdateTime.MI}:{ntesApiResponse951.CacheUpdateTime.SECONDS}");
                    }
                }
                else
                {
                    Console.WriteLine("No data received from the API.");
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP Request Error: {httpEx.Message}");
            }
            catch (JsonSerializationException jsonEx)
            {
                Console.WriteLine($"JSON Serialization Error: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }

        private async void onWindowLoaded(object sender, RoutedEventArgs e)
        {
            await FetchAndDisplayTrains();
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

        //private void PlatformNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    string platformNumberText = PlatformNumberTextBox.Text;
        //    if (int.TryParse(platformNumberText, out int platformNumber))
        //    {
        //        PlatformSubnetTextBox.Text = $"192.168.{platformNumber}.";
        //    }
        //    else if (System.Text.RegularExpressions.Regex.IsMatch(platformNumberText, @"^\d+A$"))
        //    {
        //        string numberPart = platformNumberText.TrimEnd('A');
        //        if (int.TryParse(numberPart, out int specialPlatformNumber))
        //        {
        //            int baseNumber = 100 + specialPlatformNumber;
        //            PlatformSubnetTextBox.Text = $"192.168.{baseNumber}.";
        //        }
        //        else
        //        {
        //            PlatformSubnetTextBox.Text = string.Empty;
        //        }
        //    }
        //    else
        //    {
        //        PlatformSubnetTextBox.Text = string.Empty;
        //    }
        //}

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
                        settingContent = JsonConvert.SerializeObject(_rmsSettingsManager.LoadRmsSettings(), Formatting.Indented);
                        break;
                }

                SettingsContentTextBox.Text = settingContent;
            }
        }

        //private void DeviceTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (PlatformListView.SelectedItem is Platform selectedPlatform && DeviceTypeComboBox.SelectedItem is DeviceType selectedDeviceType)
        //    {
        //        try
        //        {
        //            string nextIp = _platformDeviceManager.CalculateNextIpAddress(selectedPlatform, selectedDeviceType);
        //            DeviceIpAddressTextBox.Text = nextIp;
        //        }
        //        catch (InvalidOperationException ex)
        //        {
        //            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        }
        //    }
        //}

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

    }
}
