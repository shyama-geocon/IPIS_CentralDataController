using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using IpisCentralDisplayController.models;
using IpisCentralDisplayController.models.DisplayCommunication;
using IpisCentralDisplayController.models.DisplayConfiguration;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace IpisCentralDisplayController.services.DisplayConfigurationServices
{
    public class FrameBuilderForColourConfig
    {
        private FrameColourConfiguration FrameBytesObject;
        private List<byte> Frame;

        public FrameBuilderForColourConfig()
        {
            FrameColourConfiguration FrameBytesObject = new FrameColourConfiguration();
            Frame = new List<byte>();
        }


        public void AddFixedBytes(Device device)
        {
            #region ForLevel1

            FrameBytesObject.Start1 = 0xAA;
            FrameBytesObject.Start2 = 0xCC;           

            if(device.DeviceType == DeviceType.IVD)
            {
                FrameBytesObject.PacketIdentifier3 = 0x06; //Packet for IVD
            }
            else if (device.DeviceType == DeviceType.OVD)
            {
                FrameBytesObject.PacketIdentifier3 = 0x07; //Packet for OVD
            }

            //public byte PacketLengthMSB4 { get; set; }
            //public byte PacketLengthLSB5 { get; set; }

            //public byte SerialNumber10 { get; set; }

            FrameBytesObject.PacketType11 = 0x81; //Set Configuration (Data Transfer)

            FrameBytesObject.StartOfDataPacketIndicator12 = 0x02;// Yes this is classified as a data packet

            // DATA STUFF GOES HERE

            FrameBytesObject.Level1EndOfDataPacket = 0x03;

            //FrameBytesObject.CRC_MSB = 0xCC;
            //FrameBytesObject.CRC_LSB = 0xCC;

            FrameBytesObject.EOT = 0x04;

            #endregion


        }

        public void AddDeviceDetails( Device device)
        {
            // NOTE: ALL THE DIRECT READ FIELDS WILL NOT NECESSARILY BE  
            // READ FROM THE ACTIVETRAIN INSTANCE. MANY OF THEM WILL ALSO BE  
            // DEPENDENT ON OTHER SETTINGS WHICH IN TURN IS BEING SET FROM SOME UI  
            // ELEMENT AND HENCE IS BEING STORED IN ONE OF THE FIELDS  

            IPAddress SourceIPAddress = IPAddress.Parse("192.168.0.253");
            IPAddress DestinationIPAddress = IPAddress.Parse(device.IpAddress);

            byte[] octets = DestinationIPAddress.GetAddressBytes();
            // Make sure it's an IPv4 address
            if (octets.Length == 4)
            {
                FrameBytesObject.DestinationAddressThird6 = octets[2];   // Index 2 = 3rd octet
                FrameBytesObject.DestinationAddressFourth7 = octets[3];  // Index 3 = 4th octet

            }

            octets = SourceIPAddress.GetAddressBytes();
            // Make sure it's an IPv4 address
            if (octets.Length == 4)
            {
                FrameBytesObject.SourceAddressThird8 = octets[2];   // Index 2 = 3rd octet
                FrameBytesObject.SourceAddressFourth9 = octets[3];  // Index 3 = 4th octet

            }



            FrameBytesObject.IntensityLevel = device.IntensityByte; 

            FrameBytesObject.DataTimeout = device.IntensityByte;


        }

        public byte[] CompileFrame(Device device, ObservableCollection<TrainDisplayTemplate> TrainTemplates,  ColorDisplayTheme Theme  )
        {

            #region Processing

            #region Theme
            FrameBytesObject.R_HorizontalLine = Theme.HorizontalLineColor.R;
            FrameBytesObject.G_HorizontalLine = Theme.HorizontalLineColor.G;
            FrameBytesObject.B_HorizontalLine = Theme.HorizontalLineColor.B;

            FrameBytesObject.R_VerticalLine = Theme.VerticalLineColor.R;
            FrameBytesObject.G_VerticalLine = Theme.VerticalLineColor.G;
            FrameBytesObject.B_VerticalLine = Theme.VerticalLineColor.B;

            FrameBytesObject.R_MessageLine = Theme.MessageLineColor.R;
            FrameBytesObject.G_MessageLine = Theme.MessageLineColor.G;
            FrameBytesObject.B_MessageLine = Theme.MessageLineColor.B;

            FrameBytesObject.R_Background = Theme.BackgroundColor.R;
            FrameBytesObject.G_Background = Theme.BackgroundColor.G;
            FrameBytesObject.B_Background = Theme.BackgroundColor.B;
            #endregion

            #region TrainTemplates

            foreach (var template in TrainTemplates) {

                StatusColourConfig holder = new StatusColourConfig();

                switch (template.StatusType)
                {
                    case "Arrival":                       
                        switch (template.StatusDescription)
                        {
                            case "Running Right Time":
                                holder.StatusByte = 0x01;
                            break;

                            case "Will Arrive Shortly":
                                holder.StatusByte = 0x02; // Will Arrive Shortly
                            break;

                            case "Is Arriving On":
                                holder.StatusByte = 0x03;
                                break;

                            case "Has Arrived On":
                                holder.StatusByte = 0x04;
                                break;

                            case "Running Late":
                                holder.StatusByte = 0x05;
                                break;

                            case "Cancelled":
                                holder.StatusByte = 0x06;
                                break;

                            case "Indefinitely Late":
                                holder.StatusByte = 0x07;
                                break;

                            case "Terminated At":
                                holder.StatusByte = 0x08;
                                break;

                            case "Platform Changed":
                                holder.StatusByte = 0x09;
                                break;

                        }
                        break;

                    case "Departure":
                        switch (template.StatusDescription)
                        {
                            case "Running Right Time":
                                holder.StatusByte = 0x0A;
                                break;

                            case "Cancelled":
                                holder.StatusByte = 0x0B; 
                                break;

                            case "Is Ready to Leave":
                                holder.StatusByte = 0x0C;
                                break;

                            case "Is on Platform":
                                holder.StatusByte = 0x0D;
                                break;

                            case "Departed":
                                holder.StatusByte = 0x0E;
                                break;

                            case "Rescheduled":
                                holder.StatusByte = 0x0F;
                                break;

                            case "Diverted":
                                holder.StatusByte = 0x10;
                                break;

                            case "Delay Departure":
                                holder.StatusByte = 0x11;
                                break;

                            case "Platform Changed":
                                holder.StatusByte = 0x12;
                                break;

                            case "Change of Source":
                                holder.StatusByte = 0x13;
                                break;

                        }
                        break;
                }

                holder.R_TrainNo = template.TrainNoColor.R;
                holder.G_TrainNo = template.TrainNoColor.G;
                holder.B_TrainNo = template.TrainNoColor.B;

                holder.R_TrainName = template.TrainNameColor.R;
                holder.G_TrainName = template.TrainNameColor.G;
                holder.B_TrainName = template.TrainNameColor.B;

                holder.R_TrainTime = template.TrainTimeColor.R;
                holder.G_TrainTime = template.TrainTimeColor.G;
                holder.B_TrainTime = template.TrainTimeColor.B;

                holder.R_TrainAD = template.TrainADColor.R; 
                holder.G_TrainAD = template.TrainADColor.G;
                holder.B_TrainAD = template.TrainADColor.B; 

                holder.R_TrainPF = template.TrainPFColor.R; 
                holder.G_TrainPF = template.TrainPFColor.G;
                holder.B_TrainPF = template.TrainPFColor.B;

                FrameBytesObject.StatusColourConfigs.Add(holder);
            }

            #endregion

            AddDeviceDetails(device);

            AddFixedBytes(device);

            #endregion


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


            #region LEVEL2 CONSTRUCTION  

            Frame.Add(FrameBytesObject.IntensityLevel);
            Frame.Add(FrameBytesObject.DataTimeout);

            Frame.Add(FrameBytesObject.R_HorizontalLine);
            Frame.Add(FrameBytesObject.G_HorizontalLine);
            Frame.Add(FrameBytesObject.B_HorizontalLine);

            Frame.Add(FrameBytesObject.R_VerticalLine);
            Frame.Add(FrameBytesObject.G_VerticalLine);
            Frame.Add(FrameBytesObject.B_VerticalLine);

            Frame.Add(FrameBytesObject.R_Background);
            Frame.Add(FrameBytesObject.G_Background);
            Frame.Add(FrameBytesObject.B_Background);

            Frame.Add(FrameBytesObject.R_MessageLine);
            Frame.Add(FrameBytesObject.G_MessageLine);
            Frame.Add(FrameBytesObject.B_MessageLine);

            int i = 0;

            foreach(var status in FrameBytesObject.StatusColourConfigs)
            {
                Frame.Add(status.StatusByte);
                Frame.Add(status.R_TrainNo);
                Frame.Add(status.G_TrainNo);
                Frame.Add(status.B_TrainNo);
                Frame.Add(status.R_TrainName);
                Frame.Add(status.G_TrainName);
                Frame.Add(status.B_TrainName);
                Frame.Add(status.R_TrainTime);
                Frame.Add(status.G_TrainTime);
                Frame.Add(status.B_TrainTime);
                Frame.Add(status.R_TrainAD);
                Frame.Add(status.G_TrainAD);
                Frame.Add(status.B_TrainAD);
                Frame.Add(status.R_TrainPF);
                Frame.Add(status.G_TrainPF);
                Frame.Add(status.B_TrainPF);
               
            }

            #endregion


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


            #endregion

            //CRC Left

            byte[] frametosend = Frame.ToArray();

            return frametosend;

        }






    }
}
