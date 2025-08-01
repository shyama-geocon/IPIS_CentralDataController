using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using IpisCentralDisplayController.models;
using IpisCentralDisplayController.models.Announcement;
using IpisCentralDisplayController.models.DisplayCommunication;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace IpisCentralDisplayController.services.DisplayCommunicationServices
{
    public class FrameBuilderForMultiline
    {
        private List<byte> Frame;
        private FrameForMultiline FrameBytesObject;

        public FrameBuilderForMultiline()
        {
            Frame = new List<byte>();
            FrameBytesObject = new FrameForMultiline();
        }

        //Iterates through the entire list of active trains and 
        public TrainToBeDisplayed AddTrainToBeDisplayedtoList(ActiveTrain train)
        {
            TrainToBeDisplayed trainholder = new TrainToBeDisplayed();

            trainholder.TrainNumber = train.TrainNumber;
            trainholder.TrainName = train.TrainNameEnglish;

            trainholder.ArrivalOrDeparture = !string.IsNullOrEmpty(train.SelectedADOption) && train.SelectedADOption.Length == 1
               ? train.SelectedADOption[0]
               : throw new InvalidOperationException($"SelectedADOption '{train.SelectedADOption}' is not a valid single character.");

           // trainholder.ArrivalOrDeparture = (char)train.SelectedADOption;
            trainholder.PlatformNumber = train.PFNo.ToString();
            trainholder.StatusByte = train.StatusByte;

            //THIS FIELD IS REMAINING:public byte[] SPLStatusMessage_Nplu5_K { get; set; }
            //  public string StationNameForSplStatus { get; set; } // FOR PAGE 2: Diverted/Terminated at/Change of Source

            trainholder.StationNameForSplStatus=train.SplStationNameEnglish;

            if (trainholder.ArrivalOrDeparture == 'A')
            {
                trainholder.Time = new DateTime(train.ETA.Value.Ticks).ToString("HH:mm");
            }
            else if (trainholder.ArrivalOrDeparture == 'D')
            {
                trainholder.Time = new DateTime(train.ETD.Value.Ticks).ToString("HH:mm");
            }

            #region DID NOT NEED THIS MOST PROBABLY, WASTED WAYTOO MUCH TIME ON THIS
            ////Running right time
            //if ( (train.StatusByte == 0x01)
            //{

            //    // Fix for CS1501: No overload for method 'ToString' takes 1 arguments  
            //    // The issue occurs because 'TimeSpan' does not have a 'ToString' method that accepts a format string.  
            //    // Instead, use the 'ToString' method of 'DateTime' after converting the 'TimeSpan' to a 'DateTime'.  

            //    trainholder.Time = new DateTime(train.ETA.Value.Ticks).ToString("HH:mm");
            //   // trainholder.Time = train.ETA.ToString("HH:mm");
            //}

            ////Will arrive shortly
            //else if (train.StatusByte == 0x02)
            //{
            //    trainholder.Time = new DateTime(train.ETA.Value.Ticks).ToString("HH:mm");

            //}

            ////Is arriving on
            //else if (train.StatusByte == 0x03)
            //{
            //    trainholder.Time = new DateTime(train.ETA.Value.Ticks).ToString("HH:mm");

            //}

            ////Has arrived on 
            //else if (train.StatusByte == 0x04)
            //{



            //}

            ////Running late
            //else if (train.StatusByte == 0x05)
            //{              



            //}

            ////Cancelled || Cancelled:D
            //else if (train.StatusByte == 0x06 || train.StatusByte == 0x0B)
            //{


            //}

            ////Indefinite Late
            //else if (train.StatusByte == 0x07)
            //{

            //}

            ////Terminated At
            //else if (train.StatusByte == 0x08)
            //{


            //}

            ////Platform Changed 
            //if (train.StatusByte == 0x09 || train.StatusByte == 0x12)
            //{





            //}

            ////Running right time: DEPATURE 
            //else if (train.StatusByte == 0x0A)
            //{


            //}

            ////Is Ready To leave
            //else if (train.StatusByte == 0x0C)
            //{



            //}

            ////Is on Platform
            //else if (train.StatusByte == 0x0D)
            //{


            //}

            ////Departed
            //else if (train.StatusByte == 0x0E)
            //{


            //}

            ////Rescheduled
            //else if (train.StatusByte == 0x0F)
            //{






            //}

            ////Diverted
            //else if (train.StatusByte == 0x10)
            //{

            //}

            //// Delayed Departure
            //else if (train.StatusByte == 0x11)
            //{




            //}


            #endregion

            #region FillingTheByteArrays

            if (GetUtf16ByteSize(trainholder.TrainNumber) == 10)
            {
                trainholder.TrainNumberBytes = EncodeFixedUtf16BE(trainholder.TrainNumber, GetUtf16ByteSize(trainholder.TrainNumber));
            }
            else if (GetUtf16ByteSize(trainholder.TrainNumber) == 8)
            {
                trainholder.TrainNumberBytes = EncodeFixedUtf16BE($"0{GetUtf16ByteSize(trainholder.TrainNumber)}", 10);
            }

            trainholder.TrainNameBytes = EncodeFixedUtf16BE(trainholder.TrainName, GetUtf16ByteSize(trainholder.TrainName));

            //num of bytes must always be 10
            trainholder.TimeBytes = EncodeFixedUtf16BE(trainholder.Time, GetUtf16ByteSize(trainholder.Time));

            trainholder.ArrivalOrDepartureBytes = new byte[2];
            trainholder.ArrivalOrDepartureBytes[0] = 0x00;
            trainholder.ArrivalOrDepartureBytes[1] = (byte)trainholder.ArrivalOrDeparture; // Convert char to byte

            //  trainholder.PlatformNumberBytes = EncodeFixedUtf16BE((trainholder.PlatformNumber).ToString(), 6);

            if ((trainholder.PlatformNumber).ToString().Length == 1)
            {
                trainholder.PlatformNumberBytes = EncodeFixedUtf16BE($"00{(trainholder.PlatformNumber)}", 6);
            }
            else if ((trainholder.PlatformNumber).ToString().Length == 2)
            {
                trainholder.PlatformNumberBytes = EncodeFixedUtf16BE($"0{(trainholder.PlatformNumber)}", 6);
            }
            else if ((trainholder.PlatformNumber).ToString().Length == 3)
            {
                trainholder.PlatformNumberBytes = EncodeFixedUtf16BE((trainholder.PlatformNumber).ToString(), 6);
            }





            if (trainholder.StatusByte == 0x04)
            {
                trainholder.StatusByte = 0x04;
                trainholder.Nplus5toKfIELD = EncodeFixedUtf16BE("Arrived", GetUtf16ByteSize("Arrived"));

            }
            else if (trainholder.StatusByte == 0x06)
            {
                trainholder.StatusByte = 0x06;
                trainholder.Nplus5toKfIELD = EncodeFixedUtf16BE("Cancelled", GetUtf16ByteSize("Cancelled"));
            }
            else if (trainholder.StatusByte == 0x07)
            {
                trainholder.StatusByte = 0x07;
                trainholder.Nplus5toKfIELD = EncodeFixedUtf16BE("Indefinite Late", GetUtf16ByteSize("Indefinite Late"));
            }
            else if (trainholder.StatusByte == 0x08)
            {
                trainholder.StatusByte = 0x08;
                trainholder.Nplus5toKfIELD = EncodeFixedUtf16BE("Terminated At", GetUtf16ByteSize("Terminated At"));
                trainholder.StationNameBytes = EncodeFixedUtf16BE(trainholder.StationNameForSplStatus, GetUtf16ByteSize(trainholder.StationNameForSplStatus));
            }
            else if (trainholder.StatusByte == 0x0B)
            {
                trainholder.StatusByte = 0x0B;
                trainholder.Nplus5toKfIELD = EncodeFixedUtf16BE("Cancelled", GetUtf16ByteSize("Cancelled"));
            }
            else if (trainholder.StatusByte == 0x0F)
            {
                trainholder.StatusByte = 0x0F;
                trainholder.Nplus5toKfIELD = EncodeFixedUtf16BE("Rescheduled", GetUtf16ByteSize("Rescheduled"));

            }
            else if (trainholder.StatusByte == 0x10)
            {
                trainholder.StatusByte = 0x10;
                trainholder.Nplus5toKfIELD = EncodeFixedUtf16BE("Diverted", GetUtf16ByteSize("Diverted"));
                trainholder.StationNameBytes = EncodeFixedUtf16BE(trainholder.StationNameForSplStatus, GetUtf16ByteSize(trainholder.StationNameForSplStatus));

            }
            else if (trainholder.StatusByte == 0x13)
            {
                trainholder.StatusByte = 0x13;
                trainholder.Nplus5toKfIELD = EncodeFixedUtf16BE("Start at", GetUtf16ByteSize("Start at"));
                trainholder.StationNameBytes = EncodeFixedUtf16BE(trainholder.StationNameForSplStatus, GetUtf16ByteSize(trainholder.StationNameForSplStatus));
            }
            else
            {
                throw new Exception(message: $"Unknown status byte: {trainholder.StatusByte}");
            }



            #endregion

            return trainholder;

         }

        //public List<ByteTrainToBeDisplayed> ReadAndAddDirectFields( Device device)
        //{
        //    // NOTE: ALL THE DIRECT READ FIELDS WILL NOT NECESSARILY BE  
        //    // READ FROM THE ACTIVETRAIN INSTANCE. MANY OF THEM WILL ALSO BE  
        //    // DEPENDENT ON OTHER SETTINGS WHICH IN TURN IS BEING SET FROM SOME UI  
        //    // ELEMENT AND HENCE IS BEING STORED IN ONE OF THE FIELDS  

        //    #region ForLevel1

        //    FrameBytesObject.SourceIPAddress = IPAddress.Parse("192.168.0.253");
        //    FrameBytesObject.DestinationIPAddress = IPAddress.Parse(device.IpAddress);

        //    #endregion

        //    #region ForLevel2  
        //    // FrameBytesObject.ReverseVideo = ;  
        //    // FrameBytesObject.Speed = ;  
        //    // FrameBytesObject.EffectCode = ;  
        //    // FrameBytesObject.LetterSize = ;  
        //    // FrameBytesObject.Gap = ;  
        //    // FrameBytesObject.TimeDelay = ;  
        //    #endregion

        //    #region ForLevel3  

        //    // FrameBytesObject.StatusMessage = train.Status;
        //    FrameBytesObject.StatusByte = train.StatusByte;
        //    FrameBytesObject.StatusMessage = train.SelectedStatusOption;

        //    #endregion

        //    #region ForLevel4  
        //    //if (int.TryParse(train.TrainNumber, out int parsedTrainNumber))
        //    //{
        //    //    FrameBytesObject.TrainNumber = parsedTrainNumber;
        //    //}
        //    //else
        //    //{
        //    //    throw new InvalidOperationException($"TrainNumber '{train.TrainNumber}' cannot be converted to an integer.");
        //    //}

        //    FrameBytesObject.TrainNumber = train.TrainNumber;
        //    FrameBytesObject.TrainName = train.TrainNameEnglish;
        //    FrameBytesObject.Time = train.STA?.ToString(@"hh\:mm") ?? string.Empty; // check this properly  

        //    // Convert string to char explicitly  
        //    FrameBytesObject.ArrivalOrDepture = !string.IsNullOrEmpty(train.SelectedADOption) && train.SelectedADOption.Length == 1
        //        ? train.SelectedADOption[0]
        //        : throw new InvalidOperationException($"SelectedADOption '{train.SelectedADOption}' is not a valid single character.");

        //    FrameBytesObject.PlatformNumber = train.PFNo;

        //    //For page 2 Diverted/Terminated at/Change of Source
        //    FrameBytesObject.StationName = train.SrcNameEnglish;
        //    #endregion
        //}















        //helper method which converts the string to the specified number of bytes
        //in UTF-16 Big Endian format encoding



        public void ReadProcessAddDeviceDetails( Device device)
        {
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
            //FrameBytesObject.StatusByte = train.StatusByte;
            //FrameBytesObject.StatusMessage = train.SelectedStatusOption;
            #endregion

            #region ForLevel4  
            //FrameBytesObject.TrainNumber = train.TrainNumber;
            //FrameBytesObject.TrainName = train.TrainNameEnglish;
            //FrameBytesObject.Time = train.STA?.ToString(@"hh\:mm") ?? string.Empty; // check this properly  

            //// Convert string to char explicitly  
            //FrameBytesObject.ArrivalOrDepture = !string.IsNullOrEmpty(train.SelectedADOption) && train.SelectedADOption.Length == 1
            //    ? train.SelectedADOption[0]
            //    : throw new InvalidOperationException($"SelectedADOption '{train.SelectedADOption}' is not a valid single character.");

            //FrameBytesObject.PlatformNumber = train.PFNo;

            ////For page 2 Diverted/Terminated at/Change of Source
            //FrameBytesObject.StationName = train.SrcNameEnglish;
            #endregion


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


            //public ByteBuilder Level2Byte9 { get; set; }
            //public ByteBuilder Level2Byte10 { get; set; }
            //public ByteBuilder Level2Byte11 { get; set; }
            //public ByteBuilder Level2Byte12 { get; set; }

            #region Byte9

            FrameBytesObject.Level2Byte9= new ByteBuilder();

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
            else if(FrameBytesObject.Speed == 0x01)
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

            if (FrameBytesObject.EffectCode == 0x00)
            {
                FrameBytesObject.Level2Byte11.SetBit(5, false);
                FrameBytesObject.Level2Byte11.SetBit(4, false);
                FrameBytesObject.Level2Byte11.SetBit(3, false);

            }
            else if (FrameBytesObject.EffectCode == 0x01)
            {
                FrameBytesObject.Level2Byte11.SetBit(5, false);
                FrameBytesObject.Level2Byte11.SetBit(4, false);
                FrameBytesObject.Level2Byte11.SetBit(3, true);

            }
            else if (FrameBytesObject.EffectCode == 0x02)
            {
                FrameBytesObject.Level2Byte11.SetBit(5, false);
                FrameBytesObject.Level2Byte11.SetBit(4, true);
                FrameBytesObject.Level2Byte11.SetBit(3, false);

            }
            else if (FrameBytesObject.EffectCode == 0x03)
            {
                FrameBytesObject.Level2Byte11.SetBit(5, false);
                FrameBytesObject.Level2Byte11.SetBit(4, true);
                FrameBytesObject.Level2Byte11.SetBit(3, true);

            }
            else if (FrameBytesObject.EffectCode == 0x04)
            {
                FrameBytesObject.Level2Byte11.SetBit(5, true);
                FrameBytesObject.Level2Byte11.SetBit(4, false);
                FrameBytesObject.Level2Byte11.SetBit(3, false);

            }
            else if (FrameBytesObject.EffectCode == 0x05)
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

            FrameBytesObject.TimeDelay12 = FrameBytesObject.TimeDelay;


        }


        public void AddFixedBytes()
        {
            #region ForLevel1

            FrameBytesObject.Start1 = 0xAA;
            FrameBytesObject.Start2 = 0xCC;

            FrameBytesObject.PacketIdentifier3 = 0x05; //Packet for MLDBMono

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

            FrameBytesObject.WindowRightColumn3 = 0x01; //MSB
            FrameBytesObject.WindowRightColumn4 = 0x50; //LSB

            FrameBytesObject.WindowTopRow5 = 0x01; //MSB
            FrameBytesObject.WindowTopRow6 = 0x50; //LSB

            FrameBytesObject.WindowBottomRow7 = 0x00; //MSB
            //FrameBytesObject.WindowBottomRow8 = 0x10; //LSB
            FrameBytesObject.WindowBottomRow8 = 0x01; //LSB

            //public ByteBuilder Level2Byte9 { get; set; }
            //public ByteBuilder Level2Byte10 { get; set; }
            //public ByteBuilder Level2Byte11 { get; set; }
            //public ByteBuilder Level2Byte12 { get; set; }

            //Repeat 1 - 14 for each data page 

            FrameBytesObject.TerminationByte1 = 0xFF;
            FrameBytesObject.TerminationByte2 = 0xFF;

            // DATA(character string) STUFF GOES HERE

            FrameBytesObject.CharacterStringTerminationByte1 = 0xFF; //always 0xFF
            FrameBytesObject.CharacterStringTerminationByte2 = 0xFF; //always 0xFF

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
