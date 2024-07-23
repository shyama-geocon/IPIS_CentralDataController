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

namespace IpisCentralDisplayController.custom
{
    public partial class ProgressDialog : Window
    {
        public ProgressDialog(string message)
        {
            InitializeComponent();
            StatusText.Text = message;
        }

        public void UpdateProgress(double progress)
        {
            ProgressBar.Value = progress;
        }
    }
}
