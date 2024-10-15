using IpisCentralDisplayController.Helpers;
using IpisCentralDisplayController.managers;
using IpisCentralDisplayController.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;

namespace IpisCentralDisplayController.views
{
    public class TrainMasterViewModel : INotifyPropertyChanged
    {
        private TrainMaster _selectedTrain;
        private TrainMasterManager _trainMasterManager;
        public ObservableCollection<string> CoachList { get; set; }

        // Properties for STA Hours and Minutes
        public int STA_Hours
        {
            get => SelectedTrain?.STA?.Hours ?? 0;
            set
            {
                if (SelectedTrain != null)
                {
                    var currentMinutes = SelectedTrain.STA?.Minutes ?? 0;
                    SelectedTrain.STA = new TimeSpan(value, currentMinutes, 0);
                    OnPropertyChanged(nameof(STA_Hours));
                    OnPropertyChanged(nameof(SelectedTrain.STA));
                }
            }
        }

        public int STA_Minutes
        {
            get => SelectedTrain?.STA?.Minutes ?? 0;
            set
            {
                if (SelectedTrain != null)
                {
                    var currentHours = SelectedTrain.STA?.Hours ?? 0;
                    SelectedTrain.STA = new TimeSpan(currentHours, value, 0);
                    OnPropertyChanged(nameof(STA_Minutes));
                    OnPropertyChanged(nameof(SelectedTrain.STA));
                }
            }
        }

        // Properties for STD Hours and Minutes
        public int STD_Hours
        {
            get => SelectedTrain?.STD?.Hours ?? 0;
            set
            {
                if (SelectedTrain != null)
                {
                    var currentMinutes = SelectedTrain.STD?.Minutes ?? 0;
                    SelectedTrain.STD = new TimeSpan(value, currentMinutes, 0);
                    OnPropertyChanged(nameof(STD_Hours));
                    OnPropertyChanged(nameof(SelectedTrain.STD));
                }
            }
        }

        public int STD_Minutes
        {
            get => SelectedTrain?.STD?.Minutes ?? 0;
            set
            {
                if (SelectedTrain != null)
                {
                    var currentHours = SelectedTrain.STD?.Hours ?? 0;
                    SelectedTrain.STD = new TimeSpan(currentHours, value, 0);
                    OnPropertyChanged(nameof(STD_Minutes));
                    OnPropertyChanged(nameof(SelectedTrain.STD));
                }
            }
        }

        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (_searchQuery != value)
                {
                    _searchQuery = value;
                    OnPropertyChanged(nameof(SearchQuery));
                    TrainView.Refresh();  // Refresh to apply the filter
                }
            }
        }

        public ICollectionView TrainView { get; set; }

        public TrainMasterViewModel()
        {
            var jsonHelperAdapter = new SettingsJsonHelperAdapter();
            Trains = new ObservableCollection<TrainMaster>();
            _trainMasterManager = new TrainMasterManager(jsonHelperAdapter);
            LoadTrains();

            TrainView = CollectionViewSource.GetDefaultView(Trains);
            TrainView.Filter = TrainFilter;
        }

        private bool TrainFilter(object item)
        {
            if (item is TrainMaster train)
            {
                if (string.IsNullOrEmpty(SearchQuery))
                    return true;

                return train.TrainNumber.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                       train.TrainNameEnglish.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase);
            }
            return false;
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
                    CoachList = new ObservableCollection<string>(_selectedTrain?.CoachList ?? new List<string>());
                    OnPropertyChanged(nameof(SelectedTrain));
                    OnPropertyChanged(nameof(CoachList));
                    OnPropertyChanged(nameof(STA_Hours));
                    OnPropertyChanged(nameof(STA_Minutes));
                    OnPropertyChanged(nameof(STD_Hours));
                    OnPropertyChanged(nameof(STD_Minutes));
                }
            }
        }

        //public void LoadTrains()
        //{
        //    var trains = _trainMasterManager.LoadTrainMasters();
        //    Trains.Clear();
        //    foreach (var train in trains)
        //    {
        //        Trains.Add(train);
        //    }
        //}

        public void LoadTrains()
        {
            var trains = _trainMasterManager.LoadTrainMasters();
            Trains.Clear();
            foreach (var train in trains)
            {
                Trains.Add(train);
            }

            if (SelectedTrain != null)
            {
                // Find the train that was previously selected (if still present in the list)
                SelectedTrain = Trains.FirstOrDefault(t => t.TrainNumber == SelectedTrain.TrainNumber);

                // If the selected train is found, update the CoachList
                if (SelectedTrain != null)
                {
                    CoachList = new ObservableCollection<string>(SelectedTrain.CoachList ?? new List<string>());
                }
                else
                {
                    CoachList.Clear();
                }

                OnPropertyChanged(nameof(CoachList));
            }
            else if (Trains.Any())
            {
                // If no train is selected, select the first one in the list
                SelectedTrain = Trains.First();
                CoachList = new ObservableCollection<string>(SelectedTrain.CoachList ?? new List<string>());
                OnPropertyChanged(nameof(CoachList));
            }

            OnPropertyChanged(nameof(Trains));
        }

        public void AddTrain(TrainMaster train)
        {
            if (train != null && !string.IsNullOrEmpty(train.TrainNumber))
            {
                _trainMasterManager.AddTrainMaster(train);
                Trains.Add(train);
                SelectedTrain = train;
                CoachList = new ObservableCollection<string>(train.CoachList);
                OnPropertyChanged(nameof(CoachList));
            }
        }

        public void UpdateTrain(TrainMaster train)
        {
            if (train != null && !string.IsNullOrEmpty(train.TrainNumber))
            {
                // Update the train in the TrainMasterManager
                _trainMasterManager.UpdateTrainMaster(train);

                // Find the existing train in the collection
                var existingTrain = Trains.FirstOrDefault(t => t.TrainNumber == train.TrainNumber);
                if (existingTrain != null)
                {
                    // Update the existing train in the collection
                    var index = Trains.IndexOf(existingTrain);
                    Trains[index] = train;
                }
                else
                {
                    // Add the train if it wasn't found
                    Trains.Add(train);
                }

                // Set the selected train to the updated one
                SelectedTrain = train;
                CoachList = new ObservableCollection<string>(train.CoachList);
                OnPropertyChanged(nameof(CoachList));
                OnPropertyChanged(nameof(Trains)); // Notify that the trains list has changed
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
