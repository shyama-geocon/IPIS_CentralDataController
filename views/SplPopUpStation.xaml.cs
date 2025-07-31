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
using IpisCentralDisplayController.models;
using static MongoDB.Libmongocrypt.CryptContext;

namespace IpisCentralDisplayController.views
{

    public partial class SplPopUpStation : Window
    {
        private StationViewModel _viewModel;

        public ActiveTrain Train { get; set; }
        public byte StatusCode { get; set; }

        public SplPopUpStation(ActiveTrain train, string title ,byte statuscode)
        {
            InitializeComponent();
            _viewModel = new StationViewModel();
            DataContext = _viewModel;

            _viewModel.FieldText = $"{title}:";
            _viewModel.Title = title;
            StatusCode = statuscode;

            Train = train;
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
                "StationCode", "StationNameEnglish", "StationNameHindi", "StationNameAssamese", "StationNameBangla",
                "StationNameDogri", "StationNameGujarati", "StationNameKannada", "StationNameKonkani",
                "StationNameMalayalam", "StationNameMarathi", "StationNameManipuri", "StationNameNepali",
                "StationNameOdia", "StationNamePunjabi", "StationNameSanskrit", "StationNameSindhi",
                "StationNameTamil", "StationNameTelugu", "StationNameUrdu"
            };
            return headers.SequenceEqual(expectedHeaders);
        }

       
        private RegionalLanguage ParseRegionalLanguage(string language)
        {
            if (Enum.TryParse(language, true, out RegionalLanguage regionalLanguage))
            {
                return regionalLanguage;
            }
            return RegionalLanguage.ENGLISH; // Default to English if parsing fails
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
                //if (StatusCode == 0x10)
                //{
                //    Train.DivertedStationCode = viewModel.SelectedStation.StationCode;
                //    Train.DivertedStationNameEnglish = viewModel.SelectedStation.StationNameEnglish;
                //    Train.DivertedStationNameHindi = viewModel.SelectedStation.StationNameHindi;
                //}
                //else if(StatusCode == 0x08)
                //{
                //    Train.TerminatedStationNameEnglish = viewModel.SelectedStation.StationNameEnglish;
                //    Train.TerminatedStationNameHindi = viewModel.SelectedStation.StationNameHindi;
                //    Train.TerminatedStationCode = viewModel.SelectedStation.StationCode;
                //}

                Train.SplStationNameEnglish = viewModel.SelectedStation.StationNameEnglish;
                Train.SplStationNameHindi = viewModel.SelectedStation.StationNameHindi;
                Train.SplStationCode = viewModel.SelectedStation.StationCode;



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
