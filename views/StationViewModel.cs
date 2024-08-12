using IpisCentralDisplayController.Helpers;
using IpisCentralDisplayController.managers;
using IpisCentralDisplayController.Managers;
using IpisCentralDisplayController.models;
using IpisCentralDisplayController.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace IpisCentralDisplayController.views
{
    public class StationViewModel : INotifyPropertyChanged
    {
        private StationManager _stationManager;
        private Station _selectedStation;

        public StationViewModel()
        {
            var jsonHelperAdapter = new SettingsJsonHelperAdapter();
            _stationManager = new StationManager(jsonHelperAdapter);
            Stations = new ObservableCollection<Station>();
            LoadStations();
        }

        public ObservableCollection<Station> Stations { get; set; }

        public Station SelectedStation
        {
            get => _selectedStation;
            set
            {
                if (_selectedStation != value)
                {
                    _selectedStation = value;
                    OnPropertyChanged(nameof(SelectedStation));
                }
            }
        }

        public void LoadStations()
        {
            var stations = _stationManager.LoadStations();
            Stations.Clear();
            foreach (var station in stations)
            {
                Stations.Add(station);
            }
        }

        public void AddStation(Station station)
        {
            if (station != null && !string.IsNullOrEmpty(station.StationCode))
            {
                _stationManager.AddStation(station);
                Stations.Add(station);
                SelectedStation = null; // Clear the selection after adding
            }
        }

        public void UpdateStation(Station station)
        {
            if (station != null && !string.IsNullOrEmpty(station.StationCode))
            {
                _stationManager.UpdateStation(station);
                LoadStations(); // Refresh the list
            }
        }

        public void DeleteStation(Station station)
        {
            if (station != null && !string.IsNullOrEmpty(station.StationCode))
            {
                _stationManager.DeleteStation(station.StationCode);
                Stations.Remove(station);
                SelectedStation = null; // Clear the selection after deletion
            }
        }

        public void DeleteAllStations()
        {
            _stationManager.DeleteAllStations();
            Stations.Clear();
        }

        public void ReplaceStations(IEnumerable<Station> newStations)
        {
            if (newStations == null) return;

            Stations.Clear();

            foreach (var station in newStations)
            {
                Stations.Add(station);
            }

            _stationManager.SaveStations(Stations.ToList());

            SelectedStation = null;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
