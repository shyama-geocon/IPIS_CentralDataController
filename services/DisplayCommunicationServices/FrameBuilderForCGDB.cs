using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using IpisCentralDisplayController.models;
using IpisCentralDisplayController.models.DisplayCommunication;

namespace IpisCentralDisplayController.services.DisplayCommunicationServices
{
    public class FrameBuilderForCGDB
    {
        private List<byte> Frame;
        private FrameForCGDB FrameBytesObject;


        public FrameBuilderForCGDB()
        {
            Frame = new List<byte>();
            FrameBytesObject = new FrameForCGDB();
        }

        public void ReadAndAddDirectFields(ActiveTrain train, Device device, int COACHLIST_INDEX)
        {
            // NOTE: ALL THE DIRECT READ FIELDS WILL NOT NECESSARILY BE  
            // READ FROM THE ACTIVETRAIN INSTANCE. MANY OF THEM WILL ALSO BE  
            // DEPENDENT ON OTHER SETTINGS WHICH IN TURN IS BEING SET FROM SOME UI  
            // ELEMENT AND HENCE IS BEING STORED IN ONE OF THE FIELDS  

            #region ForLevel1

            FrameBytesObject.SourceIPAddress = IPAddress.Parse("192.168.0.253");
            FrameBytesObject.DestinationIPAddress = IPAddress.Parse(device.IpAddress);

            #endregion

            #region ForLevel2  
            FrameBytesObject.ReverseVideo = device.IsReverseVideo;
            FrameBytesObject.Speed = device.SpeedByte;
            FrameBytesObject.EffectCode = device.EffectByte;
            FrameBytesObject.LetterSize = device.LetterSizeByte;
            FrameBytesObject.Gap = device.GapByte;
            FrameBytesObject.TimeDelay = device.TimeDelayValueByte;
            #endregion


            #region ForLevel4  
            FrameBytesObject.TrainNumber = train.TrainNumber;
            FrameBytesObject.EngCoachName = train.CoachListEnglish[COACHLIST_INDEX];
            FrameBytesObject.HindiCoachName = train.CoachListHindi[COACHLIST_INDEX];

            #endregion

        }

        public void ProcessDataFromReadFields()
        {

            if (GetUtf16ByteSize(FrameBytesObject.TrainNumber) == 10)
            {
                FrameBytesObject.TrainNumberBytes = EncodeFixedUtf16BE(FrameBytesObject.TrainNumber, GetUtf16ByteSize(FrameBytesObject.TrainNumber));
            }
            else if (GetUtf16ByteSize(FrameBytesObject.TrainNumber) == 8)
            {
                FrameBytesObject.TrainNumberBytes = EncodeFixedUtf16BE($"0{FrameBytesObject.TrainNumber}", 10);
            }


            FrameBytesObject.EnglishCoachBytes = EncodeFixedUtf16BE(FrameBytesObject.EngCoachName, 10);


            FrameBytesObject.HindiCoachBytes = EncodeFixedUtf16BE( FrameBytesObject.HindiCoachName, 16);



            byte[] octets = FrameBytesObject.DestinationIPAddress.GetAddressBytes();
            // Make sure it's an IPv4 address
            if (octets.Length == 4)
            {
                FrameBytesObject.DestinationAddressThird6 = octets[2];   // Index 2 = 3rd octet
                FrameBytesObject.DestinationAddressFourth7 = octets[3];  // Index 3 = 4th octet

            }

            octets = FrameBytesObject.SourceIPAddress.GetAddressBytes();
            // Make sure it's an IPv4 address
            if (octets.Length == 4)
            {
                FrameBytesObject.SourceAddressThird8 = octets[2];   // Index 2 = 3rd octet
                FrameBytesObject.SourceAddressFourth9 = octets[3];  // Index 3 = 4th octet

            }

            #region Byte5

            FrameBytesObject.Level2Byte5 = new ByteBuilder();

            FrameBytesObject.Level2Byte5.SetBit(7, FrameBytesObject.ReverseVideo); // Reverse Video
            FrameBytesObject.Level2Byte5.SetBit(6, false);
            FrameBytesObject.Level2Byte5.SetBit(5, false);
            FrameBytesObject.Level2Byte5.SetBit(4, false);
            FrameBytesObject.Level2Byte5.SetBit(3, false);

            if (FrameBytesObject.Speed == 0x00)
            {
                FrameBytesObject.Level2Byte5.SetBit(2, false);
                FrameBytesObject.Level2Byte5.SetBit(1, false);
                FrameBytesObject.Level2Byte5.SetBit(0, false);
            }
            else if (FrameBytesObject.Speed == 0x01)
            {
                FrameBytesObject.Level2Byte5.SetBit(2, false);
                FrameBytesObject.Level2Byte5.SetBit(1, false);
                FrameBytesObject.Level2Byte5.SetBit(0, true);
            }
            else if (FrameBytesObject.Speed == 0x02)
            {
                FrameBytesObject.Level2Byte5.SetBit(2, false);
                FrameBytesObject.Level2Byte5.SetBit(1, true);
                FrameBytesObject.Level2Byte5.SetBit(0, false);
            }
            else if (FrameBytesObject.Speed == 0x03)
            {
                FrameBytesObject.Level2Byte5.SetBit(2, false);
                FrameBytesObject.Level2Byte5.SetBit(1, true);
                FrameBytesObject.Level2Byte5.SetBit(0, true);
            }
            else if (FrameBytesObject.Speed == 0x04)
            {
                FrameBytesObject.Level2Byte5.SetBit(2, true);
                FrameBytesObject.Level2Byte5.SetBit(1, false);
                FrameBytesObject.Level2Byte5.SetBit(0, false);
            }

            #endregion

            #region Byte6

            FrameBytesObject.Level2Byte6 = new ByteBuilder();

            FrameBytesObject.Level2Byte6.SetBit(7, false);
            FrameBytesObject.Level2Byte6.SetBit(6, false);
            FrameBytesObject.Level2Byte6.SetBit(5, false);
            FrameBytesObject.Level2Byte6.SetBit(4, false);


            if (FrameBytesObject.EffectCode == 0x00)
            {
                FrameBytesObject.Level2Byte6.SetBit(3, false);
                FrameBytesObject.Level2Byte6.SetBit(2, false);
                FrameBytesObject.Level2Byte6.SetBit(1, false);
                FrameBytesObject.Level2Byte6.SetBit(0, false);
            }
            else if (FrameBytesObject.EffectCode == 0x01)
            {
                FrameBytesObject.Level2Byte6.SetBit(3, false);
                FrameBytesObject.Level2Byte6.SetBit(2, false);
                FrameBytesObject.Level2Byte6.SetBit(1, false);
                FrameBytesObject.Level2Byte6.SetBit(0, true);
            }
            else if (FrameBytesObject.EffectCode == 0x02)
            {
                FrameBytesObject.Level2Byte6.SetBit(3, false);
                FrameBytesObject.Level2Byte6.SetBit(2, false);
                FrameBytesObject.Level2Byte6.SetBit(1, true);
                FrameBytesObject.Level2Byte6.SetBit(0, false);
            }
            else if (FrameBytesObject.EffectCode == 0x03)
            {
                FrameBytesObject.Level2Byte6.SetBit(3, false);
                FrameBytesObject.Level2Byte6.SetBit(2, false);
                FrameBytesObject.Level2Byte6.SetBit(1, true);
                FrameBytesObject.Level2Byte6.SetBit(0, true);
            }
            else if (FrameBytesObject.EffectCode == 0x04)
            {
                FrameBytesObject.Level2Byte6.SetBit(3, false);
                FrameBytesObject.Level2Byte6.SetBit(2, true);
                FrameBytesObject.Level2Byte6.SetBit(1, false);
                FrameBytesObject.Level2Byte6.SetBit(0, false);
            }
            else if (FrameBytesObject.EffectCode == 0x05)
            {
                FrameBytesObject.Level2Byte6.SetBit(3, false);
                FrameBytesObject.Level2Byte6.SetBit(2, true);
                FrameBytesObject.Level2Byte6.SetBit(1, false);
                FrameBytesObject.Level2Byte6.SetBit(0, true);
            }
            else if (FrameBytesObject.EffectCode == 0x06)
            {
                FrameBytesObject.Level2Byte6.SetBit(3, false);
                FrameBytesObject.Level2Byte6.SetBit(2, true);
                FrameBytesObject.Level2Byte6.SetBit(1, true);
                FrameBytesObject.Level2Byte6.SetBit(0, false);
            }
            else if (FrameBytesObject.EffectCode == 0x07)
            {
                FrameBytesObject.Level2Byte6.SetBit(3, false);
                FrameBytesObject.Level2Byte6.SetBit(2, true);
                FrameBytesObject.Level2Byte6.SetBit(1, true);
                FrameBytesObject.Level2Byte6.SetBit(0, true);
            }
            else if (FrameBytesObject.EffectCode == 0x08)
            {
                FrameBytesObject.Level2Byte6.SetBit(3, true);
                FrameBytesObject.Level2Byte6.SetBit(2, false);
                FrameBytesObject.Level2Byte6.SetBit(1, false);
                FrameBytesObject.Level2Byte6.SetBit(0, false);
            }
            else if (FrameBytesObject.EffectCode == 0x09)
            {
                FrameBytesObject.Level2Byte6.SetBit(3, true);
                FrameBytesObject.Level2Byte6.SetBit(2, false);
                FrameBytesObject.Level2Byte6.SetBit(1, false);
                FrameBytesObject.Level2Byte6.SetBit(0, true);
            }

            #endregion

            #region Byte7

            FrameBytesObject.Level2Byte7 = new ByteBuilder();


            FrameBytesObject.Level2Byte7.SetBit(7, false);
            FrameBytesObject.Level2Byte7.SetBit(6, false);

            if (FrameBytesObject.LetterSize == 0x00)
            {
                FrameBytesObject.Level2Byte7.SetBit(5, false);
                FrameBytesObject.Level2Byte7.SetBit(4, false);
                FrameBytesObject.Level2Byte7.SetBit(3, false);

            }
            else if (FrameBytesObject.LetterSize == 0x01)
            {
                FrameBytesObject.Level2Byte7.SetBit(5, false);
                FrameBytesObject.Level2Byte7.SetBit(4, false);
                FrameBytesObject.Level2Byte7.SetBit(3, true);

            }
            else if (FrameBytesObject.LetterSize == 0x02)
            {
                FrameBytesObject.Level2Byte7.SetBit(5, false);
                FrameBytesObject.Level2Byte7.SetBit(4, true);
                FrameBytesObject.Level2Byte7.SetBit(3, false);

            }
            else if (FrameBytesObject.LetterSize == 0x03)
            {
                FrameBytesObject.Level2Byte7.SetBit(5, false);
                FrameBytesObject.Level2Byte7.SetBit(4, true);
                FrameBytesObject.Level2Byte7.SetBit(3, true);

            }
            else if (FrameBytesObject.LetterSize == 0x04)
            {
                FrameBytesObject.Level2Byte7.SetBit(5, true);
                FrameBytesObject.Level2Byte7.SetBit(4, false);
                FrameBytesObject.Level2Byte7.SetBit(3, false);

            }
            else if (FrameBytesObject.LetterSize == 0x05)
            {
                FrameBytesObject.Level2Byte7.SetBit(5, true);
                FrameBytesObject.Level2Byte7.SetBit(4, false);
                FrameBytesObject.Level2Byte7.SetBit(3, true);

            }




            if (FrameBytesObject.Gap == 0x00)
            {
                FrameBytesObject.Level2Byte7.SetBit(2, false);
                FrameBytesObject.Level2Byte7.SetBit(1, false);
                FrameBytesObject.Level2Byte7.SetBit(0, false);
            }
            else if (FrameBytesObject.Gap == 0x01)
            {
                FrameBytesObject.Level2Byte7.SetBit(2, false);
                FrameBytesObject.Level2Byte7.SetBit(1, false);
                FrameBytesObject.Level2Byte7.SetBit(0, true);
            }
            else if (FrameBytesObject.Gap == 0x02)
            {
                FrameBytesObject.Level2Byte7.SetBit(2, false);
                FrameBytesObject.Level2Byte7.SetBit(1, true);
                FrameBytesObject.Level2Byte7.SetBit(0, false);
            }
            else if (FrameBytesObject.Gap == 0x03)
            {
                FrameBytesObject.Level2Byte7.SetBit(2, false);
                FrameBytesObject.Level2Byte7.SetBit(1, true);
                FrameBytesObject.Level2Byte7.SetBit(0, true);
            }
            else if (FrameBytesObject.Gap == 0x04)
            {
                FrameBytesObject.Level2Byte7.SetBit(2, true);
                FrameBytesObject.Level2Byte7.SetBit(1, false);
                FrameBytesObject.Level2Byte7.SetBit(0, false);
            }
            else if (FrameBytesObject.Gap == 0x05)
            {
                FrameBytesObject.Level2Byte7.SetBit(2, true);
                FrameBytesObject.Level2Byte7.SetBit(1, false);
                FrameBytesObject.Level2Byte7.SetBit(0, true);
            }
            else if (FrameBytesObject.Gap == 0x06)
            {
                FrameBytesObject.Level2Byte7.SetBit(2, true);
                FrameBytesObject.Level2Byte7.SetBit(1, true);
                FrameBytesObject.Level2Byte7.SetBit(0, false);
            }
            else if (FrameBytesObject.Gap == 0x07)
            {
                FrameBytesObject.Level2Byte7.SetBit(2, true);
                FrameBytesObject.Level2Byte7.SetBit(1, true);
                FrameBytesObject.Level2Byte7.SetBit(0, true);
            }


            #endregion

            FrameBytesObject.TimeDelay8 = (byte)FrameBytesObject.TimeDelay;


           
        }

        public void AddFixedBytes()
        {

            #region ForLevel1

            FrameBytesObject.Start1 = 0xAA;
            FrameBytesObject.Start2 = 0xCC;

            FrameBytesObject.PacketIdentifier3 = 0x02; //Packet for CGDB

            //public byte PacketLengthMSB4 { get; set; }
            //public byte PacketLengthLSB5 { get; set; }

            //public byte SerialNumber10 { get; set; }

            FrameBytesObject.PacketType11 = 0x81; //Data Transfer(Data)

            FrameBytesObject.StartOfDataPacketIndicator12 = 0x02;

            // DATA STUFF GOES HERE

            FrameBytesObject.Level1EndOfDataPacket = 0x03;

            //FrameBytesObject.CRC_MSB = 0xCC;
            //FrameBytesObject.CRC_LSB = 0xCC;

            FrameBytesObject.EOT = 0x04;

            #endregion

            #region ForLevel2

            FrameBytesObject.WindowLeftColumn1 = 0x01; 

            FrameBytesObject.WindowRightColumn2 = 0x30; 

            FrameBytesObject.WindowTopRow3 = 0x10; 

            FrameBytesObject.WindowBottomRow4 = 0x01;


           
                

            FrameBytesObject.SeparatorByte1 = 0xE7;
            FrameBytesObject.SeparatorByte2 = 0x00;

            

            FrameBytesObject.EndOfCoachData = 0xEC;


            #endregion

          
        }

        public byte[] CompileFrame()
        {
            Frame.Clear();
            List<int> CharacterStringStartAddToBeAddedAtIndexes = new List<int>();

            #region LEVEL1 CONSTRUCTION  
            Frame.Add(FrameBytesObject.Start1);
            Frame.Add(FrameBytesObject.Start2);
            Frame.Add(FrameBytesObject.PacketIdentifier3);
            Frame.Add(FrameBytesObject.PacketLengthMSB4);//NOT ADDED  
            Frame.Add(FrameBytesObject.PacketLengthLSB5);//NOT ADDED  
            Frame.Add(FrameBytesObject.DestinationAddressThird6);
            Frame.Add(FrameBytesObject.DestinationAddressFourth7);
            Frame.Add(FrameBytesObject.SourceAddressThird8);
            Frame.Add(FrameBytesObject.SourceAddressFourth9);
            Frame.Add(FrameBytesObject.SerialNumber10);//NOT ADDED  
            Frame.Add(FrameBytesObject.PacketType11);
            Frame.Add(FrameBytesObject.StartOfDataPacketIndicator12);

            #region LEVEL2

            Frame.Add(FrameBytesObject.WindowLeftColumn1);

            Frame.Add(FrameBytesObject.WindowRightColumn2);

            Frame.Add(FrameBytesObject.WindowTopRow3);

            Frame.Add(FrameBytesObject.WindowBottomRow4);

            Frame.Add(FrameBytesObject.Level2Byte5.ToByte());
            Frame.Add(FrameBytesObject.Level2Byte6.ToByte());
            Frame.Add(FrameBytesObject.Level2Byte7.ToByte());

            Frame.Add(FrameBytesObject.TimeDelay8);

            Frame.AddRange(FrameBytesObject.TrainNumberBytes);

            Frame.AddRange(FrameBytesObject.EnglishCoachBytes);

            Frame.Add(FrameBytesObject.SeparatorByte1);
            Frame.Add(FrameBytesObject.SeparatorByte2);

            Frame.AddRange(FrameBytesObject.HindiCoachBytes);

            Frame.Add(FrameBytesObject.EndOfCoachData);

            #endregion

            Frame.Add(FrameBytesObject.Level1EndOfDataPacket);
            Frame.Add(FrameBytesObject.CRC_MSB);//NOT ADDED  
            Frame.Add(FrameBytesObject.CRC_LSB);//NOT ADDED  
            Frame.Add(FrameBytesObject.EOT);




            //CRC Left

            #endregion


            //FrameBytesObject.PacketLengthMSB4
            Frame[3] = (byte)(((Frame.Count - 6) >> 8) & 0xFF);// Most Significant Byte

            //FrameBytesObject.PacketLengthLSB5
            Frame[4] = (byte)((Frame.Count - 6) & 0xFF);// Least Significant Byte

            byte[] frametosend = Frame.ToArray();

            return frametosend;
        }


        //helper method which converts the string to the specified number of bytes
        //in UTF-16 Big Endian format encoding
        public static byte[] EncodeFixedUtf16BE(string input, int byteLength)
        {
            byte[] result = new byte[byteLength];
            byte[] encoded = Encoding.BigEndianUnicode.GetBytes(input);

            int copyLength = Math.Min(encoded.Length, byteLength);
            Array.Copy(encoded, result, copyLength);

            return result;
        }

        // returns the number of bytes required to encode the string in UTF-16 format
        //public static int GetUtf16ByteSize(string input, bool bigEndian = false)
        public static int GetUtf16ByteSize(string input)
        {
            bool bigEndian = true;
            if (input == null)
                return 0;

            Encoding utf16 = bigEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;
            return utf16.GetByteCount(input);
        }


    }
}
