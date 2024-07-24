using System;
using System.Text.RegularExpressions;
using System.Windows;
using IpisCentralDisplayController.models;

namespace IpisCentralDisplayController.views
{
    public partial class PlatformDialog : Window
    {
        public string PlatformNumber { get; private set; }
        public PlatformType PlatformType { get; private set; }
        public string Description { get; private set; }
        public string Subnet { get; private set; }

        public PlatformDialog()
        {
            InitializeComponent();
        }

        public PlatformDialog(Platform platform) : this()
        {
            PlatformNumberTextBox.Text = platform.PlatformNumber;
            PlatformTypeComboBox.SelectedItem = platform.PlatformType;
            DescriptionTextBox.Text = platform.Description;
            SubnetTextBox.Text = platform.Subnet;
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
