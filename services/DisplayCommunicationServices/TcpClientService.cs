using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IpisCentralDisplayController.models.DisplayCommunication;

namespace IpisCentralDisplayController.services.DisplayCommunicationServices
{
    public class TcpClientService
    {
        public async Task<(bool Success, string Response, string ErrorMessage)> SendPacketAsync(ServerConfig serverConfig, CancellationToken cancellationToken)
        {
            try
            {
                using (var tcpClient = new TcpClient())
                {
                    // Connect to the server asynchronously
                    await tcpClient.ConnectAsync(serverConfig.IpAddress, serverConfig.Port, cancellationToken);

                    // Get the network stream
                    using (var stream = tcpClient.GetStream())
                    {
                        // Send the packet
                        await stream.WriteAsync(serverConfig.Packet, 0, serverConfig.Packet.Length, cancellationToken);
                        await stream.FlushAsync(cancellationToken);

                        // Read the response (adjust buffer size and response format as needed)
                        byte[] buffer = new byte[1024];
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                        string response = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        // Validate the response (customize based on your protocol)
                        bool isValid = ValidateResponse(response);

                        return (isValid, response, null);
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, null, $"Failed to communicate with {serverConfig.IpAddress}:{serverConfig.Port}: {ex.Message}");
            }
        }


        private bool ValidateResponse(string response)
        {
            // Implement your validation logic here
            // Example: Check if response matches expected format or contains specific data
            return !string.IsNullOrEmpty(response) && response.Contains("ACK"); // Placeholder logic
        }










    }
}
