using IpisCentralDisplayController.models;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for ActiveTrainWindow.xaml
    /// </summary>
    public partial class ActiveTrainWindow : Window
    {
        private readonly ActiveTrainViewModel _viewModel;
        public ActiveTrainWindow()
        {
            InitializeComponent();
        }

        public ActiveTrainWindow(ActiveTrain activeTrain)
        {
            InitializeComponent();

            
            _viewModel = new ActiveTrainViewModel(activeTrain);
            DataContext = _viewModel;
        }

        private void OpenSrcCodeWindow(object sender, RoutedEventArgs e)
        {
            var stationDbWindow = new StationDbWindow();
            if (stationDbWindow.ShowDialog() == true)
            {
                var selectedStation = (stationDbWindow.DataContext as StationViewModel)?.SelectedStation;
                if (selectedStation != null)
                {
                    _viewModel.ActiveTrain.SrcCode = selectedStation.StationCode;
                    _viewModel.ActiveTrain.SrcNameEnglish = selectedStation.StationNameEnglish;
                    _viewModel.ActiveTrain.SrcNameHindi = selectedStation.StationNameHindi;
                }
            }
        }

        private void OpenDestCodeWindow(object sender, RoutedEventArgs e)
        {
            var stationDbWindow = new StationDbWindow();
            if (stationDbWindow.ShowDialog() == true)
            {
                var selectedStation = (stationDbWindow.DataContext as StationViewModel)?.SelectedStation;
                if (selectedStation != null)
                {
                    _viewModel.ActiveTrain.DestCode = selectedStation.StationCode;
                    _viewModel.ActiveTrain.DestNameEnglish = selectedStation.StationNameEnglish;
                    _viewModel.ActiveTrain.DestNameHindi = selectedStation.StationNameHindi;
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Validation for mandatory fields
            if (string.IsNullOrWhiteSpace(_viewModel.ActiveTrain.TrainNumber) ||
                string.IsNullOrWhiteSpace(_viewModel.ActiveTrain.TrainNameEnglish) ||
                string.IsNullOrWhiteSpace(_viewModel.ActiveTrain.TrainNameHindi) ||
                string.IsNullOrWhiteSpace(_viewModel.ActiveTrain.SrcCode) ||
                string.IsNullOrWhiteSpace(_viewModel.ActiveTrain.SrcNameEnglish) ||
                string.IsNullOrWhiteSpace(_viewModel.ActiveTrain.SrcNameHindi) ||
                string.IsNullOrWhiteSpace(_viewModel.ActiveTrain.DestCode) ||
                string.IsNullOrWhiteSpace(_viewModel.ActiveTrain.DestNameEnglish) ||
                string.IsNullOrWhiteSpace(_viewModel.ActiveTrain.DestNameHindi) ||
                string.IsNullOrWhiteSpace(_viewModel.ActiveTrain.TrainType) ||
                _viewModel.ActiveTrain.STA == null ||
                _viewModel.ActiveTrain.STD == null ||
                string.IsNullOrWhiteSpace(_viewModel.ActiveTrain.CoachSequence))
            {
                MessageBox.Show("Please fill in all mandatory fields before saving.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Are you sure you want to save the changes?", "Confirm Save", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                _viewModel.ActiveTrain.Ref = TrainSource.USER;
                _viewModel.ActiveTrain.UpdateModificationTime();
                this.DialogResult = true;
                this.Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
