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
    public class FrameBuilderForAGDB
    {
        private List<byte> Frame;
        private FrameForAGDB FrameBytesObject;


        public FrameBuilderForAGDB()
        {
            Frame = new List<byte>();
            FrameBytesObject = new FrameForAGDB();
        }

        public void ReadAndAddDirectFields(ActiveTrain train, Device device)
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

            #region ForLevel3  

            // FrameBytesObject.StatusMessage = train.Status;
            FrameBytesObject.StatusByte = train.StatusByte;
           

            #endregion

            #region ForLevel4  
            //if (int.TryParse(train.TrainNumber, out int parsedTrainNumber))
            //{
            //    FrameBytesObject.TrainNumber = parsedTrainNumber;
            //}
            //else
            //{
            //    throw new InvalidOperationException($"TrainNumber '{train.TrainNumber}' cannot be converted to an integer.");
            //}

            FrameBytesObject.TrainNumber = train.TrainNumber;
            FrameBytesObject.TrainName = train.TrainNameEnglish;

            // Convert string to char explicitly  
            FrameBytesObject.ArrivalOrDeparture = !string.IsNullOrEmpty(train.SelectedADOption) && train.SelectedADOption.Length == 1
                ? train.SelectedADOption[0]
                : throw new InvalidOperationException($"SelectedADOption '{train.SelectedADOption}' is not a valid single character.");


            if (FrameBytesObject.ArrivalOrDeparture == 'A')
            {
                FrameBytesObject.Time = new DateTime(train.ETA.Value.Ticks).ToString("HH:mm");
            }
            else if (FrameBytesObject.ArrivalOrDeparture == 'D')
            {
                FrameBytesObject.Time = new DateTime(train.ETD.Value.Ticks).ToString("HH:mm");
            }
            //For page 2 Diverted/Terminated at/Change of Source
            FrameBytesObject.StationName = train.SplStationNameEnglish;

            FrameBytesObject.PlatformNumber = train.PFNo;


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


            FrameBytesObject.TrainNameBytes = EncodeFixedUtf16BE(FrameBytesObject.TrainName, GetUtf16ByteSize(FrameBytesObject.TrainName));


            FrameBytesObject.TimeBytes = EncodeFixedUtf16BE(FrameBytesObject.Time, GetUtf16ByteSize(FrameBytesObject.Time)); // No. of bytes for time should always be 10 (5 * 2)
            FrameBytesObject.ArrivalOrDeptureBytes = new byte[2];
            FrameBytesObject.ArrivalOrDeptureBytes[0] = 0x00;
            FrameBytesObject.ArrivalOrDeptureBytes[1] = (byte)FrameBytesObject.ArrivalOrDeparture; // Convert char to byte


            if ((FrameBytesObject.PlatformNumber).ToString().Length == 1)
            {
                FrameBytesObject.PlatformNumberBytes = EncodeFixedUtf16BE($"00{(FrameBytesObject.PlatformNumber)}", 6);
            }
            else if ((FrameBytesObject.PlatformNumber).ToString().Length == 2)
            {
                FrameBytesObject.PlatformNumberBytes = EncodeFixedUtf16BE($"0{(FrameBytesObject.PlatformNumber)}", 6);
            }
            else if ((FrameBytesObject.PlatformNumber).ToString().Length == 3)
            {
                FrameBytesObject.PlatformNumberBytes = EncodeFixedUtf16BE((FrameBytesObject.PlatformNumber).ToString(), 6);
            }


            if (FrameBytesObject.StatusByte == 0x04)
            {
                FrameBytesObject.Nplus5toKfIELD = EncodeFixedUtf16BE("Arrived", GetUtf16ByteSize("Arrived"));
            }
            else if (FrameBytesObject.StatusByte == 0x06)
            {
                FrameBytesObject.Nplus5toKfIELD = EncodeFixedUtf16BE("Cancelled", GetUtf16ByteSize("Cancelled"));
            }
            else if (FrameBytesObject.StatusByte == 0x07)
            {
                FrameBytesObject.Nplus5toKfIELD = EncodeFixedUtf16BE("Indefinite Late", GetUtf16ByteSize("Indefinite Late"));
            }
            else if (FrameBytesObject.StatusByte == 0x08)
            {
                FrameBytesObject.Nplus5toKfIELD = EncodeFixedUtf16BE("Terminated At", GetUtf16ByteSize("Terminated At"));
                FrameBytesObject.StationNameBytes = EncodeFixedUtf16BE(FrameBytesObject.StationName, GetUtf16ByteSize(FrameBytesObject.StationName));
            }
            else if (FrameBytesObject.StatusByte == 0x0B)
            {
                FrameBytesObject.Nplus5toKfIELD = EncodeFixedUtf16BE("Cancelled", GetUtf16ByteSize("Cancelled"));
            }
            else if (FrameBytesObject.StatusByte == 0x0F)
            {
                FrameBytesObject.Nplus5toKfIELD = EncodeFixedUtf16BE("Rescheduled", GetUtf16ByteSize("Rescheduled"));

            }
            else if (FrameBytesObject.StatusByte == 0x10)
            {
                FrameBytesObject.Nplus5toKfIELD = EncodeFixedUtf16BE("Diverted", GetUtf16ByteSize("Diverted"));
                FrameBytesObject.StationNameBytes = EncodeFixedUtf16BE(FrameBytesObject.StationName, GetUtf16ByteSize(FrameBytesObject.StationName));

            }
            else if (FrameBytesObject.StatusByte == 0x13)
            {
                FrameBytesObject.Nplus5toKfIELD = EncodeFixedUtf16BE("Start at", GetUtf16ByteSize("Start at"));
                FrameBytesObject.StationNameBytes = EncodeFixedUtf16BE(FrameBytesObject.StationName, GetUtf16ByteSize(FrameBytesObject.StationName));
            }
            else
            {
                FrameBytesObject.Nplus5toKfIELD = null;
                FrameBytesObject.StationNameBytes = null;

                //  throw new Exception(message: $"Unknown status message: {FrameBytesObject.StatusMessage}");
            }

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

            #region Byte9

            FrameBytesObject.Level2Byte9 = new ByteBuilder();

            FrameBytesObject.Level2Byte9.SetBit(7, FrameBytesObject.ReverseVideo); // Reverse Video
            FrameBytesObject.Level2Byte9.SetBit(6, false);
            FrameBytesObject.Level2Byte9.SetBit(5, false);
            FrameBytesObject.Level2Byte9.SetBit(4, false);
            FrameBytesObject.Level2Byte9.SetBit(3, false);

            if (FrameBytesObject.Speed == 0x00)
            {
                FrameBytesObject.Level2Byte9.SetBit(2, false);
                FrameBytesObject.Level2Byte9.SetBit(1, false);
                FrameBytesObject.Level2Byte9.SetBit(0, false);
            }
            else if (FrameBytesObject.Speed == 0x01)
            {
                FrameBytesObject.Level2Byte9.SetBit(2, false);
                FrameBytesObject.Level2Byte9.SetBit(1, false);
                FrameBytesObject.Level2Byte9.SetBit(0, true);
            }
            else if (FrameBytesObject.Speed == 0x02)
            {
                FrameBytesObject.Level2Byte9.SetBit(2, false);
                FrameBytesObject.Level2Byte9.SetBit(1, true);
                FrameBytesObject.Level2Byte9.SetBit(0, false);
            }
            else if (FrameBytesObject.Speed == 0x03)
            {
                FrameBytesObject.Level2Byte9.SetBit(2, false);
                FrameBytesObject.Level2Byte9.SetBit(1, true);
                FrameBytesObject.Level2Byte9.SetBit(0, true);
            }
            else if (FrameBytesObject.Speed == 0x04)
            {
                FrameBytesObject.Level2Byte9.SetBit(2, true);
                FrameBytesObject.Level2Byte9.SetBit(1, false);
                FrameBytesObject.Level2Byte9.SetBit(0, false);
            }

            #endregion

            #region Byte10

            FrameBytesObject.Level2Byte10 = new ByteBuilder();

            FrameBytesObject.Level2Byte10.SetBit(7, false);
            FrameBytesObject.Level2Byte10.SetBit(6, false);
            FrameBytesObject.Level2Byte10.SetBit(5, false);
            FrameBytesObject.Level2Byte10.SetBit(4, false);


            if (FrameBytesObject.EffectCode == 0x00)
            {
                FrameBytesObject.Level2Byte10.SetBit(3, false);
                FrameBytesObject.Level2Byte10.SetBit(2, false);
                FrameBytesObject.Level2Byte10.SetBit(1, false);
                FrameBytesObject.Level2Byte10.SetBit(0, false);
            }
            else if (FrameBytesObject.EffectCode == 0x01)
            {
                FrameBytesObject.Level2Byte10.SetBit(3, false);
                FrameBytesObject.Level2Byte10.SetBit(2, false);
                FrameBytesObject.Level2Byte10.SetBit(1, false);
                FrameBytesObject.Level2Byte10.SetBit(0, true);
            }
            else if (FrameBytesObject.EffectCode == 0x02)
            {
                FrameBytesObject.Level2Byte10.SetBit(3, false);
                FrameBytesObject.Level2Byte10.SetBit(2, false);
                FrameBytesObject.Level2Byte10.SetBit(1, true);
                FrameBytesObject.Level2Byte10.SetBit(0, false);
            }
            else if (FrameBytesObject.EffectCode == 0x03)
            {
                FrameBytesObject.Level2Byte10.SetBit(3, false);
                FrameBytesObject.Level2Byte10.SetBit(2, false);
                FrameBytesObject.Level2Byte10.SetBit(1, true);
                FrameBytesObject.Level2Byte10.SetBit(0, true);
            }
            else if (FrameBytesObject.EffectCode == 0x04)
            {
                FrameBytesObject.Level2Byte10.SetBit(3, false);
                FrameBytesObject.Level2Byte10.SetBit(2, true);
                FrameBytesObject.Level2Byte10.SetBit(1, false);
                FrameBytesObject.Level2Byte10.SetBit(0, false);
            }
            else if (FrameBytesObject.EffectCode == 0x05)
            {
                FrameBytesObject.Level2Byte10.SetBit(3, false);
                FrameBytesObject.Level2Byte10.SetBit(2, true);
                FrameBytesObject.Level2Byte10.SetBit(1, false);
                FrameBytesObject.Level2Byte10.SetBit(0, true);
            }
            else if (FrameBytesObject.EffectCode == 0x06)
            {
                FrameBytesObject.Level2Byte10.SetBit(3, false);
                FrameBytesObject.Level2Byte10.SetBit(2, true);
                FrameBytesObject.Level2Byte10.SetBit(1, true);
                FrameBytesObject.Level2Byte10.SetBit(0, false);
            }
            else if (FrameBytesObject.EffectCode == 0x07)
            {
                FrameBytesObject.Level2Byte10.SetBit(3, false);
                FrameBytesObject.Level2Byte10.SetBit(2, true);
                FrameBytesObject.Level2Byte10.SetBit(1, true);
                FrameBytesObject.Level2Byte10.SetBit(0, true);
            }
            else if (FrameBytesObject.EffectCode == 0x08)
            {
                FrameBytesObject.Level2Byte10.SetBit(3, true);
                FrameBytesObject.Level2Byte10.SetBit(2, false);
                FrameBytesObject.Level2Byte10.SetBit(1, false);
                FrameBytesObject.Level2Byte10.SetBit(0, false);
            }
            else if (FrameBytesObject.EffectCode == 0x09)
            {
                FrameBytesObject.Level2Byte10.SetBit(3, true);
                FrameBytesObject.Level2Byte10.SetBit(2, false);
                FrameBytesObject.Level2Byte10.SetBit(1, false);
                FrameBytesObject.Level2Byte10.SetBit(0, true);
            }

            #endregion

            #region Byte11
            FrameBytesObject.Level2Byte11 = new ByteBuilder();


            FrameBytesObject.Level2Byte11.SetBit(7, false);
            FrameBytesObject.Level2Byte11.SetBit(6, false);

            if (FrameBytesObject.LetterSize == 0x00)
            {
                FrameBytesObject.Level2Byte11.SetBit(5, false);
                FrameBytesObject.Level2Byte11.SetBit(4, false);
                FrameBytesObject.Level2Byte11.SetBit(3, false);

            }
            else if (FrameBytesObject.LetterSize == 0x01)
            {
                FrameBytesObject.Level2Byte11.SetBit(5, false);
                FrameBytesObject.Level2Byte11.SetBit(4, false);
                FrameBytesObject.Level2Byte11.SetBit(3, true);

            }
            else if (FrameBytesObject.LetterSize == 0x02)
            {
                FrameBytesObject.Level2Byte11.SetBit(5, false);
                FrameBytesObject.Level2Byte11.SetBit(4, true);
                FrameBytesObject.Level2Byte11.SetBit(3, false);

            }
            else if (FrameBytesObject.LetterSize == 0x03)
            {
                FrameBytesObject.Level2Byte11.SetBit(5, false);
                FrameBytesObject.Level2Byte11.SetBit(4, true);
                FrameBytesObject.Level2Byte11.SetBit(3, true);

            }
            else if (FrameBytesObject.LetterSize == 0x04)
            {
                FrameBytesObject.Level2Byte11.SetBit(5, true);
                FrameBytesObject.Level2Byte11.SetBit(4, false);
                FrameBytesObject.Level2Byte11.SetBit(3, false);

            }
            else if (FrameBytesObject.LetterSize == 0x05)
            {
                FrameBytesObject.Level2Byte11.SetBit(5, true);
                FrameBytesObject.Level2Byte11.SetBit(4, false);
                FrameBytesObject.Level2Byte11.SetBit(3, true);

            }




            if (FrameBytesObject.Gap == 0x00)
            {
                FrameBytesObject.Level2Byte11.SetBit(2, false);
                FrameBytesObject.Level2Byte11.SetBit(1, false);
                FrameBytesObject.Level2Byte11.SetBit(0, false);
            }
            else if (FrameBytesObject.Gap == 0x01)
            {
                FrameBytesObject.Level2Byte11.SetBit(2, false);
                FrameBytesObject.Level2Byte11.SetBit(1, false);
                FrameBytesObject.Level2Byte11.SetBit(0, true);
            }
            else if (FrameBytesObject.Gap == 0x02)
            {
                FrameBytesObject.Level2Byte11.SetBit(2, false);
                FrameBytesObject.Level2Byte11.SetBit(1, true);
                FrameBytesObject.Level2Byte11.SetBit(0, false);
            }
            else if (FrameBytesObject.Gap == 0x03)
            {
                FrameBytesObject.Level2Byte11.SetBit(2, false);
                FrameBytesObject.Level2Byte11.SetBit(1, true);
                FrameBytesObject.Level2Byte11.SetBit(0, true);
            }
            else if (FrameBytesObject.Gap == 0x04)
            {
                FrameBytesObject.Level2Byte11.SetBit(2, true);
                FrameBytesObject.Level2Byte11.SetBit(1, false);
                FrameBytesObject.Level2Byte11.SetBit(0, false);
            }
            else if (FrameBytesObject.Gap == 0x05)
            {
                FrameBytesObject.Level2Byte11.SetBit(2, true);
                FrameBytesObject.Level2Byte11.SetBit(1, false);
                FrameBytesObject.Level2Byte11.SetBit(0, true);
            }
            else if (FrameBytesObject.Gap == 0x06)
            {
                FrameBytesObject.Level2Byte11.SetBit(2, true);
                FrameBytesObject.Level2Byte11.SetBit(1, true);
                FrameBytesObject.Level2Byte11.SetBit(0, false);
            }
            else if (FrameBytesObject.Gap == 0x07)
            {
                FrameBytesObject.Level2Byte11.SetBit(2, true);
                FrameBytesObject.Level2Byte11.SetBit(1, true);
                FrameBytesObject.Level2Byte11.SetBit(0, true);
            }


            #endregion

            FrameBytesObject.TimeDelay12 = (byte)FrameBytesObject.TimeDelay;

        }

        public void AddFixedBytes()
        {
            #region ForLevel1

            FrameBytesObject.Start1 = 0xAA;
            FrameBytesObject.Start2 = 0xCC;

            FrameBytesObject.PacketIdentifier3 = 0x04; //Packet for AGDB

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

            FrameBytesObject.WindowLeftColumn1 = 0x00; //MSB
            FrameBytesObject.WindowLeftColumn2 = 0x01; //LSB

            FrameBytesObject.WindowRightColumn3 = 0x00; //MSB
            FrameBytesObject.WindowRightColumn4 = 0xC0; //LSB

            FrameBytesObject.WindowTopRow5 = 0x00; //MSB
            FrameBytesObject.WindowTopRow6 = 0x20; //LSB

            FrameBytesObject.WindowBottomRow7 = 0x00; //MSB
            FrameBytesObject.WindowBottomRow8 = 0x01; //LSB

            //public ByteBuilder Level2Byte9 { get; set; }
            //public ByteBuilder Level2Byte10 { get; set; }
            //public ByteBuilder Level2Byte11 { get; set; }
            //public ByteBuilder Level2Byte12 { get; set; }

            // DATA STUFF GOES HERE

            FrameBytesObject.CharacterStringTerminationByte1 = 0xFF; //always 0xFF
            FrameBytesObject.CharacterStringTerminationByte2 = 0xFF; //always 0xFF

            FrameBytesObject.TerminationByte1 = 0xFF;
            FrameBytesObject.TerminationByte2 = 0xFF;

            #endregion


            #region ForLevel3
            FrameBytesObject.HorizontalOffsetMSB = 0X00;
            FrameBytesObject.HorizontalOffsetLSB = 0X10;//ROW NO.: FROM 16TH ROW


            FrameBytesObject.VerticalOffsetMSB = 0X00;
            FrameBytesObject.VerticalOffsetLSB = 0X01; //COLUMN NUMBER

            #endregion


            #region ForLevel4
            FrameBytesObject.SeparatorByte1 = 0xE7; // Separator byte 1
            FrameBytesObject.SeparatorByte2 = 0x00; // Separator byte 2

            //The field index for the first field will most probably not be used, but I'm keeping it here to avoid confusion
            FrameBytesObject._1_FieldIndexMSB1 = 0x00;
            FrameBytesObject._1_FieldIndexLSB2 = 58;

            FrameBytesObject._2_FieldIndexMSB1 = 0x00;
            FrameBytesObject._2_FieldIndexLSB2 = 0xFD;

            FrameBytesObject._3_FieldIndexMSB1 = 0x01;
            FrameBytesObject._3_FieldIndexLSB2 = 0x2D;

            FrameBytesObject._4_FieldIndexMSB1 = 0x01;
            FrameBytesObject._4_FieldIndexLSB2 = 0x3A;

            #endregion
        }




        public byte[] CompileFrame()
        {
            Frame.Clear();
            List<int> CharacterStringStartAddToBeAddedAtIndexes = new List<int>();

           

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


            int num_of_pages = 0;

            if ((FrameBytesObject.StatusByte == 0x10) || // Diverted
                (FrameBytesObject.StatusByte == 0x08) || // Terminated at
                (FrameBytesObject.StatusByte == 0x13) || // Change of Source
                (FrameBytesObject.StatusByte == 0x0F) // Rescheduled
                )
            {
                num_of_pages = 3;
            }

            else if (FrameBytesObject.StatusByte == 0x06 || // Cancelled : Arrival
                FrameBytesObject.StatusByte == 0x07 || // Indefinite Late
                FrameBytesObject.StatusByte == 0x0B // Cancelled : Departure
                )
            {
                num_of_pages = 2;
            }

            else if(FrameBytesObject.StatusByte == 0x04) // Arrived
            {
                num_of_pages = 4; //NOT SURE
            }

            else
            {
                num_of_pages = 2; 
            }

            //THIS IS LEVEL 2 CONSTRUCTION
            for (int i = 0; i < num_of_pages; i++)
            {             
                Frame.Add(FrameBytesObject.WindowLeftColumn1);
                Frame.Add(FrameBytesObject.WindowLeftColumn2);

                Frame.Add(FrameBytesObject.WindowRightColumn3);
                Frame.Add(FrameBytesObject.WindowRightColumn4);

                Frame.Add(FrameBytesObject.WindowTopRow5);
                Frame.Add(FrameBytesObject.WindowTopRow6);

                Frame.Add(FrameBytesObject.WindowBottomRow7);
                Frame.Add(FrameBytesObject.WindowBottomRow8);

                Frame.Add(FrameBytesObject.Level2Byte9.ToByte());
                Frame.Add(FrameBytesObject.Level2Byte10.ToByte());
                Frame.Add(FrameBytesObject.Level2Byte11.ToByte());
                //Frame.Add(FrameBytesObject.Level2Byte12.ToByte());
                Frame.Add(FrameBytesObject.TimeDelay12);

                CharacterStringStartAddToBeAddedAtIndexes.Add((Frame.Count));
                Frame.Add(FrameBytesObject.StartAddOfCharStringMSB13);//NOT ADDED
                Frame.Add(FrameBytesObject.StartAddOfCharStringLSB14);//NOT ADDED
            }

            Frame.Add(FrameBytesObject.TerminationByte1);
            Frame.Add(FrameBytesObject.TerminationByte2);



            //THIS IS LEVEL 3 and 4 CONSTRUCTION
            if (FrameBytesObject.StatusByte == 0x06 || // Cancelled : Arrival
                FrameBytesObject.StatusByte == 0x07 || // Indefinite Late
                FrameBytesObject.StatusByte == 0x0B // Cancelled : Departure
                )
            {

                #region PAGE1(line1)

                int index_to_be_added = Frame.Count;
                FrameBytesObject.StartAddOfCharStringMSB13 = (byte)((index_to_be_added & 0xFF00) >> 8); //MSB
                FrameBytesObject.StartAddOfCharStringLSB14 = (byte)((index_to_be_added & 0x00FF)); //LSB
                Frame[CharacterStringStartAddToBeAddedAtIndexes[0]] = FrameBytesObject.StartAddOfCharStringMSB13; // Update MSB
                Frame[CharacterStringStartAddToBeAddedAtIndexes[0] + 1] = FrameBytesObject.StartAddOfCharStringLSB14; // Update LSB

                FrameBytesObject.HorizontalOffsetMSB = 0x00;
                FrameBytesObject.HorizontalOffsetLSB = 0x20;

                FrameBytesObject.VerticalOffsetMSB = 0x00;
                FrameBytesObject.VerticalOffsetLSB = 0x01; 

                Frame.Add(FrameBytesObject.StatusByte);
                Frame.Add(FrameBytesObject.HorizontalOffsetMSB); // row number
                Frame.Add(FrameBytesObject.HorizontalOffsetLSB);
                Frame.Add(FrameBytesObject.VerticalOffsetMSB); //column number
                Frame.Add(FrameBytesObject.VerticalOffsetLSB);

                Frame.AddRange(FrameBytesObject.TrainNumberBytes);
                Frame.Add(FrameBytesObject.SeparatorByte1);
                Frame.Add(FrameBytesObject.SeparatorByte2);

                Frame.Add(FrameBytesObject._2_FieldIndexMSB1);
                Frame.Add(FrameBytesObject._2_FieldIndexLSB2);
                Frame.AddRange(FrameBytesObject.Nplus5toKfIELD); // "Cancelled" or "Indefinite Late"

                Frame.Add(FrameBytesObject.TerminationByte1);
                Frame.Add(FrameBytesObject.TerminationByte2);
                #endregion


                #region PAGE2(line2)

                index_to_be_added = Frame.Count;
                FrameBytesObject.StartAddOfCharStringMSB13 = (byte)((index_to_be_added & 0xFF00) >> 8); //MSB
                FrameBytesObject.StartAddOfCharStringLSB14 = (byte)((index_to_be_added & 0x00FF)); //LSB
                Frame[CharacterStringStartAddToBeAddedAtIndexes[1]] = FrameBytesObject.StartAddOfCharStringMSB13; // Update MSB
                Frame[CharacterStringStartAddToBeAddedAtIndexes[1] + 1] = FrameBytesObject.StartAddOfCharStringLSB14; // Update LSB

                FrameBytesObject.HorizontalOffsetMSB = 0x00;
                FrameBytesObject.HorizontalOffsetLSB = 0x10;

                FrameBytesObject.VerticalOffsetMSB = 0x00;
                FrameBytesObject.VerticalOffsetLSB = 0x01;

                Frame.Add(FrameBytesObject.StatusByte);
                Frame.Add(FrameBytesObject.HorizontalOffsetMSB); // row number
                Frame.Add(FrameBytesObject.HorizontalOffsetLSB);
                Frame.Add(FrameBytesObject.VerticalOffsetMSB); //column number
                Frame.Add(FrameBytesObject.VerticalOffsetLSB);

                Frame.AddRange(FrameBytesObject.TrainNameBytes);

                Frame.Add(FrameBytesObject.TerminationByte1);
                Frame.Add(FrameBytesObject.TerminationByte2);
                #endregion

            }


            else if (FrameBytesObject.StatusByte == 0x10 || //Diverted
                FrameBytesObject.StatusByte == 0x08 || // Terminated
                FrameBytesObject.StatusByte == 0x13 // Change of source
                )
            {

                #region PAGE1(line1)

                int index_to_be_added = Frame.Count;
                FrameBytesObject.StartAddOfCharStringMSB13 = (byte)((index_to_be_added & 0xFF00) >> 8); //MSB
                FrameBytesObject.StartAddOfCharStringLSB14 = (byte)((index_to_be_added & 0x00FF)); //LSB
                Frame[CharacterStringStartAddToBeAddedAtIndexes[0]] = FrameBytesObject.StartAddOfCharStringMSB13; // Update MSB
                Frame[CharacterStringStartAddToBeAddedAtIndexes[0] + 1] = FrameBytesObject.StartAddOfCharStringLSB14; // Update LSB

                FrameBytesObject.HorizontalOffsetMSB = 0x00;
                FrameBytesObject.HorizontalOffsetLSB = 0x20;

                FrameBytesObject.VerticalOffsetMSB = 0x00;
                FrameBytesObject.VerticalOffsetLSB = 0x01;

                Frame.Add(FrameBytesObject.StatusByte);
                Frame.Add(FrameBytesObject.HorizontalOffsetMSB); // row number
                Frame.Add(FrameBytesObject.HorizontalOffsetLSB);
                Frame.Add(FrameBytesObject.VerticalOffsetMSB); //column number
                Frame.Add(FrameBytesObject.VerticalOffsetLSB);

                Frame.AddRange(FrameBytesObject.TrainNumberBytes);
                Frame.Add(FrameBytesObject.SeparatorByte1);
                Frame.Add(FrameBytesObject.SeparatorByte2);

                Frame.Add(FrameBytesObject._2_FieldIndexMSB1);
                Frame.Add(FrameBytesObject._2_FieldIndexLSB2);
                Frame.AddRange(FrameBytesObject.Nplus5toKfIELD); // "Cancelled" or "Indefinite Late"

                Frame.Add(FrameBytesObject.TerminationByte1);
                Frame.Add(FrameBytesObject.TerminationByte2);
                #endregion

                #region PAGE2(line1)

                 index_to_be_added = Frame.Count;
                FrameBytesObject.StartAddOfCharStringMSB13 = (byte)((index_to_be_added & 0xFF00) >> 8); //MSB
                FrameBytesObject.StartAddOfCharStringLSB14 = (byte)((index_to_be_added & 0x00FF)); //LSB
                Frame[CharacterStringStartAddToBeAddedAtIndexes[1]] = FrameBytesObject.StartAddOfCharStringMSB13; // Update MSB
                Frame[CharacterStringStartAddToBeAddedAtIndexes[1] + 1] = FrameBytesObject.StartAddOfCharStringLSB14; // Update LSB

                FrameBytesObject.HorizontalOffsetMSB = 0x00;
                FrameBytesObject.HorizontalOffsetLSB = 0x20;

                FrameBytesObject.VerticalOffsetMSB = 0x00;
                FrameBytesObject.VerticalOffsetLSB = 0x01;

                Frame.Add(FrameBytesObject.StatusByte);
                Frame.Add(FrameBytesObject.HorizontalOffsetMSB); // row number
                Frame.Add(FrameBytesObject.HorizontalOffsetLSB);
                Frame.Add(FrameBytesObject.VerticalOffsetMSB); //column number
                Frame.Add(FrameBytesObject.VerticalOffsetLSB);

                Frame.AddRange(FrameBytesObject.TrainNumberBytes);
                Frame.Add(FrameBytesObject.SeparatorByte1);
                Frame.Add(FrameBytesObject.SeparatorByte2);

                Frame.Add(FrameBytesObject._2_FieldIndexMSB1);
                Frame.Add(FrameBytesObject._2_FieldIndexLSB2);
                Frame.AddRange(FrameBytesObject.StationNameBytes); // Station Name

                Frame.Add(FrameBytesObject.TerminationByte1);
                Frame.Add(FrameBytesObject.TerminationByte2);
                #endregion

                #region PAGE3(line2)

                index_to_be_added = Frame.Count;
                FrameBytesObject.StartAddOfCharStringMSB13 = (byte)((index_to_be_added & 0xFF00) >> 8); //MSB
                FrameBytesObject.StartAddOfCharStringLSB14 = (byte)((index_to_be_added & 0x00FF)); //LSB
                Frame[CharacterStringStartAddToBeAddedAtIndexes[2]] = FrameBytesObject.StartAddOfCharStringMSB13; // Update MSB
                Frame[CharacterStringStartAddToBeAddedAtIndexes[2] + 1] = FrameBytesObject.StartAddOfCharStringLSB14; // Update LSB

                FrameBytesObject.HorizontalOffsetMSB = 0x00;
                FrameBytesObject.HorizontalOffsetLSB = 0x10;

                FrameBytesObject.VerticalOffsetMSB = 0x00;
                FrameBytesObject.VerticalOffsetLSB = 0x01;

                Frame.Add(FrameBytesObject.StatusByte);
                Frame.Add(FrameBytesObject.HorizontalOffsetMSB); // row number
                Frame.Add(FrameBytesObject.HorizontalOffsetLSB);
                Frame.Add(FrameBytesObject.VerticalOffsetMSB); //column number
                Frame.Add(FrameBytesObject.VerticalOffsetLSB);

                Frame.AddRange(FrameBytesObject.TrainNameBytes);

                Frame.Add(FrameBytesObject.TerminationByte1);
                Frame.Add(FrameBytesObject.TerminationByte2);
                #endregion

            }


            else if (FrameBytesObject.StatusByte == 0x0F  //Rescheduled           
            ){

                #region PAGE1(line1)

                int index_to_be_added = Frame.Count;
                FrameBytesObject.StartAddOfCharStringMSB13 = (byte)((index_to_be_added & 0xFF00) >> 8); //MSB
                FrameBytesObject.StartAddOfCharStringLSB14 = (byte)((index_to_be_added & 0x00FF)); //LSB
                Frame[CharacterStringStartAddToBeAddedAtIndexes[0]] = FrameBytesObject.StartAddOfCharStringMSB13; // Update MSB
                Frame[CharacterStringStartAddToBeAddedAtIndexes[0] + 1] = FrameBytesObject.StartAddOfCharStringLSB14; // Update LSB

                FrameBytesObject.HorizontalOffsetMSB = 0x00;
                FrameBytesObject.HorizontalOffsetLSB = 0x20;

                FrameBytesObject.VerticalOffsetMSB = 0x00;
                FrameBytesObject.VerticalOffsetLSB = 0x01;

                Frame.Add(FrameBytesObject.StatusByte);
                Frame.Add(FrameBytesObject.HorizontalOffsetMSB); // row number
                Frame.Add(FrameBytesObject.HorizontalOffsetLSB);
                Frame.Add(FrameBytesObject.VerticalOffsetMSB); //column number
                Frame.Add(FrameBytesObject.VerticalOffsetLSB);

                Frame.AddRange(FrameBytesObject.TrainNumberBytes);
                Frame.Add(FrameBytesObject.SeparatorByte1);
                Frame.Add(FrameBytesObject.SeparatorByte2);

                Frame.Add(FrameBytesObject._2_FieldIndexMSB1);
                Frame.Add(FrameBytesObject._2_FieldIndexLSB2);
                Frame.AddRange(FrameBytesObject.Nplus5toKfIELD); // "Rescheduled"

                Frame.Add(FrameBytesObject.TerminationByte1);
                Frame.Add(FrameBytesObject.TerminationByte2);
                #endregion

                #region PAGE2(line2)

                index_to_be_added = Frame.Count;
                FrameBytesObject.StartAddOfCharStringMSB13 = (byte)((index_to_be_added & 0xFF00) >> 8); //MSB
                FrameBytesObject.StartAddOfCharStringLSB14 = (byte)((index_to_be_added & 0x00FF)); //LSB
                Frame[CharacterStringStartAddToBeAddedAtIndexes[1]] = FrameBytesObject.StartAddOfCharStringMSB13; // Update MSB
                Frame[CharacterStringStartAddToBeAddedAtIndexes[1] + 1] = FrameBytesObject.StartAddOfCharStringLSB14; // Update LSB

                FrameBytesObject.HorizontalOffsetMSB = 0x00;
                FrameBytesObject.HorizontalOffsetLSB = 0x10;

                FrameBytesObject.VerticalOffsetMSB = 0x00;
                FrameBytesObject.VerticalOffsetLSB = 0x01;

                Frame.Add(FrameBytesObject.StatusByte);
                Frame.Add(FrameBytesObject.HorizontalOffsetMSB); // row number
                Frame.Add(FrameBytesObject.HorizontalOffsetLSB);
                Frame.Add(FrameBytesObject.VerticalOffsetMSB); //column number
                Frame.Add(FrameBytesObject.VerticalOffsetLSB);

                Frame.AddRange(FrameBytesObject.TrainNumberBytes);
                Frame.Add(FrameBytesObject.SeparatorByte1);
                Frame.Add(FrameBytesObject.SeparatorByte2);

                Frame.Add(FrameBytesObject._2_FieldIndexMSB1);
                Frame.Add(FrameBytesObject._2_FieldIndexLSB2);
                Frame.AddRange(FrameBytesObject.TimeBytes);
                Frame.Add(FrameBytesObject.SeparatorByte1);
                Frame.Add(FrameBytesObject.SeparatorByte2);


                Frame.Add(FrameBytesObject._3_FieldIndexMSB1);
                Frame.Add(FrameBytesObject._3_FieldIndexLSB2);
                Frame.AddRange(FrameBytesObject.ArrivalOrDeptureBytes);
                Frame.Add(FrameBytesObject.SeparatorByte1);
                Frame.Add(FrameBytesObject.SeparatorByte2);


                Frame.Add(FrameBytesObject._4_FieldIndexMSB1);
                Frame.Add(FrameBytesObject._4_FieldIndexLSB2);
                Frame.AddRange(FrameBytesObject.PlatformNumberBytes);
                Frame.Add(FrameBytesObject.SeparatorByte1);
                Frame.Add(FrameBytesObject.SeparatorByte2);

                Frame.Add(FrameBytesObject.TerminationByte1);
                Frame.Add(FrameBytesObject.TerminationByte2);
                #endregion

                #region PAGE3(line2)

                index_to_be_added = Frame.Count;
                FrameBytesObject.StartAddOfCharStringMSB13 = (byte)((index_to_be_added & 0xFF00) >> 8); //MSB
                FrameBytesObject.StartAddOfCharStringLSB14 = (byte)((index_to_be_added & 0x00FF)); //LSB
                Frame[CharacterStringStartAddToBeAddedAtIndexes[2]] = FrameBytesObject.StartAddOfCharStringMSB13; // Update MSB
                Frame[CharacterStringStartAddToBeAddedAtIndexes[2] + 1] = FrameBytesObject.StartAddOfCharStringLSB14; // Update LSB

                FrameBytesObject.HorizontalOffsetMSB = 0x00;
                FrameBytesObject.HorizontalOffsetLSB = 0x10;

                FrameBytesObject.VerticalOffsetMSB = 0x00;
                FrameBytesObject.VerticalOffsetLSB = 0x01;

                Frame.Add(FrameBytesObject.StatusByte);
                Frame.Add(FrameBytesObject.HorizontalOffsetMSB); // row number
                Frame.Add(FrameBytesObject.HorizontalOffsetLSB);
                Frame.Add(FrameBytesObject.VerticalOffsetMSB); //column number
                Frame.Add(FrameBytesObject.VerticalOffsetLSB);

                Frame.AddRange(FrameBytesObject.TrainNameBytes);

                Frame.Add(FrameBytesObject.TerminationByte1);
                Frame.Add(FrameBytesObject.TerminationByte2);
                #endregion

            }

            else
            {
                #region PAGE1(line1)
                int index_to_be_added = Frame.Count;

                FrameBytesObject.StartAddOfCharStringMSB13 = (byte)((index_to_be_added & 0xFF00) >> 8); //MSB
                FrameBytesObject.StartAddOfCharStringLSB14 = (byte)((index_to_be_added & 0x00FF)); //LSB
                Frame[CharacterStringStartAddToBeAddedAtIndexes[0]] = FrameBytesObject.StartAddOfCharStringMSB13; // Update MSB
                Frame[CharacterStringStartAddToBeAddedAtIndexes[0] + 1] = FrameBytesObject.StartAddOfCharStringLSB14; // Update LSB

                FrameBytesObject.HorizontalOffsetMSB = 0x00;
                FrameBytesObject.HorizontalOffsetLSB = 0x20;
                FrameBytesObject.VerticalOffsetMSB = 0x00;
                FrameBytesObject.VerticalOffsetLSB = 0x01;

                Frame.Add(FrameBytesObject.StatusByte);
                Frame.Add(FrameBytesObject.HorizontalOffsetMSB); // row number
                Frame.Add(FrameBytesObject.HorizontalOffsetLSB);
                Frame.Add(FrameBytesObject.VerticalOffsetMSB); //column number
                Frame.Add(FrameBytesObject.VerticalOffsetLSB);

                Frame.AddRange(FrameBytesObject.TrainNumberBytes);
                Frame.Add(FrameBytesObject.SeparatorByte1);
                Frame.Add(FrameBytesObject.SeparatorByte2);

                Frame.Add(FrameBytesObject._2_FieldIndexMSB1);
                Frame.Add(FrameBytesObject._2_FieldIndexLSB2);
                Frame.AddRange(FrameBytesObject.TimeBytes);
                Frame.Add(FrameBytesObject.SeparatorByte1);
                Frame.Add(FrameBytesObject.SeparatorByte2);


                Frame.Add(FrameBytesObject._3_FieldIndexMSB1);
                Frame.Add(FrameBytesObject._3_FieldIndexLSB2);
                Frame.AddRange(FrameBytesObject.ArrivalOrDeptureBytes);
                Frame.Add(FrameBytesObject.SeparatorByte1);
                Frame.Add(FrameBytesObject.SeparatorByte2);

                Frame.Add(FrameBytesObject._4_FieldIndexMSB1);
                Frame.Add(FrameBytesObject._4_FieldIndexLSB2);
                Frame.AddRange(FrameBytesObject.PlatformNumberBytes);

                Frame.Add(FrameBytesObject.TerminationByte1);
                Frame.Add(FrameBytesObject.TerminationByte2);
                #endregion

                #region PAGE2(line2)

                index_to_be_added = Frame.Count;
                FrameBytesObject.StartAddOfCharStringMSB13 = (byte)((index_to_be_added & 0xFF00) >> 8); //MSB
                FrameBytesObject.StartAddOfCharStringLSB14 = (byte)((index_to_be_added & 0x00FF)); //LSB
                Frame[CharacterStringStartAddToBeAddedAtIndexes[1]] = FrameBytesObject.StartAddOfCharStringMSB13; // Update MSB
                Frame[CharacterStringStartAddToBeAddedAtIndexes[1] + 1] = FrameBytesObject.StartAddOfCharStringLSB14; // Update LSB

                FrameBytesObject.HorizontalOffsetMSB = 0x00;
                FrameBytesObject.HorizontalOffsetLSB = 0x10;

                FrameBytesObject.VerticalOffsetMSB = 0x00;
                FrameBytesObject.VerticalOffsetLSB = 0x01;

                Frame.Add(FrameBytesObject.StatusByte);
                Frame.Add(FrameBytesObject.HorizontalOffsetMSB); // row number
                Frame.Add(FrameBytesObject.HorizontalOffsetLSB);
                Frame.Add(FrameBytesObject.VerticalOffsetMSB); //column number
                Frame.Add(FrameBytesObject.VerticalOffsetLSB);

                Frame.AddRange(FrameBytesObject.TrainNameBytes);

                Frame.Add(FrameBytesObject.TerminationByte1);
                Frame.Add(FrameBytesObject.TerminationByte2);
                #endregion
            }

            Frame.Add(FrameBytesObject.Level1EndOfDataPacket);
            Frame.Add(FrameBytesObject.CRC_MSB);//NOT ADDED  
            Frame.Add(FrameBytesObject.CRC_LSB);//NOT ADDED  
            Frame.Add(FrameBytesObject.EOT);

            ////FrameBytesObject.PacketLengthMSB4
            //Frame[3] = (byte)(((Frame.Count +1 ) >> 8) & 0xFF);// Most Significant Byte

            ////FrameBytesObject.PacketLengthLSB5
            //Frame[4] = (byte)((Frame.Count + 1) & 0xFF);// Least Significant Byte

            //FrameBytesObject.PacketLengthMSB4
            Frame[3] = (byte)(((Frame.Count - 6) >> 8) & 0xFF);// Most Significant Byte

            //FrameBytesObject.PacketLengthLSB5
            Frame[4] = (byte)((Frame.Count - 6) & 0xFF);// Least Significant Byte

            //CRC Left

           

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
