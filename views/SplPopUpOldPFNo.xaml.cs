using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using IpisCentralDisplayController.Helpers;
using IpisCentralDisplayController.Managers;
using IpisCentralDisplayController.models;

namespace IpisCentralDisplayController.views
{
    /// <summary>
    /// Interaction logic for SplPopUpOldPFNo.xaml
    /// </summary>
    ///     
    public partial class SplPopUpOldPFNo : Window
    {
        public ActiveTrain Train { get; set; }
        //public List<PlatformItem> PlatformOptions { get; set; }

        //public PlatformItem SelectedOldPF { get; set; }
        //public PlatformItem SelectedNewPF { get; set; }

        public List<int> PlatformOptions { get; set; }

        //public int SelectedOldPF { get; set; }
        //public int SelectedNewPF { get; set; }



        private PlatformDeviceManager _platformDeviceManager;

        public SplPopUpOldPFNo(ActiveTrain train)
        {
            InitializeComponent();
            DataContext = this;

            Train = train;

            var jsonHelperAdapter = new SettingsJsonHelperAdapter();
            _platformDeviceManager = new PlatformDeviceManager(jsonHelperAdapter);
            var platforms = _platformDeviceManager.LoadPlatforms();

            PlatformOptions = new List<int>();
           
            foreach (var platform in platforms)
            {
                //var holder = new PlatformItem();
                PlatformOptions.Add(int.Parse(platform.PlatformNumber));
            }

        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (Train.OldPFNo != null && Train.PFNo.ToString() != null)
            {
                DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select a PF No..", "No Platform Number Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        //public class PlatformItem
        //{
        //    public int PlatformInt { get; set; }
        //    public string PlatformString { get; set; }

        //    public PlatformItem()
        //    {

        //    }
        //}


    }
}
