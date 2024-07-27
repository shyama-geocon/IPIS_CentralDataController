using System.Windows.Media;

namespace IpisCentralDisplayController.models
{
    public class TrainDisplayTemplate
    {
        public Color TrainNoColor { get; set; }
        public Color TrainNameColor { get; set; }
        public Color TrainTimeColor { get; set; }
        public Color TrainADColor { get; set; } // A/D for Arrival/Departure
        public Color TrainPFColor { get; set; } // Platform number

        public string StatusType { get; set; } // Indicates "Arrival" or "Departure"
        public string StatusDescription { get; set; } // User-friendly status description

        public bool IsBlinking { get; set; } = false;
    }
}
