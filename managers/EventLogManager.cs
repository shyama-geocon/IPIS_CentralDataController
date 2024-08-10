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

        public List<EventLog> LoadEventLogs()
        {
            return _jsonHelper.Load<List<EventLog>>(_eventLogsKey) ?? new List<EventLog>();
        }

        public void SaveEventLogs(List<EventLog> eventLogs)
        {
            _jsonHelper.Save(_eventLogsKey, eventLogs);
        }

        public void AddEventLog(EventLog eventLog)
        {
            var eventLogs = LoadEventLogs();
            eventLogs.Add(eventLog);
            SaveEventLogs(eventLogs);
        }

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

            SaveEventLogs(eventLogs);
        }

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
