using IpisCentralDisplayController.models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IpisCentralDisplayController.views
{
    public class ActiveTrainViewModel : INotifyPropertyChanged
    {
        //  public ActiveTrain ActiveTrain { get; set; }

        private ActiveTrain _activeTrain;
        public ActiveTrain ActiveTrain
        {
            get { return _activeTrain; }
            set { 
                _activeTrain = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public ActiveTrainViewModel(ActiveTrain activeTrain)
        {
            ActiveTrain = activeTrain ?? new ActiveTrain();
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Save()
        {
            // Logic to save the train data
        }

        public void Cancel()
        {
            // Logic to handle cancel action
        }
    }
}
