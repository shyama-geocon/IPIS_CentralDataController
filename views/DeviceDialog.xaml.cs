using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Windows;
using IpisCentralDisplayController.models;

namespace IpisCentralDisplayController.views
{
    public partial class DeviceDialog : Window, INotifyPropertyChanged
    {
        public DeviceType DeviceType { get; private set; }
        public string IpAddress { get; private set; }
        public bool IsEnabled { get; private set; }
        public string Description { get; private set; }

        

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public ObservableCollection<LineOptionTemplate> LineOptions  { get; set; }

        private LineOptionTemplate _selectedLineOption;
        public LineOptionTemplate SelectedLineOption
        {
            get => _selectedLineOption;
            set
            {
                _selectedLineOption = value;
                OnPropertyChanged(); // Notify when LineOptions changes
            }
        }

        // LineOptionsIsEnabled

        private bool _lineOptionsIsEnabled =false;

        public bool LineOptionsIsEnabled
        {
            get { return _lineOptionsIsEnabled ; }
            set { 
                _lineOptionsIsEnabled = value;
                OnPropertyChanged(); // Notify when LineOptionsIsEnabled changes
            }
        }


        public DeviceDialog(string platformNumber, string subnet)
        {
            InitializeComponent();
            PlatformNumberTextBox.Text = platformNumber;
            IpAddressTextBox.Text = $"{subnet}"; // Pre-fill with subnet
            DataContext = this;
            LineOptions= new ObservableCollection<LineOptionTemplate>();
        }

        public DeviceDialog(Device device, string platformNumber, string subnet) : this(platformNumber, subnet)
        {
            DeviceTypeComboBox.SelectedItem = device.DeviceType;
            IpAddressTextBox.Text = device.IpAddress;
            EnabledCheckBox.IsChecked = device.IsEnabled;
            DescriptionTextBox.Text = device.Description;
            DataContext = this;
            LineOptions = new ObservableCollection<LineOptionTemplate>();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DeviceType = (DeviceType)DeviceTypeComboBox.SelectedItem;
            IpAddress = IpAddressTextBox.Text;
            IsEnabled = EnabledCheckBox.IsChecked ?? false;
            Description = DescriptionTextBox.Text;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void DeviceTypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (DeviceTypeComboBox.SelectedItem is DeviceType selectedDeviceType)
            {
                string ipRange = GetIpRangeForDeviceType(selectedDeviceType);
                MessageBox.Show($"Possible IP addresses for {selectedDeviceType} are: {ipRange}", "IP Address Information", MessageBoxButton.OK, MessageBoxImage.Information);
                DescriptionTextBox.Text = GetDefaultDescription(selectedDeviceType);
            }
            if( LineOptions != null)
            {
                LineOptions.Clear();
            }
            
            if (DeviceTypeComboBox.SelectedItem is DeviceType.CDS)
            {             
                LineOptions = null;
                SelectedLineOption = null;
                LineOptionsIsEnabled = false; // Disable LineOptions for CDS
            }
            else if (DeviceTypeComboBox.SelectedItem is DeviceType.PrimaryServer)
            {
               
                LineOptions = null;
                SelectedLineOption = null;
                LineOptionsIsEnabled = false; // Disable LineOptions for Primary Server
            }
            else if (DeviceTypeComboBox.SelectedItem is DeviceType.SecondaryServer)
            {
               
                LineOptions = null;
                SelectedLineOption = null;
                LineOptionsIsEnabled = false; // Disable LineOptions for Secondary Server
            }
            else if (DeviceTypeComboBox.SelectedItem is DeviceType.PDC)
            {
               
                LineOptions = null;
                SelectedLineOption = null;
                LineOptionsIsEnabled = false; // Disable LineOptions for PDC
            }
            else if (DeviceTypeComboBox.SelectedItem is DeviceType.CGDB)
            {
                LineOptions.Add(new LineOptionTemplate()
                {
                    Name = "1 Line",
                    Value = 1
                });
                LineOptionsIsEnabled = true; // Enable LineOptions for CGDB
                SelectedLineOption= LineOptions.FirstOrDefault();
            }
            else if (DeviceTypeComboBox.SelectedItem is DeviceType.AGDB)
            {
                LineOptions.Add(new LineOptionTemplate()
                {
                    Name = "2 Lines",
                    Value = 2
                });
                LineOptionsIsEnabled = true;
                SelectedLineOption = LineOptions.FirstOrDefault();
            }
            else if (DeviceTypeComboBox.SelectedItem is DeviceType.PFDB)
            {
                LineOptions.Add(new LineOptionTemplate()
                {
                    Name = "1 Line",
                    Value = 1
                });
                LineOptionsIsEnabled = true;
                SelectedLineOption = LineOptions.FirstOrDefault();
            }
            else if (DeviceTypeComboBox.SelectedItem is DeviceType.OVD)
            {
                LineOptions.Add(new LineOptionTemplate()
                {
                    Name = "6 Lines",
                    Value = 6
                });

                LineOptions.Add(new LineOptionTemplate()
                {
                    Name = "12 Lines",
                    Value = 12
                });

                LineOptions.Add(new LineOptionTemplate()
                {
                    Name = "18 Lines",
                    Value = 18
                });
                LineOptionsIsEnabled = true; // Enable LineOptions for OVD
                SelectedLineOption = LineOptions.FirstOrDefault();

            }
            else if (DeviceTypeComboBox.SelectedItem is DeviceType.IVD)
            {
                LineOptions.Add(new LineOptionTemplate()
                {
                    Name = "6 Lines",
                    Value = 6
                });

                LineOptions.Add(new LineOptionTemplate()
                {
                    Name = "12 Lines",
                    Value = 12
                });

                LineOptions.Add(new LineOptionTemplate()
                {
                    Name = "18 Lines",
                    Value = 18
                });
                LineOptionsIsEnabled = true;
                SelectedLineOption = LineOptions.FirstOrDefault();

            }
            else if (DeviceTypeComboBox.SelectedItem is DeviceType.SLDB)
            {
                LineOptions.Add(new LineOptionTemplate()
                {
                    Name = "1 Line",
                    Value = 1
                });
                LineOptionsIsEnabled = true;
                SelectedLineOption = LineOptions.FirstOrDefault();
            }
            else if (DeviceTypeComboBox.SelectedItem is DeviceType.MLDB)
            {
                LineOptions.Add(new LineOptionTemplate()
                {
                    Name = "2 Lines",
                    Value = 2
                });
                LineOptions.Add(new LineOptionTemplate()
                {
                    Name = "3 Lines",
                    Value = 3
                });
                LineOptions.Add(new LineOptionTemplate()
                {
                    Name = "4 Lines",
                    Value = 4
                });
                LineOptions.Add(new LineOptionTemplate()
                {
                    Name = "5 Lines",
                    Value = 5
                });
                LineOptions.Add(new LineOptionTemplate()
                {
                    Name = "6 Lines",
                    Value = 6
                });
                LineOptions.Add(new LineOptionTemplate()
                {
                    Name = "7 Lines",
                    Value = 7
                });
                LineOptions.Add(new LineOptionTemplate()
                {
                    Name = "8 Lines",
                    Value = 8
                });
                LineOptions.Add(new LineOptionTemplate()
                {
                    Name = "9 Lines",
                    Value = 9
                });
                LineOptions.Add(new LineOptionTemplate()
                {
                    Name = "10 Lines",
                    Value = 10
                });
                LineOptionsIsEnabled = true; // Enable LineOptions for MLDB
                SelectedLineOption = LineOptions.FirstOrDefault();

            }
            else if (DeviceTypeComboBox.SelectedItem is DeviceType.LED_TV)
            {
                LineOptions = null;
                SelectedLineOption = null;
                LineOptionsIsEnabled = false; // Disable LineOptions for LED_TV              
            }
            else if (DeviceTypeComboBox.SelectedItem is DeviceType.NW_SW)
            {

                LineOptions = null;
                SelectedLineOption = null;
                LineOptionsIsEnabled = false; // Disable LineOptions for NW_SW
               
            }



        }

        private string GetIpRangeForDeviceType(DeviceType deviceType)
        {
            return deviceType switch
            {
                DeviceType.CDS => "252",
                DeviceType.PrimaryServer => "253",
                DeviceType.SecondaryServer => "254",
                DeviceType.PDC => "252",
                DeviceType.CGDB => "2-39",
                DeviceType.AGDB => "131-160",
                DeviceType.PFDB => "161-190",
                DeviceType.OVD => "40-70",
                DeviceType.IVD => "71-100",
                DeviceType.SLDB => "101-130",
                DeviceType.MLDB => "101-130",
                DeviceType.LED_TV => "191-220",
                DeviceType.NW_SW => "221-250",
                _ => "Unknown"
            };
        }

        private string GetDefaultDescription(DeviceType deviceType)
        {
            return deviceType switch
            {
                DeviceType.CDS => "Central Data Switch / Platform Gateway",
                DeviceType.PrimaryServer => "CDC Main Server",
                DeviceType.SecondaryServer => "CDC Backup Server",
                DeviceType.PDC => "Platform Display Controller / Gateway",
                DeviceType.CGDB => "Coach Guidance Display Board",
                DeviceType.AGDB => "At-a-Glance Display Board",
                DeviceType.PFDB => "Platform Display Board",
                DeviceType.OVD => "Outdoor Video Display Board",
                DeviceType.IVD => "Indoor Video Display Board",
                DeviceType.SLDB => "Single Line Display Board",
                DeviceType.MLDB => "Multi Line Display Board",
                DeviceType.LED_TV => "LED TV / Display Monitor",
                DeviceType.NW_SW => "Network Switch",
                _ => "Unknown Device"
            };
        }
    }
}
