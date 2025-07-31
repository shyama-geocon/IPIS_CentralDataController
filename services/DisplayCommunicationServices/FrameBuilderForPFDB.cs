using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;
using IpisCentralDisplayController.models;
using IpisCentralDisplayController.models.DisplayCommunication;
using SharpCompress.Compressors.Xz;

namespace IpisCentralDisplayController.services.DisplayCommunicationServices
{
    public class FrameBuilderForPFDB
    {
        //Making byte LIST right now, but we can also 
        //use byte array eventually if you make a
        //separate method to calculate the size of the byte
        //frame based on certain parameters

        // should this be a list or an observablecollection ?
        private List<byte> Frame;
        private FrameForPFDB FrameBytesObject;

        public FrameBuilderForPFDB()
        {
            Frame = new List<byte>();
            FrameBytesObject = new FrameForPFDB();
        }

        public void ReadAndAddDirectFields(ActiveTrain train, Device device)
        {
            // NOTE: ALL THE DIRECT READ FIELDS WILL NOT NECESSARILY BE  
            // READ FROM THE ACTIVETRAIN INSTANCE. MANY OF THEM WILL ALSO BE  
            // DEPENDENT ON OTHER SETTINGS WHICH IN TURN IS BEING SET FROM SOME UI  
            // ELEMENT AND HENCE IS BEING STORED IN ONE OF THE FIELDS  

            #region ForLevel1

            FrameBytesObject.SourceIPAddress = IPAddress.Parse("192.168.0.253");
            FrameBytesObject.DestinationIPAddress = IPAddress.Parse(device.IpAddress) ;

            #endregion

            #region ForLevel2  
            // FrameBytesObject.ReverseVideo = ;  
            // FrameBytesObject.Speed = ;  
            // FrameBytesObject.EffectCode = ;  
            // FrameBytesObject.LetterSize = ;  
            // FrameBytesObject.Gap = ;  
            // FrameBytesObject.TimeDelay = ;  
            #endregion

            #region ForLevel3  

            // FrameBytesObject.StatusMessage = train.Status;
            FrameBytesObject.StatusByte = train.StatusByte;
            FrameBytesObject.StatusMessage = train.SelectedStatusOption;

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
            FrameBytesObject.Time = train.STA?.ToString(@"hh\:mm") ?? string.Empty; // check this properly  

            // Convert string to char explicitly  
            FrameBytesObject.ArrivalOrDepture = !string.IsNullOrEmpty(train.SelectedADOption) && train.SelectedADOption.Length == 1
                ? train.SelectedADOption[0]
                : throw new InvalidOperationException($"SelectedADOption '{train.SelectedADOption}' is not a valid single character.");

            FrameBytesObject.PlatformNumber = train.PFNo;

            //For page 2 Diverted/Terminated at/Change of Source
            FrameBytesObject.StationName = train.SrcNameEnglish;
            #endregion
        }

        public void ProcessDataFromReadFields()
        {
            //Process the data from the read fields and convert them to byte arrays
            //FrameBytesObject.TrainNameBytes = Encoding.ASCII.GetBytes(FrameBytesObject.TrainName);
            //FrameBytesObject.TimeBytes = Encoding.ASCII.GetBytes(FrameBytesObject.Time);
            //FrameBytesObject.ArrivalOrDeptureBytes = new byte[] { (byte)FrameBytesObject.ArrivalOrDepture };
            //FrameBytesObject.PlatformNumberBytes = BitConverter.GetBytes(FrameBytesObject.PlatformNumber);

            //FrameBytesObject.TrainNumberBytes= BitConverter.GetBytes(FrameBytesObject.TrainNumber);
            FrameBytesObject.TrainNumberBytes = EncodeFixedUtf16BE(FrameBytesObject.TrainNumber, GetUtf16ByteSize(FrameBytesObject.TrainNumber));
            FrameBytesObject.TrainNameBytes = EncodeFixedUtf16BE(FrameBytesObject.TrainName, GetUtf16ByteSize(FrameBytesObject.TrainName));
            //comeback Time
            FrameBytesObject.Time = "00:00";
            FrameBytesObject.TimeBytes = EncodeFixedUtf16BE(FrameBytesObject.Time, GetUtf16ByteSize(FrameBytesObject.Time));
            FrameBytesObject.ArrivalOrDeptureBytes = new byte[2] ;
            FrameBytesObject.ArrivalOrDeptureBytes[0] =0x00;
            FrameBytesObject.ArrivalOrDeptureBytes[1] = (byte)FrameBytesObject.ArrivalOrDepture; // Convert char to byte

            //byte[] platformNumberBytes = new byte[3];
            //platformNumberBytes[0] = (byte)(FrameBytesObject.PlatformNumber & 0x00FF0000);                            // MSB
            //platformNumberBytes[1] = (byte)(FrameBytesObject.PlatformNumber & 0x0000FF00);
            //platformNumberBytes[2] = (byte)(FrameBytesObject.PlatformNumber & 0xFF);  // LSB

            FrameBytesObject.PlatformNumberBytes = EncodeFixedUtf16BE((FrameBytesObject.PlatformNumber).ToString(), 6);
            //  BitConverter.GetBytes(FrameBytesObject.PlatformNumber);


                 if (FrameBytesObject.StatusByte == 0x04)
                {
                    FrameBytesObject.StatusByte = 0x04;
                    FrameBytesObject.Nplus5toKfIELD = EncodeFixedUtf16BE("Arrived", GetUtf16ByteSize("Arrived"));

                }
                else if (FrameBytesObject.StatusByte == 0x06)
                {
                    FrameBytesObject.StatusByte = 0x06;
                    FrameBytesObject.Nplus5toKfIELD = EncodeFixedUtf16BE("Cancelled", GetUtf16ByteSize("Cancelled"));
                }
                else if (FrameBytesObject.StatusByte == 0x07)
                {
                    FrameBytesObject.StatusByte = 0x07;
                    FrameBytesObject.Nplus5toKfIELD = EncodeFixedUtf16BE("Indefinite Late", GetUtf16ByteSize("Indefinite Late"));
                }
                else if (FrameBytesObject.StatusByte == 0x08)
                {
                    FrameBytesObject.StatusByte = 0x08;
                    FrameBytesObject.Nplus5toKfIELD = EncodeFixedUtf16BE("Terminated At", GetUtf16ByteSize("Terminated At"));
                    FrameBytesObject.StationNameBytes = EncodeFixedUtf16BE(FrameBytesObject.StationName, GetUtf16ByteSize(FrameBytesObject.StationName));
                }              
                else if (FrameBytesObject.StatusByte == 0x0B )
                {
                    FrameBytesObject.StatusByte = 0x0B;
                    FrameBytesObject.Nplus5toKfIELD = EncodeFixedUtf16BE("Cancelled", GetUtf16ByteSize("Cancelled"));
                }
              
                else if (FrameBytesObject.StatusByte == 0x0F)
                {
                    FrameBytesObject.StatusByte = 0x0F;
                    FrameBytesObject.Nplus5toKfIELD = EncodeFixedUtf16BE("Rescheduled", GetUtf16ByteSize("Rescheduled"));

                }
                else if (FrameBytesObject.StatusByte == 0x10)
                {
                    FrameBytesObject.StatusByte = 0x10;
                    FrameBytesObject.Nplus5toKfIELD = EncodeFixedUtf16BE("Diverted", GetUtf16ByteSize("Diverted"));
                    FrameBytesObject.StationNameBytes = EncodeFixedUtf16BE(FrameBytesObject.StationName, GetUtf16ByteSize(FrameBytesObject.StationName));

                }
              
                else if (FrameBytesObject.StatusByte == 0x13)
                {
                    FrameBytesObject.StatusByte = 0x13;
                    FrameBytesObject.Nplus5toKfIELD = EncodeFixedUtf16BE("Start at", GetUtf16ByteSize("Start at"));
                    FrameBytesObject.StationNameBytes = EncodeFixedUtf16BE(FrameBytesObject.StationName, GetUtf16ByteSize(FrameBytesObject.StationName));
                }
                else
                {
                    throw new Exception(message: $"Unknown status message: {FrameBytesObject.StatusMessage}");
                }

            







            //if (FrameBytesObject.ArrivalOrDepture== 'A') {
            //    if (FrameBytesObject.StatusMessage == "Running Right Time")
            //    {
            //        FrameBytesObject.StatusByte = 0x01;
            //    }
            //    else if (FrameBytesObject.StatusMessage == "Will Arrive Shortly")
            //    {
            //        FrameBytesObject.StatusByte = 0x02;
            //    }
            //    else if (FrameBytesObject.StatusMessage == "Is Arriving On")
            //    {
            //        FrameBytesObject.StatusByte = 0x03;
            //    }
            //    else if (FrameBytesObject.StatusMessage == "Has Arrived On")
            //    {
            //        FrameBytesObject.StatusByte = 0x04;
            //        FrameBytesObject.Nplus5toKfIELD = EncodeFixedUtf16BE("Arrived", GetUtf16ByteSize("Arrived"));

            //    }
            //    else if (FrameBytesObject.StatusMessage == "Running Late")
            //    {
            //        FrameBytesObject.StatusByte = 0x05;
            //    }
            //    else if (FrameBytesObject.StatusMessage == "Cancelled")
            //    {
            //        FrameBytesObject.StatusByte = 0x06;
            //        FrameBytesObject.Nplus5toKfIELD = EncodeFixedUtf16BE("Cancelled", GetUtf16ByteSize("Cancelled"));
            //    }
            //    else if (FrameBytesObject.StatusMessage == "Indefinite Late")
            //    {
            //        FrameBytesObject.StatusByte = 0x07;
            //        FrameBytesObject.Nplus5toKfIELD = EncodeFixedUtf16BE("Indefinite Late", GetUtf16ByteSize("Indefinite Late"));
            //    }
            //    else if (FrameBytesObject.StatusMessage == "Terminated At")
            //    {
            //        FrameBytesObject.StatusByte = 0x08;
            //        FrameBytesObject.Nplus5toKfIELD = EncodeFixedUtf16BE("Terminated At", GetUtf16ByteSize("Terminated At"));
            //        FrameBytesObject.StationNameBytes = EncodeFixedUtf16BE(FrameBytesObject.StationName, GetUtf16ByteSize(FrameBytesObject.StationName));
            //    }
            //    else if (FrameBytesObject.StatusMessage == "Platform Changed")
            //    {
            //        FrameBytesObject.StatusByte = 0x09;
            //    }
            //    else 
            //    {
            //        throw new Exception(message: $"Unknown status message: {FrameBytesObject.StatusMessage}");
            //    }
            //}
            //else if(FrameBytesObject.ArrivalOrDepture == 'D')
            //{
            //    if (FrameBytesObject.StatusMessage == "Running Right Time")
            //    {
            //        FrameBytesObject.StatusByte = 0x0A;
            //    }
            //    else if (FrameBytesObject.StatusMessage == "Cancelled")
            //    {
            //        FrameBytesObject.StatusByte = 0x0B;
            //        FrameBytesObject.Nplus5toKfIELD = EncodeFixedUtf16BE("Cancelled", GetUtf16ByteSize("Cancelled"));
            //    }
            //    else if (FrameBytesObject.StatusMessage == "Is Ready To Leave")
            //    {
            //        FrameBytesObject.StatusByte = 0x0C;
            //    }
            //    else if (FrameBytesObject.StatusMessage == "Is On Platform")
            //    {
            //        FrameBytesObject.StatusByte = 0x0D;
            //    }
            //    else if (FrameBytesObject.StatusMessage == "Departed")
            //    {
            //        FrameBytesObject.StatusByte = 0x0E;
            //    }
            //    else if (FrameBytesObject.StatusMessage == "Rescheduled")
            //    {
            //        FrameBytesObject.StatusByte = 0x0F;
            //        FrameBytesObject.Nplus5toKfIELD = EncodeFixedUtf16BE("Rescheduled", GetUtf16ByteSize("Rescheduled"));

            //    }
            //    else if (FrameBytesObject.StatusMessage == "Diverted")
            //    {
            //        FrameBytesObject.StatusByte = 0x10;
            //        FrameBytesObject.Nplus5toKfIELD = EncodeFixedUtf16BE("Diverted", GetUtf16ByteSize("Diverted"));
            //        FrameBytesObject.StationNameBytes = EncodeFixedUtf16BE(FrameBytesObject.StationName, GetUtf16ByteSize(FrameBytesObject.StationName));

            //    }
            //    else if (FrameBytesObject.StatusMessage == "Delayed")
            //    {
            //        FrameBytesObject.StatusByte = 0x11;
            //    }
            //    else if (FrameBytesObject.StatusMessage == "Platform Changed")
            //    {
            //        FrameBytesObject.StatusByte = 0x12;
            //    }
            //    else if (FrameBytesObject.StatusMessage == "Change Of Source")
            //    {
            //        FrameBytesObject.StatusByte = 0x13;
            //        FrameBytesObject.Nplus5toKfIELD = EncodeFixedUtf16BE("Start at", GetUtf16ByteSize("Start at"));
            //        FrameBytesObject.StationNameBytes = EncodeFixedUtf16BE(FrameBytesObject.StationName, GetUtf16ByteSize(FrameBytesObject.StationName));
            //    }
            //    else
            //    {
            //        throw new Exception(message: $"Unknown status message: {FrameBytesObject.StatusMessage}");
            //    }

            //}

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

            //// Add separator bytes
        }

        public void AddFixedBytes()
        {
            #region ForLevel1

            FrameBytesObject.Start1 = 0xAA; 
            FrameBytesObject.Start2 = 0xCC;

            FrameBytesObject.PacketIdentifier3 = 0x03; //Packet for PFDB

            //public byte PacketLengthMSB4 { get; set; }
            //public byte PacketLengthLSB5 { get; set; }

            //public byte SerialNumber10 { get; set; }

            FrameBytesObject.PacketType11 = 0x81; //Data Transfer(Data)

            FrameBytesObject.StartOfDataPacketIndicator12 = 0x02;

            // DATA STUFF GOES HERE

            FrameBytesObject.Level1EndOfDataPacket = 0xAA;

            //FrameBytesObject.CRC_MSB = 0xCC;
            //FrameBytesObject.CRC_LSB = 0xCC;

            FrameBytesObject.EOT = 0xCC;

            #endregion


            #region ForLevel2

            FrameBytesObject.WindowLeftColumn1 = 0x00; //MSB
            FrameBytesObject.WindowLeftColumn2 = 0x01; //LSB

            FrameBytesObject.WindowRightColumn3 = 0x01; //MSB
            FrameBytesObject.WindowRightColumn4 = 0x50; //LSB

            FrameBytesObject.WindowTopRow5 = 0x01; //MSB
            FrameBytesObject.WindowTopRow6 = 0x50; //LSB

            FrameBytesObject.WindowBottomRow7 = 0x00; //MSB
            FrameBytesObject.WindowBottomRow8 = 0x10; //LSB

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
            FrameBytesObject.HorizontalOffsetLSB = 0X01;
            FrameBytesObject.VerticalOffsetMSB = 0X00;
            FrameBytesObject.VerticalOffsetLSB = 0X01;

            #endregion

            #region ForLevel4
            FrameBytesObject.SeparatorByte1 = 0xE7; // Separator byte 1
            FrameBytesObject.SeparatorByte2 = 0x00; // Separator byte 2

            FrameBytesObject.TrainNameFieldIndexMSB1 = 0x00;
            FrameBytesObject.TrainNameFieldIndexLSB2 = 58;

            FrameBytesObject.TimeFieldIndexMSB1 = 0x00;
            FrameBytesObject.TimeFieldIndexLSB2 = 0xFD;

            FrameBytesObject.ArrivalOrDeptureFieldIndexMSB1 = 0x01;
            FrameBytesObject.ArrivalOrDeptureFieldIndexLSB2 = 0x2D;

            FrameBytesObject.PlatformNumberFieldIndexMSB1 = 0x01;
            FrameBytesObject.PlatformNumberFieldIndexLSB2 = 0x3A;

            #endregion



        }

        public byte[] CompileFrame()
        {
            Frame.Clear();
                

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

            //////////////////
            //for double page
            /////////////////
            if ((FrameBytesObject.StatusByte == 0x08) ||
                (FrameBytesObject.StatusByte == 0x10) ||
                (FrameBytesObject.StatusByte == 0x13) ||
                (FrameBytesObject.StatusByte == 0x0F) 
            ){

                #region LEVEL2 CONSTRUCTION  

                //FIRST DATA PAGE
                #region
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
                Frame.Add(FrameBytesObject.Level2Byte12.ToByte());

                Frame.Add(FrameBytesObject.StartAddOfCharStringMSB13);//NOT ADDED
                Frame.Add(FrameBytesObject.StartAddOfCharStringLSB14);//NOT ADDED

                #endregion

                //IF MORE THAN 1 DATA PAGE, THEN IT GOES HERE
                //there is more than 1 data page, so we are adding it here
                #region
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
                Frame.Add(FrameBytesObject.Level2Byte12.ToByte());

                Frame.Add(FrameBytesObject.StartAddOfCharStringMSB13);//NOT ADDED
                Frame.Add(FrameBytesObject.StartAddOfCharStringLSB14);//NOT ADDED
                #endregion
                //END OF DATA PAGE 2

                Frame.Add(FrameBytesObject.TerminationByte1);
                Frame.Add(FrameBytesObject.TerminationByte2);

                #region LEVEL3 CONSTRUCTION  

                //FIRST DATA PAGE STARTS
                #region
                Frame.Add(FrameBytesObject.StatusByte);
                Frame.Add(FrameBytesObject.HorizontalOffsetMSB);
                Frame.Add(FrameBytesObject.HorizontalOffsetLSB);
                Frame.Add(FrameBytesObject.VerticalOffsetMSB);
                Frame.Add(FrameBytesObject.VerticalOffsetLSB);

                #region LEVEL4 CONSTRUCTION 

                if ((FrameBytesObject.StatusByte == 0x08) || //Terminated at
                    (FrameBytesObject.StatusByte == 0x10) || //Diverted
                    (FrameBytesObject.StatusByte == 0x13)    //Change of Source
                    ){

                    Frame.AddRange(FrameBytesObject.TrainNumberBytes); // Use AddRange to add byte[]  
                    Frame.Add(FrameBytesObject.SeparatorByte1);
                    Frame.Add(FrameBytesObject.SeparatorByte2);

                    Frame.Add(FrameBytesObject.TrainNameFieldIndexMSB1);
                    Frame.Add(FrameBytesObject.TrainNameFieldIndexLSB2);
                    Frame.AddRange(FrameBytesObject.TrainNameBytes);
                    Frame.Add(FrameBytesObject.SeparatorByte1);
                    Frame.Add(FrameBytesObject.SeparatorByte2);

                    Frame.Add(FrameBytesObject.TimeFieldIndexMSB1);
                    Frame.Add(FrameBytesObject.TimeFieldIndexLSB2);

                    Frame.AddRange(FrameBytesObject.Nplus5toKfIELD);

                }


                else if (FrameBytesObject.StatusByte == 0x0F) //Rescheduled
                {
                    Frame.AddRange(FrameBytesObject.TrainNumberBytes); // Use AddRange to add byte[]  
                    Frame.Add(FrameBytesObject.SeparatorByte1);
                    Frame.Add(FrameBytesObject.SeparatorByte2);

                    Frame.Add(FrameBytesObject.TrainNameFieldIndexMSB1);
                    Frame.Add(FrameBytesObject.TrainNameFieldIndexLSB2);
                    Frame.AddRange(FrameBytesObject.TrainNameBytes);
                    Frame.Add(FrameBytesObject.SeparatorByte1);
                    Frame.Add(FrameBytesObject.SeparatorByte2);

                    Frame.Add(FrameBytesObject.TimeFieldIndexMSB1);
                    Frame.Add(FrameBytesObject.TimeFieldIndexLSB2);

                    Frame.AddRange(FrameBytesObject.Nplus5toKfIELD);

                }



                #endregion



                //Actually part of level3 but added here since
                //we have more than 1 data page
                Frame.Add(FrameBytesObject.CharacterStringTerminationByte1);
                Frame.Add(FrameBytesObject.CharacterStringTerminationByte2);
                #endregion
                //END OF DATA PAGE 1


                //SECOND DATA PAGE STARTS
                #region
                Frame.Add(FrameBytesObject.StatusByte);
                Frame.Add(FrameBytesObject.HorizontalOffsetMSB);
                Frame.Add(FrameBytesObject.HorizontalOffsetLSB);
                Frame.Add(FrameBytesObject.VerticalOffsetMSB);
                Frame.Add(FrameBytesObject.VerticalOffsetLSB);

                #region LEVEL4 CONSTRUCTION 


                if ((FrameBytesObject.StatusByte == 0x08) || //Terminated at
                    (FrameBytesObject.StatusByte == 0x10) || //Diverted
                    (FrameBytesObject.StatusByte == 0x13)    //Change of Source
                    )
                {

                    Frame.AddRange(FrameBytesObject.TrainNumberBytes); // Use AddRange to add byte[]  
                    Frame.Add(FrameBytesObject.SeparatorByte1);
                    Frame.Add(FrameBytesObject.SeparatorByte2);

                    Frame.Add(FrameBytesObject.TrainNameFieldIndexMSB1);
                    Frame.Add(FrameBytesObject.TrainNameFieldIndexLSB2);
                    Frame.AddRange(FrameBytesObject.TrainNameBytes);
                    Frame.Add(FrameBytesObject.SeparatorByte1);
                    Frame.Add(FrameBytesObject.SeparatorByte2);

                    Frame.Add(FrameBytesObject.TimeFieldIndexMSB1);
                    Frame.Add(FrameBytesObject.TimeFieldIndexLSB2);

                    Frame.AddRange(FrameBytesObject.StationNameBytes);

                }


                else if (FrameBytesObject.StatusByte == 0x0F) //Rescheduled
                {
                    Frame.AddRange(FrameBytesObject.TrainNumberBytes); // Use AddRange to add byte[]  
                    Frame.Add(FrameBytesObject.SeparatorByte1);
                    Frame.Add(FrameBytesObject.SeparatorByte2);

                    Frame.Add(FrameBytesObject.TrainNameFieldIndexMSB1);
                    Frame.Add(FrameBytesObject.TrainNameFieldIndexLSB2);
                    Frame.AddRange(FrameBytesObject.TrainNameBytes);
                    Frame.Add(FrameBytesObject.SeparatorByte1);
                    Frame.Add(FrameBytesObject.SeparatorByte2);

                    Frame.Add(FrameBytesObject.TimeFieldIndexMSB1);
                    Frame.Add(FrameBytesObject.TimeFieldIndexLSB2);
                    Frame.AddRange(FrameBytesObject.TimeBytes);
                    Frame.Add(FrameBytesObject.SeparatorByte1);
                    Frame.Add(FrameBytesObject.SeparatorByte2);

                    Frame.Add(FrameBytesObject.ArrivalOrDeptureFieldIndexMSB1);
                    Frame.Add(FrameBytesObject.ArrivalOrDeptureFieldIndexLSB2);
                    Frame.AddRange(FrameBytesObject.ArrivalOrDeptureBytes);
                    Frame.Add(FrameBytesObject.SeparatorByte1);
                    Frame.Add(FrameBytesObject.SeparatorByte2);

                    Frame.Add(FrameBytesObject.PlatformNumberFieldIndexMSB1);
                    Frame.Add(FrameBytesObject.PlatformNumberFieldIndexLSB2);
                    Frame.AddRange(FrameBytesObject.PlatformNumberBytes);

                }
              

                #endregion

                //Actually part of level3 but added here since
                //we have more than 1 data page
                Frame.Add(FrameBytesObject.CharacterStringTerminationByte1);
                Frame.Add(FrameBytesObject.CharacterStringTerminationByte2);


                #endregion
                //END OF DATA PAGE 2

                #endregion



                #endregion


            }





            //////////////////
            //for single page
            /////////////////
            else
            {

                #region LEVEL2 CONSTRUCTION  
                Frame.Add(FrameBytesObject.WindowLeftColumn1);
                Frame.Add(FrameBytesObject.WindowLeftColumn2);
                Frame.Add(FrameBytesObject.WindowRightColumn3);
                Frame.Add(FrameBytesObject.WindowRightColumn4);
                Frame.Add(FrameBytesObject.WindowTopRow5);
                Frame.Add(FrameBytesObject.WindowTopRow6);
                Frame.Add(FrameBytesObject.WindowBottomRow7);
                Frame.Add(FrameBytesObject.WindowBottomRow8);

                //V.IMP : REMOVE THIS LATER PLEASE
                FrameBytesObject.Level2Byte9 = new ByteBuilder();
                FrameBytesObject.Level2Byte10 = new ByteBuilder();
                FrameBytesObject.Level2Byte11 = new ByteBuilder();
                FrameBytesObject.Level2Byte12 = new ByteBuilder();


                Frame.Add(FrameBytesObject.Level2Byte9.ToByte());
                Frame.Add(FrameBytesObject.Level2Byte10.ToByte());
                Frame.Add(FrameBytesObject.Level2Byte11.ToByte());
                Frame.Add(FrameBytesObject.Level2Byte12.ToByte());
                Frame.Add(FrameBytesObject.StartAddOfCharStringMSB13);
                Frame.Add(FrameBytesObject.StartAddOfCharStringLSB14);
                //IF MORE THAN 1 DATA PAGE, THEN IT GOES HERE
                Frame.Add(FrameBytesObject.TerminationByte1);
                Frame.Add(FrameBytesObject.TerminationByte2);


                #region LEVEL3 CONSTRUCTION  
                Frame.Add(FrameBytesObject.StatusByte);
                Frame.Add(FrameBytesObject.HorizontalOffsetMSB);
                Frame.Add(FrameBytesObject.HorizontalOffsetLSB);
                Frame.Add(FrameBytesObject.VerticalOffsetMSB);
                Frame.Add(FrameBytesObject.VerticalOffsetLSB);

                #region LEVEL4 CONSTRUCTION 

                if ((FrameBytesObject.StatusByte == 0x01) || //Running Right Time 
                    (FrameBytesObject.StatusByte == 0x02) || //Will Arrive Shortly 
                    (FrameBytesObject.StatusByte == 0x03) || //Is Arriving On
                    (FrameBytesObject.StatusByte == 0x05) || //Running Late 
                    (FrameBytesObject.StatusByte == 0x09) || //Platform Changed 
                    (FrameBytesObject.StatusByte == 0x0A) || //Running Right Time 
                    (FrameBytesObject.StatusByte == 0x0C) || //Is Ready to Leave 
                    (FrameBytesObject.StatusByte == 0x0D) || //Is on Platform
                    (FrameBytesObject.StatusByte == 0x0E) || //Departed
                    (FrameBytesObject.StatusByte == 0x11) || // Delay Departure 
                    (FrameBytesObject.StatusByte == 0x12)  //Departed
                    )
                {
                    //PERFORM VALIDATION FOR ALL FIELDS, V.IMP
                    //if (FrameBytesObject.TrainNumberBytes != null)
                    //{
                    //    Frame.AddRange(FrameBytesObject.TrainNumberBytes);
                    //}
                    //else
                    //{
                    //    // Handle the null case appropriately  
                    //    throw new InvalidOperationException("TrainNumberBytes is null.");
                    //}

                    Frame.AddRange(FrameBytesObject.TrainNumberBytes); // Use AddRange to add byte[]  
                    Frame.Add(FrameBytesObject.SeparatorByte1);
                    Frame.Add(FrameBytesObject.SeparatorByte2);

                    Frame.Add(FrameBytesObject.TrainNameFieldIndexMSB1);
                    Frame.Add(FrameBytesObject.TrainNameFieldIndexLSB2);
                    Frame.AddRange(FrameBytesObject.TrainNameBytes);
                    Frame.Add(FrameBytesObject.SeparatorByte1);
                    Frame.Add(FrameBytesObject.SeparatorByte2);

                    Frame.Add(FrameBytesObject.TimeFieldIndexMSB1);
                    Frame.Add(FrameBytesObject.TimeFieldIndexLSB2);

                    Frame.AddRange(FrameBytesObject.TimeBytes);
                    Frame.Add(FrameBytesObject.SeparatorByte1);
                    Frame.Add(FrameBytesObject.SeparatorByte2);

                    Frame.Add(FrameBytesObject.ArrivalOrDeptureFieldIndexMSB1);
                    Frame.Add(FrameBytesObject.ArrivalOrDeptureFieldIndexLSB2);
                    Frame.AddRange(FrameBytesObject.ArrivalOrDeptureBytes);
                    Frame.Add(FrameBytesObject.SeparatorByte1);
                    Frame.Add(FrameBytesObject.SeparatorByte2);

                    Frame.Add(FrameBytesObject.PlatformNumberFieldIndexMSB1);
                    Frame.Add(FrameBytesObject.PlatformNumberFieldIndexLSB2);
                    Frame.AddRange(FrameBytesObject.PlatformNumberBytes);

                }

                else if ((FrameBytesObject.StatusByte == 0x07) || //Indefinite Late
                    (FrameBytesObject.StatusByte == 0x06)  ||//Cancelled 
                    (FrameBytesObject.StatusByte == 0x0B)  //Cancelled 
                )
                {
                    Frame.AddRange(FrameBytesObject.TrainNumberBytes); // Use AddRange to add byte[]  
                    Frame.Add(FrameBytesObject.SeparatorByte1);
                    Frame.Add(FrameBytesObject.SeparatorByte2);

                    Frame.Add(FrameBytesObject.TrainNameFieldIndexMSB1);
                    Frame.Add(FrameBytesObject.TrainNameFieldIndexLSB2);
                    Frame.AddRange(FrameBytesObject.TrainNameBytes);
                    Frame.Add(FrameBytesObject.SeparatorByte1);
                    Frame.Add(FrameBytesObject.SeparatorByte2);

                    Frame.Add(FrameBytesObject.TimeFieldIndexMSB1);
                    Frame.Add(FrameBytesObject.TimeFieldIndexLSB2);
                   
                    Frame.AddRange(FrameBytesObject.Nplus5toKfIELD);

                }


                else if ((FrameBytesObject.StatusByte == 0x04) //Arrived
               )
                {
                    Frame.AddRange(FrameBytesObject.TrainNumberBytes); // Use AddRange to add byte[]  
                    Frame.Add(FrameBytesObject.SeparatorByte1);
                    Frame.Add(FrameBytesObject.SeparatorByte2);

                    Frame.Add(FrameBytesObject.TrainNameFieldIndexMSB1);
                    Frame.Add(FrameBytesObject.TrainNameFieldIndexLSB2);
                    Frame.AddRange(FrameBytesObject.TrainNameBytes);
                    Frame.Add(FrameBytesObject.SeparatorByte1);
                    Frame.Add(FrameBytesObject.SeparatorByte2);

                    Frame.Add(FrameBytesObject.TimeFieldIndexMSB1);
                    Frame.Add(FrameBytesObject.TimeFieldIndexLSB2);

                    Frame.AddRange(FrameBytesObject.Nplus5toKfIELD);

                    Frame.Add(FrameBytesObject.SeparatorByte1);
                    Frame.Add(FrameBytesObject.SeparatorByte2);

                    Frame.Add(FrameBytesObject.PlatformNumberFieldIndexMSB1);
                    Frame.Add(FrameBytesObject.PlatformNumberFieldIndexLSB2);
                    Frame.AddRange(FrameBytesObject.PlatformNumberBytes);

                }


                #endregion
                #endregion

                Frame.Add(FrameBytesObject.CharacterStringTerminationByte1);
                Frame.Add(FrameBytesObject.CharacterStringTerminationByte2);

                #endregion
            }


            Frame.Add(FrameBytesObject.Level1EndOfDataPacket);
            Frame.Add(FrameBytesObject.CRC_MSB);//NOT ADDED  
            Frame.Add(FrameBytesObject.CRC_LSB);//NOT ADDED  
            Frame.Add(FrameBytesObject.EOT);

            //FrameBytesObject.PacketLengthMSB4
            Frame[3] = (byte)(((Frame.Count +1 ) >> 8) & 0xFF);// Most Significant Byte

            //FrameBytesObject.PacketLengthLSB5
            Frame[4] = (byte)((Frame.Count + 1) & 0xFF);// Least Significant Byte

            //CRC Left

            #endregion

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




//public static byte[] ComputeCrc16CCITT(byte[] data)
//{
//    ushort crc = 0xFFFF;
//    foreach (byte b in data)
//    {
//        crc ^= (ushort)(b << 8);
//        for (int i = 0; i < 8; i++)
//        {
//            if ((crc & 0x8000) != 0)
//                crc = (ushort)((crc << 1) ^ 0x1021);
//            else
//                crc <<= 1;
//        }
//    }

//    // Convert CRC to two bytes (big-endian: MSB first)
//    byte crcMsb = (byte)((crc >> 8) & 0xFF);
//    byte crcLsb = (byte)(crc & 0xFF);

//    // Create a new array with original data + 2 CRC bytes
//    byte[] result = new byte[data.Length + 2];
//    Buffer.BlockCopy(data, 0, result, 0, data.Length);
//    result[data.Length] = crcMsb;
//    result[data.Length + 1] = crcLsb;

//    return result;
//}


//I've the data in the form of a byte array. I want to append 2 bytes of CRC. CRC-16-CCITT (also known as CRC-CCITT) 
//is used for data integrity. The polynomial of 
//CRC-16 is “x16+x12+x5+1” and its hex value 
//is 1021

//This should be the function prototype: public static byte[] ComputeCrc16CCITT(byte[] data)































//public byte[] AddStartBytes()
//{
//    Frame.Add(FrameBytesObject.Start1); // Start byte 1
//    Frame.Add(FrameBytesObject.Start2); // Start byte 2
//    Frame.Add(FrameBytesObject.PacketIdentifier3); // Packet identifier byte

//    //THIS IS A BAD WORK AROUD FOR NOW, CHANGE THIS PLEASE
//    byte[] result = Frame.ToArray();
//    return result;
//}

//public void AssignBytes()
//{
//    //We aren't really assigning ALLLL the bytes in this methd
//    //because many if the fixed bytes are already
//    //initialized in the object constructior of FrameBytesObject




//}