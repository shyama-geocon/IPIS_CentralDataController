using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IpisCentralDisplayController
{
    public class TrainListViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<TrainViewModel> Trains { get; set; }

        public TrainListViewModel()
        {
            Trains = new ObservableCollection<TrainViewModel>();
        }

        public async Task FetchAndDisplayTrains()
        {
            try
            {
                var trainService = new NtesAPI951();
                var trainsResponse = await trainService.GetTrainsAsync("NDLS", 30);

                if (trainsResponse != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Trains.Clear();

                        if (trainsResponse.VTrainList != null)
                        {
                            foreach (var train in trainsResponse.VTrainList)
                            {
                                Trains.Add(new TrainViewModel(train));
                            }
                        }

                        if (trainsResponse.VRescheduledTrainList != null)
                        {
                            foreach (var rescheduledTrain in trainsResponse.VRescheduledTrainList)
                            {
                                Trains.Add(new TrainViewModel(rescheduledTrain));
                            }
                        }

                        if (trainsResponse.VCancelledTrainList != null)
                        {
                            foreach (var cancelledTrain in trainsResponse.VCancelledTrainList)
                            {
                                Trains.Add(new TrainViewModel(cancelledTrain));
                            }
                        }
                    });
                }
                else
                {
                    MessageBox.Show("No data received from the API.");
                }
            }
            catch (HttpRequestException httpEx)
            {
                MessageBox.Show($"HTTP Request Error: {httpEx.Message}");
            }
            catch (JsonSerializationException jsonEx)
            {
                MessageBox.Show($"JSON Serialization Error: {jsonEx.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
