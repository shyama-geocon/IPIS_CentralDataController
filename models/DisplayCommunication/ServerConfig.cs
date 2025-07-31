using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpisCentralDisplayController.models.DisplayCommunication
{
    public class ServerConfig
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public byte[] Packet { get; set; }
    }
}
