using System;

namespace IpisCentralDisplayController.models
{
    public class EventLog
    {
        public DateTime Timestamp { get; set; }
        public int EventID { get; set; }
        public EventType EventType { get; set; }
        public string Source { get; set; }
        public string Description { get; set; }
    }

    public enum EventType
    {
        Information,
        Error,
        Warning,
        Critical
    }
}
