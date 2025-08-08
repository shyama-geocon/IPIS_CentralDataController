using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using IpisCentralDisplayController.models;

namespace IpisCentralDisplayController.views
{
    public partial class PlatformDialog : Window, INotifyPropertyChanged
    {
        //   public string PlatformNumber { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        private string _platformNumber;    
        public string PlatformNumber
        {
            get { return _platformNumber; }
             set { 
                _platformNumber = value;
                OnPropertyChanged();
            }
        }

        private PlatformType _platformType;
        public PlatformType PlatformType
        {
            get { return _platformType; }
            set { 
                _platformType = value;
                OnPropertyChanged();
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set {
                _description = value;
                OnPropertyChanged();
            }
        }


        private string _subnet;
        public string Subnet
        {
            get { return _subnet; }
            set {
                _subnet = value;
                OnPropertyChanged();
            }
        }


        public PlatformDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        public PlatformDialog(Platform platform) : this()
        {
            PlatformNumberTextBox.Text = platform.PlatformNumber;
            PlatformTypeComboBox.SelectedItem = platform.PlatformType;
            DescriptionTextBox.Text = platform.Description;
            SubnetTextBox.Text = platform.Subnet;

            PlatformNumber = platform.PlatformNumber;
            PlatformType = platform.PlatformType;
            Description = platform.Description;
            Subnet = platform.Subnet;

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            PlatformNumber = PlatformNumberTextBox.Text;
            PlatformType = (PlatformType)PlatformTypeComboBox.SelectedItem;
            Description = DescriptionTextBox.Text;
            Subnet = SubnetTextBox.Text;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void PlatformNumberTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string platformNumber = PlatformNumberTextBox.Text;
            SubnetTextBox.Text = EstimateSubnet(platformNumber);
        }

        private string EstimateSubnet(string platformNumber)
        {
            if (int.TryParse(platformNumber, out int numericPlatform))
            {
                return $"192.168.{numericPlatform}";
            }
            else
            {
                var match = Regex.Match(platformNumber, @"^(\d+)[A]$");
                if (match.Success && int.TryParse(match.Groups[1].Value, out int specialPlatform))
                {
                    return $"192.168.{100 + specialPlatform}";
                }
            }

            return string.Empty;
        }
    }
}
