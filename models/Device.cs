using System;

namespace IpisCentralDisplayController.models
{
    public class Device
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public DeviceType DeviceType { get; set; }
        public string IpAddress { get; set; }
        public bool Status { get; set; }
        public DateTime LastStatusWhen { get; set; }
        public bool IsEnabled { get; set; }
    }
}
