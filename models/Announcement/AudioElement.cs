using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpisCentralDisplayController.models.Announcement
{
    public class AudioElement
    {
        public string FilePath { get; set; }  // null if it's a pause
        public int? PauseDurationMs { get; set; } // null if it's an audio file

        public static AudioElement FromFile(string path) => new AudioElement { FilePath = path };
        public static AudioElement Pause(int durationMs) => new AudioElement { PauseDurationMs = durationMs };
    }

}
