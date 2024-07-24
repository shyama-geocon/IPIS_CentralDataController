using System;
using System.Windows;
using IpisCentralDisplayController.models;

namespace IpisCentralDisplayController.views
{
    public partial class DeviceDialog : Window
    {
        public DeviceType DeviceType { get; private set; }
        public string IpAddress { get; private set; }
        public bool IsEnabled { get; private set; }
        public string Description { get; private set; }

        public DeviceDialog(string platformNumber, string subnet)
        {
            InitializeComponent();
            PlatformNumberTextBox.Text = platformNumber;
            IpAddressTextBox.Text = $"{subnet}"; // Pre-fill with subnet
        }

        public DeviceDialog(Device device, string platformNumber, string subnet) : this(platformNumber, subnet)
        {
            DeviceTypeComboBox.SelectedItem = device.DeviceType;
            IpAddressTextBox.Text = device.IpAddress;
            EnabledCheckBox.IsChecked = device.IsEnabled;
            DescriptionTextBox.Text = device.Description;
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
