using IpisCentralDisplayController.converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpisCentralDisplayController.models
{
    [JsonConverter(typeof(MediaFileConverter))]
    public abstract class MediaFile
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Updated { get; set; } = DateTime.Now;
        public MediaType MediaType { get; set; }
    }

    public enum MediaType
    {
        Image,
        Video,
        TextSlide,
        Audio
    }

    public class ImageFile : MediaFile
    {
        public string Resolution { get; set; }

        public ImageFile()
        {
            MediaType = MediaType.Image;
        }
    }

    public class VideoFile : MediaFile
    {
        public TimeSpan Duration { get; set; }
        public string Resolution { get; set; }
        public string ThumbnailPath { get; set; }

        public VideoFile()
        {
            MediaType = MediaType.Video;
        }
    }

    public class TextSlideFile : MediaFile
    {
        public string TextContent { get; set; }

        public TextSlideFile()
        {
            MediaType = MediaType.TextSlide;
        }
    }

    public class AudioFile : MediaFile
    {
        public TimeSpan Duration { get; set; }
        public string BitRate { get; set; }

        public AudioFile()
        {
            MediaType = MediaType.Audio;
        }
    }

}
