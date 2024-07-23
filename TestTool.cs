using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace IpisCentralDisplayController
{
    static class SessionVars
    {
        private static TcpClient _tcpClient = null;
        public static TcpClient tcpClient
        {
            get { return _tcpClient; }
            set { _tcpClient = value; }
        }

        private static Int32 _tcpPort = 5000;
        public static Int32 tcpPort
        {
            get { return _tcpPort; }
            set { _tcpPort = value; }
        }

        private static string _ipAddr = "";
        public static string ipAddr
        {
            get { return _ipAddr; }
            set { _ipAddr = value; }
        }

        private static NetworkStream _nStream = null;
        public static NetworkStream nStream
        {
            get { return _nStream; }
            set { _nStream = value; }
        }

        private static bool _sConnected = false;
        public static bool sConnected
        {
            get { return _sConnected; }
            set { _sConnected = value; }
        }
    }

    public enum DispType_t { DISP_SLIDESHOW, DISP_STEADY };
    public enum brghtControl_t { BRIGHT_AUTO, BRIGHT_MANUAL };

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DispDataIPIS_t // command: 0x0C
    {
        public UInt32 header;
        public UInt32 disp_width;
        public UInt32 disp_height;
        public UInt32 disp_type;
        // 0: SLDB/MLDB,
        // 1: CGDB,
        // 2: AGDB

        public UInt32 transition;
        //  None
        // 0: Curtain Left - Right
        // 1: Curtain Right - Left
        // 2: Curtain Top - Bottom
        // 3: Curtain Bottom - Top
        // 4: Running Left - Right
        // 5: Running Right - Left
        // 6: Running Top - Bottom
        // 7: Running Bottom - Top
        public UInt32 transition_speed;

        public UInt32 effect;
        // 0: Steady
        // 1: Scroll
        // 2: Flashing

        public UInt32 effect_speed;

        public UInt32 brghtLevel;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
        public byte[] dp_buf;
    };

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DispDataIPISs_t // commandL 0x0D
    {
        public UInt32 header;
        public UInt32 disp_width;
        public UInt32 disp_height;
        public UInt32 disp_type;
        // 0: SLDB/MLDB,
        // 1: CGDB,
        // 2: AGDB

        public UInt32 transition;
        // 0: None
        // 1: Curtain Left - Right
        // 2: Curtain Right - Left
        // 3: Curtain Top - Bottom
        // 4: Curtain Bottom - Top
        // 5: Running Left - Right
        // 6: Running Right - Left
        // 7: Running Top - Bottom
        // 8: Running Bottom - Top
        public UInt32 transition_speed;

        public UInt32 effect;
        // 0: Steady
        // 1: Scroll
        // 2: Flashing

        public UInt32 effect_speed;

        public UInt32 brghtLevel;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19200)]
        public byte[] dp_buf;
    };

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct DispDataIPISrgb_t // commandL 0x0C
    {
        public UInt32 header;
        public UInt32 disp_width;
        public UInt32 disp_height;
        public UInt32 disp_type;
        // 0: SLDB/MLDB,
        // 1: CGDB,
        // 2: AGDB

        public UInt32 transition;
        // 0: None
        // 1: Curtain Left - Right
        // 2: Curtain Right - Left
        // 3: Curtain Top - Bottom
        // 4: Curtain Bottom - Top
        // 5: Running Left - Right
        // 6: Running Right - Left
        // 7: Running Top - Bottom
        // 8: Running Bottom - Top
        public UInt32 transition_speed;

        public UInt32 effect;
        // 0: Steady
        // 1: Scroll
        // 2: Flashing

        public UInt32 effect_speed;

        public UInt32 brghtLevel;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6912)]
        public UInt64[] dp_buf;
    };

    public enum ip_mode_e { IP_STATIC, IP_DHCP };
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct config_t
    {
        public UInt32 header;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] deviceId;

        //Ethernet
        public ip_mode_e ip_mode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] staticIpAddr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] netMask;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] gateway;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] dns;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] mac_address;

        //TCP Server
        public UInt32 port;
    };

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct CmdPacket_t
    {
        public UInt32 header;
        public UInt32 size;
        public UInt32 cmd;
        // command codes
        // send static image:                   0x0C   
        // send scroll image:                   0x0D                          
        // start displaying image:              0x1C
        // stop displaying image:               0x2C
        // reset card:                          0x19
        // go to OTA mode:                      0x0F

        public UInt32 info;
        public UInt32 crc;
    };

    public class ColorViewModel : INotifyPropertyChanged
    {
        private Brush _textColor1;
        private Brush _backgroundColor1;
        private Brush _textColor2;
        private Brush _backgroundColor2;
        private Brush _textColor3;
        private Brush _backgroundColor3;
        private Brush _textColor4;
        private Brush _backgroundColor4;
        private Brush _textColor5;
        private Brush _backgroundColor5;

        public ColorViewModel()
        {
            // Set default values
            TextColor1 = new SolidColorBrush(Colors.LimeGreen);
            BackgroundColor1 = new SolidColorBrush(Colors.Black);
            TextColor2 = new SolidColorBrush(Colors.LimeGreen);
            BackgroundColor2 = new SolidColorBrush(Colors.Black);
            TextColor3 = new SolidColorBrush(Colors.LimeGreen);
            BackgroundColor3 = new SolidColorBrush(Colors.Black);
            TextColor4 = new SolidColorBrush(Colors.LimeGreen);
            BackgroundColor4 = new SolidColorBrush(Colors.Black);
            TextColor5 = new SolidColorBrush(Colors.LimeGreen);
            BackgroundColor5 = new SolidColorBrush(Colors.Black);
        }

        public Brush TextColor1
        {
            get => _textColor1;
            set { _textColor1 = value; OnPropertyChanged(nameof(TextColor1)); }
        }

        public Brush BackgroundColor1
        {
            get => _backgroundColor1;
            set { _backgroundColor1 = value; OnPropertyChanged(nameof(BackgroundColor1)); }
        }

        public Brush TextColor2
        {
            get => _textColor2;
            set { _textColor2 = value; OnPropertyChanged(nameof(TextColor2)); }
        }

        public Brush BackgroundColor2
        {
            get => _backgroundColor2;
            set { _backgroundColor2 = value; OnPropertyChanged(nameof(BackgroundColor2)); }
        }

        public Brush TextColor3
        {
            get => _textColor3;
            set { _textColor3 = value; OnPropertyChanged(nameof(TextColor3)); }
        }

        public Brush BackgroundColor3
        {
            get => _backgroundColor3;
            set { _backgroundColor3 = value; OnPropertyChanged(nameof(BackgroundColor3)); }
        }

        public Brush TextColor4
        {
            get => _textColor4;
            set { _textColor4 = value; OnPropertyChanged(nameof(TextColor4)); }
        }

        public Brush BackgroundColor4
        {
            get => _backgroundColor4;
            set { _backgroundColor4 = value; OnPropertyChanged(nameof(BackgroundColor4)); }
        }

        public Brush TextColor5
        {
            get => _textColor5;
            set { _textColor5 = value; OnPropertyChanged(nameof(TextColor5)); }
        }

        public Brush BackgroundColor5
        {
            get => _backgroundColor5;
            set { _backgroundColor5 = value; OnPropertyChanged(nameof(BackgroundColor5)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
