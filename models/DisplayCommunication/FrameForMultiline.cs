using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IpisCentralDisplayController.models.DisplayCommunication
{
    public class FrameForMultiline
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

        public byte WindowLeftColumn1 { get; set; } //MSB
        public byte WindowLeftColumn2 { get; set; } //LSB

        public byte WindowRightColumn3 { get; set; } //MSB
        public byte WindowRightColumn4 { get; set; } //LSB

        public byte WindowTopRow5 { get; set; } //MSB
        public byte WindowTopRow6 { get; set; } //LSB

        public byte WindowBottomRow7 { get; set; }//MSB
        public byte WindowBottomRow8 { get; set; } //LSB

        public ByteBuilder Level2Byte9 { get; set; }
        public ByteBuilder Level2Byte10 { get; set; }
        public ByteBuilder Level2Byte11 { get; set; }
        public byte TimeDelay12 { get; set; }

        public byte StartAddOfCharStringMSB13 { get; set; }
        public byte StartAddOfCharStringLSB14 { get; set; }

        //IF MORE THAN 1 DATA PAGE, THEN IT GOES HERE

        public byte TerminationByte1 { get; set; }
        public byte TerminationByte2 { get; set; }


        #region Level3
        public byte StatusByte { get; set; }
        public byte HorizontalOffsetMSB { get; set; }
        public byte HorizontalOffsetLSB { get; set; }
        public byte VerticalOffsetMSB { get; set; }
        public byte VerticalOffsetLSB { get; set; }

        #region Level4

        public byte[] TrainNumberBytes { get; set; } // length of array is fixed


        public byte TrainNameFieldIndexMSB1 { get; set; }
        public byte TrainNameFieldIndexLSB2 { get; set; }
        public byte[] TrainNameBytes { get; set; }// length of array is NOT fixed
                                                  // will need to find it out dynamically 

        public byte TimeFieldIndexMSB1 { get; set; }//For another status this will automatically become the field index for the next field, whatever it is
        public byte TimeFieldIndexLSB2 { get; set; }//For another status this will automatically become the field index for the next field, whatever it is

        #region DEFAULT
        public byte[] TimeBytes { get; set; } // length of array is fixed


        public byte ArrivalOrDeptureFieldIndexMSB1 { get; set; }
        public byte ArrivalOrDeptureFieldIndexLSB2 { get; set; }
        public byte[] ArrivalOrDeptureBytes { get; set; } // length of array is fixed



        public byte PlatformNumberFieldIndexMSB1 { get; set; }
        public byte PlatformNumberFieldIndexLSB2 { get; set; }
        public byte[] PlatformNumberBytes { get; set; } // length of array is fixed


        #endregion


        #region Cancelled/Indefinite Late  Diverted/Terminated at/Change of Source  Rescheduled  Arrived

        public byte[] Nplus5toKfIELD { get; set; }
        public byte[] StationNameBytes { get; set; } // FOR PAGE 2: Diverted/Terminated at/Change of Source
        #endregion




        public byte SeparatorByte1 { get; set; } // Separator byte : always 0xE7
        public byte SeparatorByte2 { get; set; } // Separator byte : always 0x00



        #endregion



        #endregion

        public byte CharacterStringTerminationByte1 { get; set; } //always 0xFF
        public byte CharacterStringTerminationByte2 { get; set; } //always 0xFF

        #endregion

        public byte Level1EndOfDataPacket { get; set; } // always 0x03

        public byte CRC_MSB { get; set; }
        public byte CRC_LSB { get; set; }

        public byte EOT { get; set; }

        #endregion



        #endregion

        //These are fields which are read from the device
        //or from the UI directly, and are not part of the frame structure
        //in itself
        #region DIRECT READ FIELDS FROM UI OR ACTIVETRAIN INSTANCE OR FROM A DEVICE INSTANCE

        #region ForLevel1
        public IPAddress DestinationIPAddress { get; set; }
        public IPAddress SourceIPAddress { get; set; }

        #endregion

        #region ForLevel2
        //These fields are used to set the bit values in the
        //Level2Byte9, Level2Byte10, Level2Byte11, and Level2Byte12,
        public bool ReverseVideo { get; set; }
        public byte Speed { get; set; }
        public byte EffectCode { get; set; }
        public byte LetterSize { get; set; }
        public byte Gap { get; set; }
        public byte TimeDelay { get; set; }
        #endregion

        //#region ForLevel3
        //// public int StatusCode { get; set; }
        //public string StatusMessage { get; set; }

        //#endregion

        //#region ForLevel4
        ////public int TrainNumber { get; set; }
        //public string TrainNumber { get; set; }
        //public string TrainName { get; set; }
        //public string Time { get; set; }
        //public char ArrivalOrDepture { get; set; }
        //public int PlatformNumber { get; set; }
        //public string StationName { get; set; } // FOR PAGE 2: Diverted/Terminated at/Change of Source


        //#endregion

        #endregion

        public FrameForMultiline()
        {
                
        }
    }
}
