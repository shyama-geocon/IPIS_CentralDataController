using IpisCentralDisplayController.Helpers;
using IpisCentralDisplayController.models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IpisCentralDisplayController.managers
{
    public class EventLogManager
    {
        private readonly IJsonHelper _jsonHelper;
        private readonly string _eventLogsKey = "eventLogs";

        public EventLogManager(IJsonHelper jsonHelper)
        {
            _jsonHelper = jsonHelper;
        }

        // Load all event logs
        public List<EventLog> LoadEventLogs()
        {
            return _jsonHelper.Load<List<EventLog>>(_eventLogsKey) ?? new List<EventLog>();
        }

        // Save all event logs
        public void SaveEventLogs(List<EventLog> eventLogs)
        {
            _jsonHelper.Save(_eventLogsKey, eventLogs);
        }

        // Add a new event log
        public void AddEventLog(EventLog eventLog)
        {
            var eventLogs = LoadEventLogs();
            eventLog.IsSentToServer = false;
            eventLogs.Add(eventLog);
            SaveEventLogs(eventLogs);
        }

        // Update an existing event log
        public void UpdateEventLog(EventLog eventLog)
        {
            var eventLogs = LoadEventLogs();
            var existingEventLog = eventLogs.FirstOrDefault(e => e.EventID == eventLog.EventID);
            if (existingEventLog == null)
            {
                throw new Exception("Event log not found.");
            }

            existingEventLog.Timestamp = eventLog.Timestamp;
            existingEventLog.EventType = eventLog.EventType;
            existingEventLog.Source = eventLog.Source;
            existingEventLog.Description = eventLog.Description;
            existingEventLog.IsSentToServer = eventLog.IsSentToServer; // Ensure IsSentToServer is updated

            SaveEventLogs(eventLogs);
        }


        public void UpdateLogSentStatus(int eventID, bool isSentToServer)
        {
            var eventLogs = LoadEventLogs();
            var eventLog = eventLogs.FirstOrDefault(e => e.EventID == eventID);
            if (eventLog != null)
            {
                eventLog.IsSentToServer = isSentToServer;
                SaveEventLogs(eventLogs);
            }
        }


        public void SendUnsentLogsToServer()
        {
            var eventLogs = LoadEventLogs();
            foreach (var log in eventLogs.Where(e => !e.IsSentToServer))
            {
                bool isSent = SendLogToServer(log);
                if (isSent)
                {
                    log.IsSentToServer = true;
                }
            }
            SaveEventLogs(eventLogs);
        }

        private bool SendLogToServer(EventLog log)
        {
            // Simulate the process of sending a log to the server
            // Return true if the log was successfully sent, otherwise false
            return true; 
        }

        // Delete a specific event log by event ID
        public void DeleteEventLog(int eventID)
        {
            var eventLogs = LoadEventLogs();
            var eventLog = eventLogs.FirstOrDefault(e => e.EventID == eventID);
            if (eventLog == null)
            {
                throw new Exception("Event log not found.");
            }
            eventLogs.Remove(eventLog);
            SaveEventLogs(eventLogs);
        }

        // Delete all event logs
        public void DeleteAllEventLogs()
        {
            var eventLogs = LoadEventLogs();
            eventLogs.Clear();
            SaveEventLogs(eventLogs);
        }

        public EventLog FindEventLogById(int eventID)
        {
            var eventLogs = LoadEventLogs();
            return eventLogs.FirstOrDefault(e => e.EventID == eventID);
        }
    }
}
