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


        public TrainToBeDisplayed AddTrainToBeDisplayedtoList(ActiveTrain train)
        {
            TrainToBeDisplayed trainholder = new TrainToBeDisplayed();

            trainholder.TrainNumber = train.TrainNumber;
            trainholder.TrainName = train.TrainNameEnglish;

            trainholder.ArrivalOrDepture = train.SelectedADOption;
            trainholder.PlatformNumber = train.PFNo.ToString();
            trainholder.StatusByte = train.StatusByte;

            //THIS FIELD IS REMAINING:public byte[] SPLStatusMessage_Nplu5_K { get; set; }
           // public string StationNameForSplStatus { get; set; } // FOR PAGE 2: Diverted/Terminated at/Change of Source


            if (trainholder.ArrivalOrDepture == "A")
            {
                trainholder.Time = new DateTime(train.ETA.Value.Ticks).ToString("HH:mm");
            }
            else if (trainholder.ArrivalOrDepture == "D")
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
