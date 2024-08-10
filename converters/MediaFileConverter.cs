using IpisCentralDisplayController.models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace IpisCentralDisplayController.converters
{
    public class MediaFileConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(MediaFile).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            MediaFile mediaFile;

            switch ((MediaType)jo["MediaType"].Value<int>())
            {
                case MediaType.Image:
                    mediaFile = new ImageFile();
                    break;
                case MediaType.Video:
                    mediaFile = new VideoFile();
                    break;
                case MediaType.TextSlide:
                    mediaFile = new TextSlideFile();
                    break;
                case MediaType.Audio:
                    mediaFile = new AudioFile();
                    break;
                default:
                    throw new Exception("Unknown MediaType");
            }

            serializer.Populate(jo.CreateReader(), mediaFile);
            return mediaFile;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject jo = new JObject();
            var mediaFile = (MediaFile)value;

            jo.Add("Id", JToken.FromObject(mediaFile.Id));
            jo.Add("Name", JToken.FromObject(mediaFile.Name));
            jo.Add("FilePath", JToken.FromObject(mediaFile.FilePath));
            jo.Add("Created", JToken.FromObject(mediaFile.Created));
            jo.Add("Updated", JToken.FromObject(mediaFile.Updated));
            jo.Add("MediaType", JToken.FromObject(mediaFile.MediaType));

            if (mediaFile is ImageFile imageFile)
            {
                jo.Add("Resolution", JToken.FromObject(imageFile.Resolution));
            }
            else if (mediaFile is VideoFile videoFile)
            {
                jo.Add("Duration", JToken.FromObject(videoFile.Duration));
                jo.Add("Resolution", JToken.FromObject(videoFile.Resolution));
            }
            else if (mediaFile is TextSlideFile textSlideFile)
            {
                jo.Add("TextContent", JToken.FromObject(textSlideFile.TextContent));
            }
            else if (mediaFile is AudioFile audioFile)
            {
                jo.Add("Duration", JToken.FromObject(audioFile.Duration));
                jo.Add("BitRate", JToken.FromObject(audioFile.BitRate));
            }

            jo.WriteTo(writer);
        }
    }
}
