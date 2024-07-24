//using System.Windows;
//using System.Windows.Media;

//namespace IpisCentralDisplayController.models
//{
//    public class DisplayStyle
//    {
//        public int Sno { get; set; }
//        public string StyleName { get; set; }
//        public RegionalLanguage Language { get; set; }
//        public FontFamily Font { get; set; }
//        public double FontSize { get; set; }
//        public FontWeight FontWeight { get; set; }
//        public FontStyle FontStyle { get; set; }
//        public double MarginTop { get; set; }
//        public double MarginLeft { get; set; }
//        public HorizontalAlignment AlignmentH { get; set; }
//        public VerticalAlignment AlignmentV { get; set; }

//        public string TestString { get; set; }
//    }
//}
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace IpisCentralDisplayController.models
{
    public class DisplayStyle : INotifyPropertyChanged
    {
        private int _sno;
        private string _styleName;
        private RegionalLanguage _language;
        private FontFamily _font;
        private double _fontSize;
        private int _fontWeight;
        private int _fontStyle;
        private double _marginTop;
        private double _marginLeft;
        private int _alignmentH;
        private int _alignmentV;
        private string _testString;

        public int Sno
        {
            get => _sno;
            set
            {
                if (_sno != value)
                {
                    _sno = value;
                    OnPropertyChanged();
                }
            }
        }

        public string StyleName
        {
            get => _styleName;
            set
            {
                if (_styleName != value)
                {
                    _styleName = value;
                    OnPropertyChanged();
                }
            }
        }

        public RegionalLanguage Language
        {
            get => _language;
            set
            {
                if (_language != value)
                {
                    _language = value;
                    OnPropertyChanged();
                    TestString = GetTestStringForLanguage(_language);
                }
            }
        }

        public FontFamily Font
        {
            get => _font;
            set
            {
                if (_font != value)
                {
                    _font = value;
                    OnPropertyChanged();
                }
            }
        }

        public double FontSize
        {
            get => _fontSize;
            set
            {
                if (_fontSize != value)
                {
                    _fontSize = value;
                    OnPropertyChanged();
                }
            }
        }

        public int FontWeight
        {
            get => _fontWeight;
            set
            {
                if (_fontWeight != value)
                {
                    _fontWeight = value;
                    OnPropertyChanged();
                }
            }
        }

        public int FontStyle
        {
            get => _fontStyle;
            set
            {
                if (_fontStyle != value)
                {
                    _fontStyle = value;
                    OnPropertyChanged();
                }
            }
        }

        public double MarginTop
        {
            get => _marginTop;
            set
            {
                if (_marginTop != value)
                {
                    _marginTop = value;
                    OnPropertyChanged();
                }
            }
        }

        public double MarginLeft
        {
            get => _marginLeft;
            set
            {
                if (_marginLeft != value)
                {
                    _marginLeft = value;
                    OnPropertyChanged();
                }
            }
        }

        public int AlignmentH
        {
            get => _alignmentH;
            set
            {
                if (_alignmentH != value)
                {
                    _alignmentH = value;
                    OnPropertyChanged();
                }
            }
        }

        public int AlignmentV
        {
            get => _alignmentV;
            set
            {
                if (_alignmentV != value)
                {
                    _alignmentV = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TestString
        {
            get => _testString;
            set
            {
                if (_testString != value)
                {
                    _testString = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string GetTestStringForLanguage(RegionalLanguage language)
        {
            return language switch
            {
                RegionalLanguage.ENGLISH => "Indian Railways",
                RegionalLanguage.HINDI => "भारतीय रेल",
                RegionalLanguage.ASSAMESE => "ভাৰতীয় ৰেলৱে",
                RegionalLanguage.BANGLA => "ভারতীয় রেলওয়ে",
                RegionalLanguage.BODO => "--",
                RegionalLanguage.DOGRI => "भारतीय रेलवे",
                RegionalLanguage.GUJARATI => "ભારતીય રેલ્વે",
                RegionalLanguage.KANNADA => "ಭಾರತೀಯ ರೈಲ್ವೆ",
                RegionalLanguage.KASHMIRI => "ہندوستانی ریلوے",
                RegionalLanguage.KONKANI => "भारतीय रेल्वे",
                RegionalLanguage.MALAYALAM => "ഇന്ത്യൻ റെയിൽവേ",
                RegionalLanguage.MARATHI => "भारतीय रेल्वे",
                RegionalLanguage.MANIPURI => "ꯏꯟꯗꯤꯌꯟ ꯔꯦꯂꯋꯦꯖꯒꯤ ꯗꯥ",
                RegionalLanguage.NEPALI => "भारतीय रेलवे",
                RegionalLanguage.ODIA => "ଭାରତୀୟ ରେଳ",
                RegionalLanguage.PUNJABI => "ਭਾਰਤੀ ਰੇਲਵੇ",
                RegionalLanguage.SANSKRIT => "भारतीय रेलवे",
                RegionalLanguage.SANTHALI => "Bharot disom reak̕ rel gạḍi",
                RegionalLanguage.SINDHI => "هندستاني ريلوي",
                RegionalLanguage.TAMIL => "இந்திய ரயில்வே",
                RegionalLanguage.TELUGU => "భారతీయ రైల్వేలు",
                RegionalLanguage.URDU => "ہندوستانی ریلوے",
                _ => "Unknown"
            };
        }
    }
}
