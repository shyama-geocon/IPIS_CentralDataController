using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using IpisCentralDisplayController.models;
using IpisCentralDisplayController.models.DisplayConfiguration;

namespace IpisCentralDisplayController.services.DisplayConfigurationServices
{
    public class FrameBuilderForMonoConfig
    {
        private FrameColourConfiguration FrameBytesObject;
        private List<byte> Frame;

        public FrameBuilderForMonoConfig()
        {
            FrameColourConfiguration FrameBytesObject = new FrameColourConfiguration();
            Frame = new List<byte>();
        }


        public void AddFixedBytes(Device device)
        {
            #region ForLevel1

            FrameBytesObject.Start1 = 0xAA;
            FrameBytesObject.Start2 = 0xCC;


            if (device.DeviceType == DeviceType.PFDB)
            {
                FrameBytesObject.PacketIdentifier3 = 0x03;
            }
            else if (device.DeviceType == DeviceType.AGDB)
            {
                FrameBytesObject.PacketIdentifier3 = 0x04;
            }
            else if (device.DeviceType == DeviceType.CGDB)
            {
                FrameBytesObject.PacketIdentifier3 = 0x02;
            }
            else if (device.DeviceType == DeviceType.MLDB)
            {
                FrameBytesObject.PacketIdentifier3 = 0x05;
            }


            //public byte PacketLengthMSB4 { get; set; }
            //public byte PacketLengthLSB5 { get; set; }

            //public byte SerialNumber10 { get; set; }

            FrameBytesObject.PacketType11 = 0x84; //Set Configuration (Data Transfer)

            FrameBytesObject.StartOfDataPacketIndicator12 = 0x02;// Yes this is classified as a data packet

            // DATA STUFF GOES HERE

            FrameBytesObject.Level1EndOfDataPacket = 0x03;

            //FrameBytesObject.CRC_MSB = 0xCC;
            //FrameBytesObject.CRC_LSB = 0xCC;

            FrameBytesObject.EOT = 0x04;

            #endregion


        }

        public void AddDeviceDetails(Device device)
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

            FrameBytesObject.DataTimeout = device.DataTimeoutValueByte;

        }

        public byte[] CompileFrame(Device device)
        {           

            AddDeviceDetails(device);

            AddFixedBytes(device);

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
           
            #endregion


            //Frame.Add(FrameBytesObject.Level1EndOfDataPacket);
            //Frame.Add(FrameBytesObject.CRC_MSB);//NOT ADDED  
            //Frame.Add(FrameBytesObject.CRC_LSB);//NOT ADDED  
            //Frame.Add(FrameBytesObject.EOT);

            //////FrameBytesObject.PacketLengthMSB4
            ////Frame[3] = (byte)(((Frame.Count +1 ) >> 8) & 0xFF);// Most Significant Byte

            //////FrameBytesObject.PacketLengthLSB5
            ////Frame[4] = (byte)((Frame.Count + 1) & 0xFF);// Least Significant Byte

            ////FrameBytesObject.PacketLengthMSB4
            //Frame[3] = (byte)(((Frame.Count - 6) >> 8) & 0xFF);// Most Significant Byte

            ////FrameBytesObject.PacketLengthLSB5
            //Frame[4] = (byte)((Frame.Count - 6) & 0xFF);// Least Significant Byte


            Frame.Add(FrameBytesObject.Level1EndOfDataPacket);


            #region PacketLength

            //FrameBytesObject.PacketLengthMSB4
            Frame[3] = (byte)(((Frame.Count - 3) >> 8) & 0xFF);// Most Significant Byte

            //FrameBytesObject.PacketLengthLSB5
            Frame[4] = (byte)((Frame.Count - 3) & 0xFF);// Least Significant Byte



            #endregion


            #region CRC            

            List<byte> newList = new List<byte>();

            if (Frame.Count > 3)
            {
                newList = Frame
                    .Skip(3)
                    .Take(Frame.Count)
                    .ToList();
            }

            (FrameBytesObject.CRC_MSB, FrameBytesObject.CRC_LSB) = ComputeCrc16CCITT(newList);

            #endregion

            Frame.Add(FrameBytesObject.CRC_MSB);
            Frame.Add(FrameBytesObject.CRC_LSB);
            Frame.Add(FrameBytesObject.EOT);




            #endregion



            byte[] frametosend = Frame.ToArray();

            return frametosend;

        }

        public static (byte crcMsb, byte crcLsb) ComputeCrc16CCITT(List<byte> data)
        {
            ushort crc = 0xFFFF;
            foreach (byte b in data)
            {
                crc ^= (ushort)(b << 8);
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 0x8000) != 0)
                        crc = (ushort)((crc << 1) ^ 0x1021);
                    else
                        crc <<= 1;
                }
            }
            // Convert CRC to two bytes (big-endian: MSB first)
            byte crcMsb = (byte)((crc >> 8) & 0xFF);
            byte crcLsb = (byte)(crc & 0xFF);
            // Create a new array with original data + 2 CRC bytes
            //byte[] result = new byte[data.Length + 2];
            //Buffer.BlockCopy(data, 0, result, 0, data.Length);
            //result[data.Length] = crcMsb;
            //result[data.Length + 1] = crcLsb;

            return (crcMsb, crcLsb);
        }

    }
}
