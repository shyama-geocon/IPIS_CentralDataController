using IpisCentralDisplayController.Helpers;
using IpisCentralDisplayController.Managers;
using IpisCentralDisplayController.models;
using IpisCentralDisplayController.ntes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
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
using System.Windows.Shapes;

namespace IpisCentralDisplayController.views
{
    /// <summary>
    /// Interaction logic for TrainMasterDbWindow.xaml
    /// </summary>
    public partial class TrainMasterDbWindow : Window, INotifyPropertyChanged
    {
        private StationInfoManager _stationInfoManager;
        private StationInfo _stationInfo;
        private string _trainGroupBoxHeader;
        public event PropertyChangedEventHandler PropertyChanged;
        private TrainMasterViewModel _viewModel;

        public string TrainGroupBoxHeader
        {
            get => _trainGroupBoxHeader;
            set
            {
                _trainGroupBoxHeader = value;
                OnPropertyChanged(nameof(TrainGroupBoxHeader));
            }
        }

        public TrainMasterDbWindow()
        {
            InitializeComponent();
            _viewModel = new TrainMasterViewModel();  // Assuming TrainMasterViewModel is the name of your ViewModel class
            this.DataContext = _viewModel;
            InitializeStationInfo();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void InitializeStationInfo()
        {
            try
            {
                var stationInfoManager = new StationInfoManager(new SettingsJsonHelperAdapter());
                _stationInfo = stationInfoManager.LoadStationInfo();

                if (_stationInfo == null)
                {
                    MessageBox.Show("No valid station information found. Please ensure that station data is available before proceeding.", "Station Information Missing", MessageBoxButton.OK, MessageBoxImage.Warning);
                    this.Close();
                }
                else
                {
                    gb_header.Header = $"Trains for Station Code: {_stationInfo.StationCode}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading station information: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private async void FetchFromNTESButton_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait; // Set the cursor to loading
            tb_status.Text = "Fetching data from NTES...";

            try
            {
                var ntesApi = new NtesAPI952();
                var ntesResponse = await ntesApi.GetTrainsAsync(_stationInfo.StationCode);

                if (ntesResponse?.VTrainList == null || !ntesResponse.VTrainList.Any())
                {
                    MessageBox.Show("No trains found for the specified station.", "No Data", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Convert the list of NtesTrain952 objects to TrainMaster objects using the constructor
                var trains = ntesResponse.VTrainList.Select(nt => new TrainMaster(nt)).ToList();

                var result = MessageBox.Show("Do you want to replace the entire list of trains (Yes), update only the trains fetched from NTES (No), or cancel this operation (Cancel)?",
                                             "Replace Trains",
                                             MessageBoxButton.YesNoCancel,
                                             MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Replace all trains
                    _viewModel.ReplaceTrains(trains);
                    MessageBox.Show("All trains have been replaced with the data from NTES.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (result == MessageBoxResult.No)
                {
                    // Replace only trains fetched from NTES
                    _viewModel.ReplaceTrains(trains, true);
                    MessageBox.Show("Only NTES trains have been replaced with the data from NTES.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }


                tb_status.Text = "NTES data fetched and processed successfully.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while fetching data from NTES: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                tb_status.Text = "Error occurred during data fetch.";
            }
            finally
            {
                Mouse.OverrideCursor = null; // Reset the cursor to default
            }
        }

        private void AddTrainButton_Click(object sender, RoutedEventArgs e)
        {
            // Prompt user to enter train details
            var trainNumber = PromptUserForInput("Enter Train Number:");
            var trainNameEnglish = PromptUserForInput("Enter Train Name (English):");

            if (!string.IsNullOrEmpty(trainNumber) && !string.IsNullOrEmpty(trainNameEnglish))
            {
                var newTrain = new TrainMaster
                {
                    TrainNumber = trainNumber,
                    TrainNameEnglish = trainNameEnglish,
                    // Initialize other fields as needed
                };

                _viewModel.AddTrain(newTrain);
                MessageBox.Show("Train added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Train Number and Name (English) are required.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateTrainButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedTrain != null)
            {
                _viewModel.UpdateTrain(_viewModel.SelectedTrain);
                MessageBox.Show("Train details updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select a train to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteTrainButton_Click(object sender, RoutedEventArgs e)
        {
            if (TrainListView.SelectedItems.Count > 0)
            {
                var result = MessageBox.Show("Are you sure you want to delete the selected trains?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (var selectedItem in TrainListView.SelectedItems)
                    {
                        var selectedTrain = selectedItem as TrainMaster;
                        if (selectedTrain != null)
                        {
                            _viewModel.DeleteTrain(selectedTrain);
                        }
                    }
                    MessageBox.Show("Selected trains deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Please select trains to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteAllTrainsButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete all trains?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                _viewModel.DeleteAllTrains();
                MessageBox.Show("All trains deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Import Train Data"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    Mouse.OverrideCursor = Cursors.Wait; // Set the cursor to loading

                    string[] lines = File.ReadAllLines(openFileDialog.FileName);
                    if (lines.Length < 2)
                    {
                        MessageBox.Show("The CSV file is empty or incorrectly formatted.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Check header format
                    string[] headers = lines[0].Split(',');
                    if (!ValidateCsvHeaders(headers))
                    {
                        MessageBox.Show("The CSV file format is incorrect. Please check the file and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var trains = new List<TrainMaster>();

                    // Process each line after the header
                    for (int i = 1; i < lines.Length; i++)
                    {
                        string[] fields = lines[i].Split(',');

                        // Ensure that the number of fields is correct
                        if (fields.Length != headers.Length)
                        {
                            MessageBox.Show($"Line {i + 1} in the CSV file is incorrectly formatted.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        var train = new TrainMaster
                        {
                            TrainNumber = fields[0],
                            TrainNameEnglish = fields[1],
                            TrainNameHindi = fields[2],
                            TrainNameRegional = fields[3],
                            SrcCode = fields[4],
                            SrcNameEnglish = fields[5],
                            SrcNameHindi = fields[6],
                            SrcNameRegional = fields[7],
                            DestCode = fields[8],
                            DestNameEnglish = fields[9],
                            DestNameHindi = fields[10],
                            DestNameRegional = fields[11],
                            STA = ParseTime(fields[12]),
                            STD = ParseTime(fields[13]),
                            DaysOfDeparture = fields[14],
                            DaysOfArrival = fields[15],
                            Platform = fields[16],
                            CoachSequence = fields[17],
                            TrainType = fields[18]
                        };

                        trains.Add(train);

                        // Update the status every 10 entries
                        if (i % 10 == 0)
                        {
                            tb_status.Text = $"Processing {i} of {lines.Length - 1} trains...";
                            await Task.Delay(50); // Allows UI to update
                        }
                    }

                    _viewModel.ReplaceTrains(trains);

                    tb_status.Text = "Train data imported successfully.";
                    MessageBox.Show("Train data imported successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while importing the CSV file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    tb_status.Text = "Error occurred during import.";
                }
                finally
                {
                    Mouse.OverrideCursor = null; // Reset the cursor to default
                }
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Export Train Data"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (var writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        // Write the header line
                        writer.WriteLine("TrainNumber,TrainNameEnglish,TrainNameHindi,TrainNameRegional," +
                                         "SrcCode,SrcNameEnglish,SrcNameHindi,SrcNameRegional," +
                                         "DestCode,DestNameEnglish,DestNameHindi,DestNameRegional," +
                                         "STA,STD,DaysOfDeparture,DaysOfArrival,Platform,CoachSequence,TrainType");

                        // Write each train as a line in the CSV
                        foreach (var train in _viewModel.Trains)
                        {
                            writer.WriteLine($"{train.TrainNumber},{train.TrainNameEnglish},{train.TrainNameHindi},{train.TrainNameRegional}," +
                                             $"{train.SrcCode},{train.SrcNameEnglish},{train.SrcNameHindi},{train.SrcNameRegional}," +
                                             $"{train.DestCode},{train.DestNameEnglish},{train.DestNameHindi},{train.DestNameRegional}," +
                                             $"{train.STA?.ToString(@"hh\:mm")},{train.STD?.ToString(@"hh\:mm")},{train.DaysOfDeparture},{train.DaysOfArrival}," +
                                             $"{train.Platform},{train.CoachSequence},{train.TrainType}");
                        }
                    }

                    MessageBox.Show("Train data exported successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while exporting the data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private string PromptUserForInput(string message)
        {
            var inputDialog = new InputDialog(message);
            if (inputDialog.ShowDialog() == true)
            {
                return inputDialog.ResponseText;
            }
            return null;
        }

        private bool ValidateCsvHeaders(string[] headers)
        {
            string[] expectedHeaders = {
            "TrainNumber", "TrainNameEnglish", "TrainNameHindi", "TrainNameRegional",
            "SrcCode", "SrcNameEnglish", "SrcNameHindi", "SrcNameRegional",
            "DestCode", "DestNameEnglish", "DestNameHindi", "DestNameRegional",
            "STA", "STD", "DaysOfDeparture", "DaysOfArrival", "Platform", "CoachSequence", "TrainType"
        };
            return headers.SequenceEqual(expectedHeaders);
        }

        private TimeSpan? ParseTime(string timeString)
        {
            if (TimeSpan.TryParseExact(timeString, @"hh\:mm", CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }
            return null; // Handle invalid or empty time strings
        }
    }
}
