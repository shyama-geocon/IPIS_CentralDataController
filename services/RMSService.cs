using IpisCentralDisplayController.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IpisCentralDisplayController.services
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class RMSService : IService
    {
        private bool _isRunning;
        private readonly RmsServerSettings _rmsSettings;
        private Queue<EventLog> _eventLogQueue;  // FIFO Queue for event logs
        private readonly object _queueLock = new object();  // Lock for thread-safe queue access
        private CancellationTokenSource _cts;

        public RMSService(RmsServerSettings rmsSettings)
        {
            _rmsSettings = rmsSettings;
            _eventLogQueue = new Queue<EventLog>();  // Initialize the queue
        }

        public void Start()
        {
            _isRunning = true;
            _cts = new CancellationTokenSource();

            // Start a background task to process the event logs
            Task.Run(() => ProcessEventLogs(_cts.Token), _cts.Token);

            Console.WriteLine("RMSService started.");
        }

        public void Stop()
        {
            _isRunning = false;
            _cts.Cancel();  // Stop the background task

            Console.WriteLine("RMSService stopped.");
        }

        public bool IsRunning()
        {
            return _isRunning;
        }

        // Method to receive event logs and add them to the FIFO queue
        public void ReceiveEventLog(EventLog log)
        {
            lock (_queueLock)
            {
                _eventLogQueue.Enqueue(log);
                Console.WriteLine($"EventLog with ID {log.EventID} received and added to queue.");
            }
        }

        // Background task to process event logs
        private async Task ProcessEventLogs(CancellationToken token)
        {
            while (_isRunning && !token.IsCancellationRequested)
            {
                EventLog logToSend = null;

                lock (_queueLock)
                {
                    if (_eventLogQueue.Count > 0)
                    {
                        logToSend = _eventLogQueue.Dequeue();
                        Console.WriteLine($"Processing EventLog with ID {logToSend.EventID}.");
                    }
                }

                if (logToSend != null)
                {
                    // Send the log to RMS and update its status
                    bool sentSuccessfully = await SendEventLogToRmsAsync(logToSend);

                    if (sentSuccessfully)
                    {
                        logToSend.IsSentToServer = true;
                        Console.WriteLine($"EventLog with ID {logToSend.EventID} sent successfully.");
                        UpdateLocalMemory(logToSend);  // Update local memory to mark log as sent
                    }
                    else
                    {
                        Console.WriteLine($"Failed to send EventLog with ID {logToSend.EventID}. Re-queuing...");
                        lock (_queueLock)
                        {
                            _eventLogQueue.Enqueue(logToSend);  // Re-queue the log if sending failed
                        }
                    }
                }

                await Task.Delay(1000); 
            }
        }

        // Method to send the event log to the RMS server
        private async Task<bool> SendEventLogToRmsAsync(EventLog log)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_rmsSettings.ApiUrl);

                    var eventLogJson = new
                    {
                        timestamp = log.Timestamp,
                        eventID = log.EventID,
                        eventType = log.EventType.ToString(),
                        source = log.Source,
                        description = log.Description
                    };

                    var jsonContent = JsonConvert.SerializeObject(eventLogJson);
                    var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("/api/ext/event-logs", content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Event log sent successfully.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to send event log. Status code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending event log: {ex.Message}");
            }

            return false;
        }


        public async Task<bool> SendStationInfoAsync(StationInfo stationInfo)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_rmsSettings.ApiUrl);

                    var stationInfoJson = new
                    {
                        stationCode = stationInfo.StationCode,
                        regionalLanguage = stationInfo.RegionalLanguage.ToString(),
                        stationNameEnglish = stationInfo.StationNameEnglish,
                        stationNameHindi = stationInfo.StationNameHindi,
                        stationNameRegional = stationInfo.StationNameRegional,
                        latitude = stationInfo.Latitude,
                        longitude = stationInfo.Longitude,
                        altitude = stationInfo.Altitude,
                        numberOfPlatforms = stationInfo.NumberOfPlatforms,
                        numberOfSplPlatforms = stationInfo.NumberOfSplPlatforms,
                        numberOfStationEntrances = stationInfo.NumberOfStationEntrances,
                        numberOfPlatformBridges = stationInfo.NumberOfPlatformBridges
                    };

                    var jsonContent = JsonConvert.SerializeObject(stationInfoJson);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("/api/ext/station-info", content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Station info sent successfully.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to send station info. Status code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending station info: {ex.Message}");
            }

            return false;
        }

        public async Task<bool> UpdateStationInfoAsync(StationInfo stationInfo)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_rmsSettings.ApiUrl);

                    var stationInfoJson = new
                    {
                        stationCode = stationInfo.StationCode,
                        regionalLanguage = stationInfo.RegionalLanguage.ToString(),  // Convert enum to string if necessary
                        stationNameEnglish = stationInfo.StationNameEnglish,
                        stationNameHindi = stationInfo.StationNameHindi,
                        stationNameRegional = stationInfo.StationNameRegional,
                        latitude = stationInfo.Latitude,
                        longitude = stationInfo.Longitude,
                        altitude = stationInfo.Altitude,
                        numberOfPlatforms = stationInfo.NumberOfPlatforms,
                        numberOfSplPlatforms = stationInfo.NumberOfSplPlatforms,
                        numberOfStationEntrances = stationInfo.NumberOfStationEntrances,
                        numberOfPlatformBridges = stationInfo.NumberOfPlatformBridges
                    };

                    var jsonContent = JsonConvert.SerializeObject(stationInfoJson);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync("/api/ext/station-info", content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Station info updated successfully.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to update station info. Status code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating station info: {ex.Message}");
            }

            return false;
        }


        public async Task<bool> SendCapAlertAsync(CAPAlert capAlert)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_rmsSettings.ApiUrl);
                    var content = new StringContent(JsonConvert.SerializeObject(capAlert), System.Text.Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("/api/ext/cap-alerts", content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("CAP alert sent successfully.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to send CAP alert. Status code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending CAP alert: {ex.Message}");
            }

            return false;
        }

        // Method to send platform and devices status to the RMS server
        //public async Task<bool> SendPlatformStatusAsync(PlatformStatus platformStatus)
        //{
        //    try
        //    {
        //        using (HttpClient client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri(_rmsSettings.ApiUrl);
        //            var content = new StringContent(JsonConvert.SerializeObject(platformStatus), System.Text.Encoding.UTF8, "application/json");
        //            HttpResponseMessage response = await client.PostAsync("/api/ext/platforms", content);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                Console.WriteLine("Platform status sent successfully.");
        //                return true;
        //            }
        //            else
        //            {
        //                Console.WriteLine($"Failed to send platform status. Status code: {response.StatusCode}");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error sending platform status: {ex.Message}");
        //    }

        //    return false;
        //}

        private void UpdateLocalMemory(EventLog log)
        {
            Console.WriteLine($"EventLog with ID {log.EventID} updated in local memory (IsSentToServer = {log.IsSentToServer}).");
        }
    }
}
