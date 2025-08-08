using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IpisCentralDisplayController.models.DisplayCommunication;

namespace IpisCentralDisplayController.models.DisplayConfiguration
{

    public class StatusColourConfig
    {
        public byte StatusByte { get; set; }

        public byte R_TrainNo { get; set; }
        public byte G_TrainNo { get; set; }
        public byte B_TrainNo { get; set; }

        public byte R_TrainName { get; set; }
        public byte G_TrainName { get; set; }
        public byte B_TrainName { get; set; }

        public byte R_TrainTime { get; set; }
        public byte G_TrainTime { get; set; }
        public byte B_TrainTime { get; set; }

        public byte R_TrainAD { get; set; } // A/D for Arrival/Departure
        public byte G_TrainAD { get; set; }
        public byte B_TrainAD { get; set; } // A/D for Arrival/Departure

        public byte R_TrainPF { get; set; } // Platform number
        public byte G_TrainPF { get; set; }
        public byte B_TrainPF { get; set; } // Platform number

    }


    public class FrameColourConfiguration
    {

        #region Level1
        public byte Start1 { get; set; }
        public byte Start2 { get; set; }
        public byte PacketIdentifier3 { get; set; }
        public byte PacketLengthMSB4 { get; set; }
        public byte PacketLengthLSB5 { get; set; }
        public byte DestinationAddressThird6 { get; set; }
        public byte DestinationAddressFourth7 { get; set; }
        public byte SourceAddressThird8 { get; set; }
        public byte SourceAddressFourth9 { get; set; }
        public byte SerialNumber10 { get; set; }
        public byte PacketType11 { get; set; }
        public byte StartOfDataPacketIndicator12 { get; set; }

        #region Level2


        public byte IntensityLevel { get; set; }
        public byte DataTimeout  { get; set; }

        public byte R_HorizontalLine { get; set; }
        public byte G_HorizontalLine { get; set; }
        public byte B_HorizontalLine { get; set; }

        public byte R_VerticalLine { get; set; }
        public byte G_VerticalLine { get; set; }
        public byte B_VerticalLine { get; set; }

        public byte R_Background { get; set; }
        public byte G_Background { get; set; }
        public byte B_Background { get; set; }


        public byte R_MessageLine { get; set; }
        public byte G_MessageLine { get; set; }
        public byte B_MessageLine { get; set; }


        public List<StatusColourConfig> StatusColourConfigs { get; set; } = new List<StatusColourConfig>();


        #endregion

        public byte Level1EndOfDataPacket { get; set; } // always 0x03

        public byte CRC_MSB { get; set; }
        public byte CRC_LSB { get; set; }

        public byte EOT { get; set; }

        #endregion



    }
}
