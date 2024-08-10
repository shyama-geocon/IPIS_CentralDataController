using System.Windows;

namespace IpisCentralDisplayController.views
{
    public partial class InputDialog : Window
    {
        public string ResponseText { get; private set; }
        public string Prompt { get; set; }

        public InputDialog(string prompt)
        {
            InitializeComponent();
            DataContext = this;
            Prompt = prompt;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ResponseText = InputTextBox.Text;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
