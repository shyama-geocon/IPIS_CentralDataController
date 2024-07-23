using IpisCentralDisplayController.Managers;
using IpisCentralDisplayController.models;
using System;
using System.Linq;
using System.Windows;

namespace IpisCentralDisplayController.views
{
    public partial class StationInfoWindow : Window
    {
        private readonly StationInfoManager _stationInfoManager;

        public StationInfoWindow(StationInfoManager stationInfoManager)
        {
            InitializeComponent();
            _stationInfoManager = stationInfoManager;
            LoadRegionalLanguages();
            LoadStationInfo();
        }

        private void LoadRegionalLanguages()
        {
            RegLanguageComboBox.ItemsSource = Enum.GetValues(typeof(RegionalLanguage)).Cast<RegionalLanguage>();
        }

        private void LoadStationInfo()
        {
            var stationInfo = _stationInfoManager.LoadStationInfo();
            if (stationInfo != null)
            {
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

        private void SaveStationInfoButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(StationCodeTextBox.Text) ||
                string.IsNullOrEmpty(StationNameEnTextBox.Text) ||
                RegLanguageComboBox.SelectedItem == null ||
                !StationLatTextBox.Value.HasValue ||
                !StationLongTextBox.Value.HasValue ||
                !StationAltTextBox.Value.HasValue ||
                !StationPlatformsTextBox.Value.HasValue ||
                !NumberOfSplPlatformsTextBox.Value.HasValue ||
                !NumberOfStationEntrancesTextBox.Value.HasValue ||
                !NumberOfPlatformBridgesTextBox.Value.HasValue)
            {
                MessageBox.Show("All fields are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var stationInfo = new StationInfo
            {
                StationCode = StationCodeTextBox.Text,
                RegionalLanguage = (RegionalLanguage)RegLanguageComboBox.SelectedItem,
                StationNameEnglish = StationNameEnTextBox.Text,
                StationNameHindi = StationNameHiTextBox.Text,
                StationNameRegional = StationNameRLTextBox.Text,
                Latitude = (double)StationLatTextBox.Value.Value,
                Longitude = (double)StationLongTextBox.Value.Value,
                Altitude = (double)StationAltTextBox.Value.Value,
                NumberOfPlatforms = StationPlatformsTextBox.Value.Value,
                NumberOfSplPlatforms = NumberOfSplPlatformsTextBox.Value.Value,
                NumberOfStationEntrances = NumberOfStationEntrancesTextBox.Value.Value,
                NumberOfPlatformBridges = NumberOfPlatformBridgesTextBox.Value.Value
            };

            _stationInfoManager.SaveStationInfo(stationInfo);
            MessageBox.Show("Station information saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }

        private void CancelStationInfoButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
