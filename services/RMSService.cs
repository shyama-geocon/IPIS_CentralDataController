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
                    //client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_rmsSettings.ApiKey}");  // If authentication is needed

                    var content = new StringContent(JsonConvert.SerializeObject(log), System.Text.Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("/api/logs", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending event log: {ex.Message}");
            }

            return false;
        }

        private void UpdateLocalMemory(EventLog log)
        {
            Console.WriteLine($"EventLog with ID {log.EventID} updated in local memory (IsSentToServer = {log.IsSentToServer}).");
        }
    }
}
