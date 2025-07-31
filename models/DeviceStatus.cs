using System;

namespace IpisCentralDisplayController.models
{
    public class DeviceStatus
    {
        public string PlatformNumber { get; set; }
        public int DeviceId { get; set; }
        public DeviceType DeviceType { get; set; }
        public string Description { get; set; }
        public string IpAddress { get; set; }
        public bool Status { get; set; }
        public DateTime LastStatusWhen { get; set; }
        public bool IsEnabled { get; set; }
        public string StatusDisplay => Status ? "OK" : "ERR";
        public string LastStatusFormatted => DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
    }
}
