using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IpisCentralDisplayController.models.DisplayCommunication
{
    public class FrameForCGDB
    {

        #region ACTUAL FRAME BYTES
        //Note: Contains nested level regions to reflect the actual 
        //frame structure

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

        public byte WindowLeftColumn1 { get; set; } 
        public byte WindowRightColumn2 { get; set; } 
        public byte WindowTopRow3 { get; set; } 
        public byte WindowBottomRow4 { get; set; }


        public ByteBuilder Level2Byte5 { get; set; }
        public ByteBuilder Level2Byte6 { get; set; }
        public ByteBuilder Level2Byte7 { get; set; }
        public byte TimeDelay8 { get; set; }

        public byte[] TrainNumberBytes { get; set; } // length of array is fixed : 10 bytes

        public byte[] EnglishCoachBytes { get; set; } // length of array is fixed : 10 bytes

        public byte SeparatorByte1 { get; set; } // Separator byte : always 0xE7
        public byte SeparatorByte2 { get; set; } // Separator byte : always 0x00
        public byte[] HindiCoachBytes { get; set; } // length of array is fixed : 16 bytes

        public byte EndOfCoachData { get; set; } // Always 0xEC

        #endregion

        public byte Level1EndOfDataPacket { get; set; } // always 0x03
        public byte CRC_MSB { get; set; }
        public byte CRC_LSB { get; set; }
        public byte EOT { get; set; }

        #endregion



        #endregion


        #region Direct Read Fields

        #region ForLevel1
        public IPAddress DestinationIPAddress { get; set; }
        public IPAddress SourceIPAddress { get; set; }

        #endregion

        #region ForLevel2
        //These fields are used to set the bit values in the
        //Level2Byte9, Level2Byte10, Level2Byte11, and Level2Byte12,
        public bool ReverseVideo { get; set; }
        public int Speed { get; set; }
        public int EffectCode { get; set; }
        public int LetterSize { get; set; }
        public int Gap { get; set; }
        public int TimeDelay { get; set; }
        public string TrainNumber { get; set; }
        public string EngCoachName { get; set; }
        public string HindiCoachName { get; set; }
        #endregion

        #endregion


    }
}
