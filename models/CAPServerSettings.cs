using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpisCentralDisplayController.models
{
    public class CAPServerSettings
    {
        public bool IsEnabled { get; set; }

        public DisseminationMode DisseminationMode { get; set; }

        public string ApiUrl { get; set; }

        public string StationCode { get; set; }
        public string ApiKey { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public int PollingInterval { get; set; }

        public int AlertDisplayTime { get; set; }

        public int AlertRepetitionTime { get; set; }
    }

    public enum DisseminationMode
    {
        Auto,
        Manual
    }
}
