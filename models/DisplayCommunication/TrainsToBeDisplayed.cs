using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpisCentralDisplayController.models.DisplayCommunication
{
    public class TrainToBeDisplayed
    {
        public string TrainNumber { get; set; }
        public string TrainName { get; set; }
        public string Time { get; set; }
        public string ArrivalOrDepture { get; set; }
        public string PlatformNumber { get; set; }
        public byte StatusByte { get; set; }
        public byte[] SPLStatusMessage_Nplu5_K { get; set; }
        public string StationNameForSplStatus { get; set; } // FOR PAGE 2: Diverted/Terminated at/Change of Source

        public TrainToBeDisplayed()
        {
                
        }



    }
}
