using IpisCentralDisplayController.managers;
using IpisCentralDisplayController.Managers;
using IpisCentralDisplayController.models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for StationDbWindow.xaml
    /// </summary>
    public partial class StationDbWindow : Window
    {
        private StationViewModel _viewModel;
        public StationDbWindow()
        {
            InitializeComponent();
            _viewModel = new StationViewModel();
            DataContext = _viewModel;
        }

        private void AddStationButton_Click(object sender, RoutedEventArgs e)
        {
            // Prompt user to enter station details
            var stationCode = PromptUserForInput("Enter Station Code:");
            var stationNameEnglish = PromptUserForInput("Enter Station Name (English):");

            if (!string.IsNullOrEmpty(stationCode) && !string.IsNullOrEmpty(stationNameEnglish))
            {
                var newStation = new Station
                {
                    StationCode = stationCode,
                    StationNameEnglish = stationNameEnglish
                    // Initialize other fields as needed
                };

                _viewModel.AddStation(newStation);
                MessageBox.Show("Station added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Station Code and Name (English) are required.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditStationButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedStation != null)
            {
                _viewModel.UpdateStation(_viewModel.SelectedStation);
                MessageBox.Show("Station details updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select a station to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteStationButton_Click(object sender, RoutedEventArgs e)
        {
            if (StationListView.SelectedItems.Count > 0)
            {
                var result = MessageBox.Show("Are you sure you want to delete the selected stations?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (var selectedItem in StationListView.SelectedItems)
                    {
                        var selectedStation = selectedItem as Station;
                        if (selectedStation != null)
                        {
                            _viewModel.DeleteStation(selectedStation);
                        }
                    }
                    MessageBox.Show("Selected stations deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Please select stations to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteAllStationsButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete all stations?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                _viewModel.DeleteAllStations();
                MessageBox.Show("All stations deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private async void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Import Station Data"
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

                    var stations = new List<Station>();

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

                        var station = new Station
                        {
                            StationCode = fields[0],
                            StationNameEnglish = fields[1],
                            StationNameHindi = fields[2],
                            StationNameAssamese = fields[3],
                            StationNameBangla = fields[4],
                            StationNameDogri = fields[5],
                            StationNameGujarati = fields[6],
                            StationNameKannada = fields[7],
                            StationNameKonkani = fields[8],
                            StationNameMalayalam = fields[9],
                            StationNameMarathi = fields[10],
                            StationNameManipuri = fields[11],
                            StationNameNepali = fields[12],
                            StationNameOdia = fields[13],
                            StationNamePunjabi = fields[14],
                            StationNameSanskrit = fields[15],
                            StationNameSindhi = fields[16],
                            StationNameTamil = fields[17],
                            StationNameTelugu = fields[18],
                            StationNameUrdu = fields[19]
                        };

                        stations.Add(station);

                        // Update the status every 10 entries (or as preferred)
                        if (i % 10 == 0)
                        {
                            tb_status.Text = $"Processing {i} of {lines.Length - 1} stations...";
                            await Task.Delay(50); // Allows UI to update
                        }
                    }

                    //// Add stations to the station manager and update the view model
                    //foreach (var station in stations)
                    //{
                    //    _viewModel.AddStation(station);
                    //}

                    _viewModel.ReplaceStations(stations);

                    tb_status.Text = "Station data imported successfully.";
                    MessageBox.Show("Station data imported successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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


        private bool ValidateCsvHeaders(string[] headers)
        {
            string[] expectedHeaders = {
                "StationCode", "StationNameEnglish", "StationNameHindi", "StationNameAssamese", "StationNameBangla",
                "StationNameDogri", "StationNameGujarati", "StationNameKannada", "StationNameKonkani",
                "StationNameMalayalam", "StationNameMarathi", "StationNameManipuri", "StationNameNepali",
                "StationNameOdia", "StationNamePunjabi", "StationNameSanskrit", "StationNameSindhi",
                "StationNameTamil", "StationNameTelugu", "StationNameUrdu"
            };
            return headers.SequenceEqual(expectedHeaders);
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Export Station Data"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (var writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        // Write the header line
                        writer.WriteLine("StationCode,StationNameEnglish,StationNameHindi,StationNameAssamese,StationNameBangla," +
                                         "StationNameDogri,StationNameGujarati,StationNameKannada,StationNameKonkani," +
                                         "StationNameMalayalam,StationNameMarathi,StationNameManipuri,StationNameNepali," +
                                         "StationNameOdia,StationNamePunjabi,StationNameSanskrit,StationNameSindhi," +
                                         "StationNameTamil,StationNameTelugu,StationNameUrdu");

                        // Write each station as a line in the CSV
                        foreach (var station in _viewModel.Stations)
                        {
                            writer.WriteLine($"{station.StationCode},{station.StationNameEnglish},{station.StationNameHindi},{station.StationNameAssamese}," +
                                             $"{station.StationNameBangla},{station.StationNameDogri},{station.StationNameGujarati},{station.StationNameKannada}," +
                                             $"{station.StationNameKonkani},{station.StationNameMalayalam},{station.StationNameMarathi},{station.StationNameManipuri}," +
                                             $"{station.StationNameNepali},{station.StationNameOdia},{station.StationNamePunjabi},{station.StationNameSanskrit}," +
                                             $"{station.StationNameSindhi},{station.StationNameTamil},{station.StationNameTelugu},{station.StationNameUrdu}");
                        }
                    }

                    MessageBox.Show("Station data exported successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while exporting the data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private RegionalLanguage ParseRegionalLanguage(string language)
        {
            if (Enum.TryParse(language, true, out RegionalLanguage regionalLanguage))
            {
                return regionalLanguage;
            }
            return RegionalLanguage.ENGLISH; // Default to English if parsing fails
        }

        private void PlayRegionalSoundButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BrowseRegionalSoundButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BrowseHindiSoundButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PlayHindiSoundButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BrowseEnglishSoundButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PlayEnglishSoundButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BrowseBaseDirectoryButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UpdateStationButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedStation == null)
            {
                MessageBox.Show("Please select a station to update.", "No Station Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Prompt the user for confirmation
            var result = MessageBox.Show("Are you sure you want to update the selected station?", "Confirm Update", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Update the selected station
                    _viewModel.UpdateStation(_viewModel.SelectedStation);
                    MessageBox.Show("Station updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while updating the station: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UseThisButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is StationViewModel viewModel && viewModel.SelectedStation != null)
            {
                DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select a station.", "No Station Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }



}
