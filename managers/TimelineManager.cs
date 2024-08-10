using IpisCentralDisplayController.models;
using IpisCentralDisplayController.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IpisCentralDisplayController.Managers
{
    public class TimelineManager
    {
        private readonly IJsonHelper _jsonHelper;
        private readonly string _timelinesKey = "timelines";

        public TimelineManager(IJsonHelper jsonHelper)
        {
            _jsonHelper = jsonHelper;
        }

        public List<Timeline> LoadTimelines()
        {
            return _jsonHelper.Load<List<Timeline>>(_timelinesKey) ?? new List<Timeline>();
        }

        public void SaveTimelines(List<Timeline> timelines)
        {
            _jsonHelper.Save(_timelinesKey, timelines);
        }

        public void SaveTimeline(Timeline timeline)
        {
            var timelines = LoadTimelines();
            var existingTimeline = timelines.FirstOrDefault(t => t.Id == timeline.Id);

            if (existingTimeline != null)
            {
                // Update existing timeline
                existingTimeline.Name = timeline.Name;
                existingTimeline.Items = timeline.Items;
                existingTimeline.Updated = DateTime.Now;
            }
            else
            {
                // Add new timeline if it does not exist
                timelines.Add(timeline);
            }

            SaveTimelines(timelines);
        }


        public void SaveTimelineAs(Timeline timeline)
        {
            var timelines = LoadTimelines();
            if (timelines.Exists(t => t.Name == timeline.Name))
            {
                throw new Exception("A timeline with this name already exists. Please choose a different name.");
            }
            timelines.Add(timeline);
            SaveTimelines(timelines);
        }

        public void AddTimeline(Timeline timeline)
        {
            var timelines = LoadTimelines();
            if (timelines.Any(t => t.Id == timeline.Id))
            {
                throw new Exception("Timeline with this ID already exists.");
            }
            timelines.Add(timeline);
            SaveTimelines(timelines);
        }

        public void UpdateTimeline(Timeline timeline)
        {
            var timelines = LoadTimelines();
            var existingTimeline = timelines.FirstOrDefault(t => t.Id == timeline.Id);
            if (existingTimeline == null)
            {
                throw new Exception("Timeline not found.");
            }
            // Update timeline properties here
            existingTimeline.Name = timeline.Name;
            existingTimeline.Items = timeline.Items;
            existingTimeline.Updated = DateTime.Now;

            SaveTimelines(timelines);
        }

        public void DeleteTimeline(string id)
        {
            var timelines = LoadTimelines();
            var timeline = timelines.FirstOrDefault(t => t.Id == id);
            if (timeline == null)
            {
                throw new Exception("Timeline not found.");
            }
            timelines.Remove(timeline);
            SaveTimelines(timelines);
        }

        public void DeleteAllTimelines()
        {
            var timelines = LoadTimelines();
            timelines.Clear();
            SaveTimelines(timelines);
        }

        public Timeline FindTimelineById(string id)
        {
            var timelines = LoadTimelines();
            return timelines.FirstOrDefault(t => t.Id == id);
        }

        public void AddTimelineItem(string timelineId, TimelineItem item)
        {
            var timeline = FindTimelineById(timelineId);
            if (timeline == null)
            {
                throw new Exception("Timeline not found.");
            }
            if (timeline.Items.Any(i => i.Id == item.Id))
            {
                throw new Exception("Item with this ID already exists in the timeline.");
            }
            timeline.Items.Add(item);
            UpdateTimeline(timeline);
        }

        public void RemoveTimelineItem(string timelineId, string itemId)
        {
            var timeline = FindTimelineById(timelineId);
            if (timeline == null)
            {
                throw new Exception("Timeline not found.");
            }
            var item = timeline.Items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
            {
                throw new Exception("Item not found in the timeline.");
            }
            timeline.Items.Remove(item);
            UpdateTimeline(timeline);
        }

        public void UpdateTimelineItem(string timelineId, TimelineItem item)
        {
            var timeline = FindTimelineById(timelineId);
            if (timeline == null)
            {
                throw new Exception("Timeline not found.");
            }
            var existingItem = timeline.Items.FirstOrDefault(i => i.Id == item.Id);
            if (existingItem == null)
            {
                throw new Exception("Item not found in the timeline.");
            }
            // Update timeline item properties here
            existingItem.Name = item.Name;
            existingItem.ItemType = item.ItemType;
            existingItem.FilePath = item.FilePath;
            existingItem.Duration = item.Duration;
            existingItem.Resolution = item.Resolution;
            existingItem.Position = item.Position;
            existingItem.ThumbnailPath = item.ThumbnailPath;
            existingItem.Offset = item.Offset;

            UpdateTimeline(timeline);
        }
    }
}
