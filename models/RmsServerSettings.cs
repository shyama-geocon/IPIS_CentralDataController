using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpisCentralDisplayController.models
{
    public class RmsServerSettings
    {
        public bool IsEnabled { get; set; }             // Enable/Disable RMS Server
        public string ApiUrl { get; set; }              // API URL of the server
        public bool IsCapSyncEnabled { get; set; }      // Enable CAP Sync
        public bool IsEventLogsEnabled { get; set; }    // Enable Event Logs
        public bool IsPlatformDevicesStatusEnabled { get; set; } // Enable Platforms-Devices Status
                                                                 // This should also take care of sending any update platform/device info

        public bool IsStationInfoEnabled { get; set; }  // Enable Station Info
        public bool IsActiveTrainsInfoEnabled { get; set; }  // Enable ActiveTrains Info
    }
}
