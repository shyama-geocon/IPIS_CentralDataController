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
        public char ArrivalOrDeparture { get; set; }
        public string PlatformNumber { get; set; }
        public byte StatusByte { get; set; }
        public string SpecialStatusMsg { get; set; }
        public string StationNameForSplStatus { get; set; } // FOR PAGE 2: Diverted/Terminated at/Change of Source


        public byte[] TrainNumberBytes { get; set; } // length of array is fixed
        public byte[] TrainNameBytes { get; set; }// length of array is NOT fixed
                                                  // will need to find it out dynamically 
        public byte[] TimeBytes { get; set; } // length of array is fixed
        public byte[] ArrivalOrDepartureBytes { get; set; } // length of array is fixed
        public byte[] PlatformNumberBytes { get; set; } // length of array is fixed       
        public byte[] Nplus5toKfIELD { get; set; }
        public byte[] StationNameBytes { get; set; } // FOR PAGE 2: Diverted/Terminated at/Change of Source







        public TrainToBeDisplayed()
        {
            


        }



    }
}
