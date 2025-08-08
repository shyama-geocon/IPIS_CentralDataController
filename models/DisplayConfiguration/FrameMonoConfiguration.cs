using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpisCentralDisplayController.models.DisplayConfiguration
{
    public class FrameMonoConfiguration
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
        public byte DataTimeout { get; set; } // min


        #endregion

        public byte Level1EndOfDataPacket { get; set; } // always 0x03

        public byte CRC_MSB { get; set; }
        public byte CRC_LSB { get; set; }

        public byte EOT { get; set; }

        #endregion

    }
}
