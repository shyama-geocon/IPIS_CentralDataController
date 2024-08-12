using IpisCentralDisplayController.Helpers;
using IpisCentralDisplayController.managers;
using IpisCentralDisplayController.models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace IpisCentralDisplayController.views
{
    public class TrainMasterViewModel : INotifyPropertyChanged
    {
        private TrainMaster _selectedTrain;
        private TrainMasterManager _trainMasterManager;

        public TrainMasterViewModel()
        {
            var jsonHelperAdapter = new SettingsJsonHelperAdapter();
            Trains = new ObservableCollection<TrainMaster>();
            _trainMasterManager = new TrainMasterManager(jsonHelperAdapter);
            LoadTrains();
        }

        public ObservableCollection<TrainMaster> Trains { get; set; }

        public TrainMaster SelectedTrain
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

        public void LoadTrains()
        {
            var trains = _trainMasterManager.LoadTrainMasters();
            Trains.Clear();
            foreach (var train in trains)
            {
                Trains.Add(train);
            }
        }

        public void AddTrain(TrainMaster train)
        {
            if (train != null && !string.IsNullOrEmpty(train.TrainNumber))
            {
                _trainMasterManager.AddTrainMaster(train);
                Trains.Add(train);
                SelectedTrain = null;
            }
        }

        public void UpdateTrain(TrainMaster train)
        {
            if (train != null && !string.IsNullOrEmpty(train.TrainNumber))
            {
                _trainMasterManager.UpdateTrainMaster(train);
                LoadTrains(); // Refresh the list
            }
        }

        public void DeleteTrain(TrainMaster train)
        {
            if (train != null && !string.IsNullOrEmpty(train.TrainNumber))
            {
                _trainMasterManager.DeleteTrainMaster(train.TrainNumber);
                Trains.Remove(train);
                SelectedTrain = null;
            }
        }

        public void DeleteAllTrains()
        {
            _trainMasterManager.DeleteAllTrainMasters();
            Trains.Clear();
        }

        public void ReplaceTrains(List<TrainMaster> newTrains, bool replaceOnlyNTES = false)
        {
            if (newTrains == null || !newTrains.Any())
            {
                return;
            }

            if (replaceOnlyNTES)
            {
                Trains = new ObservableCollection<TrainMaster>(Trains.Where(t => !t.IsFromNTES));
            }
            else
            {
                Trains.Clear();
            }

            foreach (var train in newTrains)
            {
                Trains.Add(train);
            }

            _trainMasterManager.SaveTrainMasters(Trains.ToList());

            SelectedTrain = null;
            OnPropertyChanged(nameof(Trains));
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
