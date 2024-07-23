using System.Collections.Generic;

namespace IpisCentralDisplayController.models
{
    public class Platform
    {
        public string PlatformNumber { get; set; }
        public PlatformType PlatformType { get; set; }
        public string Description { get; set; }
        public string Subnet { get; set; }
        public List<Device> Devices { get; set; } = new List<Device>();
    }
}
