using System.Windows;
using System.Windows.Media;

namespace IpisCentralDisplayController.models
{
    public class DisplayStyle
    {
        public int Sno { get; set; }
        public string StyleName { get; set; }
        public RegionalLanguage Language { get; set; }
        public FontFamily Font { get; set; }
        public double FontSize { get; set; }
        public FontWeight FontWeight { get; set; }
        public FontStyle FontStyle { get; set; }
        public double MarginTop { get; set; }
        public double MarginLeft { get; set; }
        public HorizontalAlignment AlignmentH { get; set; }
        public VerticalAlignment AlignmentV { get; set; }
    }
}
