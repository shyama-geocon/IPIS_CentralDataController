using System;

namespace IpisCentralDisplayController.models
{
    public class Device
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public DeviceType DeviceType { get; set; }
        public string Description { get; set; }
        public string IpAddress { get; set; }
        public bool Status { get; set; }
        public DateTime LastStatusWhen { get; set; }


        public byte SpeedByte { get; set; }
        public byte EffectByte { get; set; }
        public byte LetterSizeByte { get; set; }
        public byte IntensityByte { get; set; }
        public byte TimeDelayValueByte { get; set; }
        public byte DataTimeoutValueByte { get; set; }
        public bool IsReverseVideo { get; set; }
        public bool IsEnabled { get; set; }

    }
}
