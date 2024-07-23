using IpisCentralDisplayController.models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace IpisCentralDisplayController.views
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public TrainListViewModel TrainListVM { get; set; }
        public ColorViewModel ColorVM { get; set; }

        private DisplayStyle _selectedStyle;
        private string _testText;

        public ObservableCollection<DisplayStyle> DisplayStyles { get; set; }

        public DisplayStyle SelectedStyle
        {
            get { return _selectedStyle; }
            set
            {
                if (_selectedStyle != value)
                {
                    _selectedStyle = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TestText
        {
            get { return _testText; }
            set
            {
                if (_testText != value)
                {
                    _testText = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SaveCommand { get; set; }

        public MainViewModel()
        {
            TrainListVM = new TrainListViewModel();
            ColorVM = new ColorViewModel();

            // Initialize the collection and commands
            DisplayStyles = new ObservableCollection<DisplayStyle>();
            //SaveCommand = new RelayCommand(Save);
        }

        private void Save()
        {
            // Implement save logic here
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
