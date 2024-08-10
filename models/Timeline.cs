using System;
using System.Collections.Generic;

namespace IpisCentralDisplayController.models
{
    public class Timeline
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public List<TimelineItem> Items { get; set; } = new List<TimelineItem>();
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Updated { get; set; } = DateTime.Now;

        public Timeline Clone()
        {
            var clonedTimeline = new Timeline
            {
                Id = Guid.NewGuid().ToString(), // Generate new ID for the cloned timeline
                Name = this.Name + " (Copy)",
                Created = this.Created,
                Updated = DateTime.Now
            };

            foreach (var item in this.Items)
            {
                clonedTimeline.Items.Add(item.Clone());
            }

            return clonedTimeline;
        }
    }

    public class TimelineItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public TimelineItemType ItemType { get; set; }
        public string FilePath { get; set; }
        public TimeSpan Duration { get; set; }
        public string Resolution { get; set; }
        public int Position { get; set; } // Position on the timeline
        public string ThumbnailPath { get; set; } // Path to thumbnail image if applicable
        public TimeSpan Offset { get; set; } // Relative time to appear after the timeline has started
        public TimeSpan StartTime { get; set; } // Start time for video
        public TimeSpan StopTime { get; set; } // Stop time for video
        public TimeSpan MaxDuration { get; set; } // Maximum duration based on media type
        public int PixelWidth { get; set; }
        public int PixelHeight { get; set; }
        public bool IsVideo => ItemType == TimelineItemType.Video;

        public string DisplayMode { get; set; } = "Fit"; // Default to "Fit"

        public TimeSpan VideoDuration { get; set; }

        // Calculated properties (for convenience)
        public TimeSpan EndTime => Offset + Duration;

        public TimelineItem Clone()
        {
            return new TimelineItem
            {
                Id = Guid.NewGuid().ToString(), // Generate new ID for the cloned item
                Name = this.Name,
                ItemType = this.ItemType,
                FilePath = this.FilePath,
                Duration = this.Duration,
                Resolution = this.Resolution,
                Position = this.Position,
                ThumbnailPath = this.ThumbnailPath,
                Offset = this.Offset,
                StartTime = this.StartTime,
                StopTime = this.StopTime,
                MaxDuration = this.MaxDuration,
                PixelWidth = this.PixelWidth,
                PixelHeight = this.PixelHeight,
                DisplayMode = this.DisplayMode // Include the display mode in the clone
            };
        }
    }


    public enum TimelineItemType
    {
        Image,
        Video,
        TextSlide,
        Audio,
        Transition,
        Effect
    }
}
