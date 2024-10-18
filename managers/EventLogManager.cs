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

        // Filter by date range
        public List<EventLog> FilterLogsByDate(DateTime? fromDate, DateTime? toDate)
        {
            var eventLogs = LoadEventLogs();
            if (fromDate.HasValue && toDate.HasValue)
            {
                return eventLogs.Where(log => log.Timestamp.Date >= fromDate.Value.Date && log.Timestamp.Date <= toDate.Value.Date).ToList();
            }
            return eventLogs;
        }

        // Filter by EventType
        public List<EventLog> FilterLogsByEventType(List<EventType> eventTypes)
        {
            var eventLogs = LoadEventLogs();
            if (eventTypes != null && eventTypes.Count > 0)
            {
                return eventLogs.Where(log => eventTypes.Contains(log.EventType)).ToList();
            }
            return eventLogs;
        }

        // Filter by both date range and event type
        public List<EventLog> FilterLogs(DateTime? fromDate, DateTime? toDate, List<EventType> eventTypes)
        {
            var eventLogs = LoadEventLogs();
            var filteredLogs = eventLogs.AsEnumerable();

            // Apply date range filter
            if (fromDate.HasValue && toDate.HasValue)
            {
                filteredLogs = filteredLogs.Where(log => log.Timestamp.Date >= fromDate.Value.Date && log.Timestamp.Date <= toDate.Value.Date);
            }

            // Apply event type filter
            if (eventTypes != null && eventTypes.Count > 0)
            {
                filteredLogs = filteredLogs.Where(log => eventTypes.Contains(log.EventType));
            }

            return filteredLogs.ToList();
        }

        // Update log sent status
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

        // Find an event log by ID
        public EventLog FindEventLogById(int eventID)
        {
            var eventLogs = LoadEventLogs();
            return eventLogs.FirstOrDefault(e => e.EventID == eventID);
        }
    }
}
