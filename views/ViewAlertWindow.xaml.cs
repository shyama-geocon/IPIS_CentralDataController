using IpisCentralDisplayController.models;
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

namespace IpisCentralDisplayController.views
{
    /// <summary>
    /// Interaction logic for ViewAlertWindow.xaml
    /// </summary>
    public partial class ViewAlertWindow : Window
    {
        public ViewAlertWindow()
        {
            InitializeComponent();
        }

        public void SetAlertDetails(CAPAlert alert)
        {
            IdentifierText.Text = alert.Identifier;
            SenderText.Text = alert.Sender;
            SentText.Text = alert.Sent.ToString();
            StatusText.Text = alert.Status;
            MsgTypeText.Text = alert.MsgType;
            SourceText.Text = alert.Source;
            ScopeText.Text = alert.Scope;
            CategoryText.Text = alert.Info.Category;
            EventText.Text = alert.Info.Event;
            UrgencyText.Text = alert.Info.Urgency;
            SeverityText.Text = alert.Info.Severity;
            CertaintyText.Text = alert.Info.Certainty;
            DescriptionText.Text = alert.Info.Description;
            EffectiveText.Text = alert.Info.Effective.ToString();
            ExpiresText.Text = alert.Info.Expires.ToString();
            AreaDescText.Text = alert.Info.Area[0].AreaDesc; // Assuming the first area
        }

        private void SendAlertButton_Click(object sender, RoutedEventArgs e)
        {
            // Implement the logic to send the alert
        }

        private void PauseResumeAlertButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int duration = PauseResumeDuration.Value ?? 600; // Default to 600 if null

            if (button.Content.ToString() == "Pause Alert")
            {
                // Implement the logic to pause the alert with the specified duration
                button.Content = "Resume Alert";
            }
            else
            {
                // Implement the logic to resume the alert with the specified duration
                button.Content = "Pause Alert";
            }
        }
    }
}
