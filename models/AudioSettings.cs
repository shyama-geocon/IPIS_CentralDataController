using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace IpisCentralDisplayController.models
{
    public class AudioSettings
    {
        public string MicInterface { get; set; }
        public int MicVolume { get; set; }
        public string MonitorInterface { get; set; }
        public int MonitorVolume { get; set; }
        public List<double> MonitorEqualizer { get; set; }
        public string AudioOutInterface { get; set; }
        public int AudioOutVolume { get; set; }
        public List<double> AudioOutEqualizer { get; set; }
    }
}

