using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IpisCentralDisplayController.models;
using Newtonsoft.Json;

namespace IpisCentralDisplayController.services
{
    public class DeviceStatusService : IService
    {
        private bool _isRunning;
        private CancellationTokenSource _cts;
        private Queue<(Platform Platform, Device Device)> _deviceQueue;
        private List<Platform> _platforms;

        public event Action<string> OnStatusUpdateComplete; // Event to send JSON response back to MainWindow

        public DeviceStatusService(List<Platform> platforms)
        {
            _platforms = platforms;
            _deviceQueue = new Queue<(Platform, Device)>();
            EnqueueAllDevices();
        }

        public void Start()
        {
            _isRunning = true;
            _cts = new CancellationTokenSource();
            Task.Run(() => ProcessDeviceQueue(_cts.Token), _cts.Token);
            Console.WriteLine("DeviceStatusService started.");
        }

        public void Stop()
        {
            _isRunning = false;
            _cts.Cancel();
            Console.WriteLine("DeviceStatusService stopped.");
        }

        public bool IsRunning()
        {
            return _isRunning;
        }

        private void EnqueueAllDevices()
        {
            foreach (var platform in _platforms)
            {
                foreach (var device in platform.Devices)
                {
                    _deviceQueue.Enqueue((platform, device));
                }
            }
        }

        private async Task ProcessDeviceQueue(CancellationToken token)
        {
            while (_isRunning && !token.IsCancellationRequested && _deviceQueue.Count > 0)
            {
                var (platform, device) = _deviceQueue.Dequeue();
                bool isReachable;

                if (platform.PlatformNumber == "0")
                {
                    isReachable = PingDevice(device.IpAddress);
                }
                else
                {
                    isReachable = await QueryDeviceStatusViaPDC(device.IpAddress);
                }

                device.Status = isReachable;
                device.LastStatusWhen = DateTime.Now;

                await Task.Delay(500, token); // Small delay between checks
            }

            // Convert the updated _platforms list to JSON format
            var jsonResponse = JsonConvert.SerializeObject(_platforms, Formatting.Indented);
            OnStatusUpdateComplete?.Invoke(jsonResponse); // Send JSON to MainWindow
        }

        private bool PingDevice(string ipAddress)
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = ping.Send(ipAddress, 2000); // Timeout 2 seconds
                    return reply.Status == IPStatus.Success;
                }
            }
            catch (PingException ex)
            {
                Console.WriteLine($"Ping to {ipAddress} failed: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> QueryDeviceStatusViaPDC(string deviceIpAddress)
        {
            try
            {
                using (var client = new TcpClient(deviceIpAddress, 7888))
                using (var stream = client.GetStream())
                {
                    var message = new { msg_type = "get-status" };
                    var messageJson = JsonConvert.SerializeObject(message);
                    var messageBytes = Encoding.UTF8.GetBytes(messageJson);

                    await stream.WriteAsync(messageBytes, 0, messageBytes.Length);

                    byte[] responseBuffer = new byte[4096];
                    int bytesRead = await stream.ReadAsync(responseBuffer, 0, responseBuffer.Length);
                    var responseJson = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);
                    var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJson);

                    return response.ContainsKey("status") && response["status"].ToString() == "OK";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error querying device status via PDC: {ex.Message}");
                return false;
            }
        }
    }
}
