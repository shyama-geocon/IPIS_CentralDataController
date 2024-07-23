using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpisCentralDisplayController.ntes
{
    public class RestServiceMessage
    {
        [JsonProperty("serviceCallFlag")]
        public string ServiceCallFlag { get; set; }

        [JsonProperty("serviceDataFoundFlag")]
        public string ServiceDataFoundFlag { get; set; }

        [JsonProperty("serviceDataResultFlag")]
        public string ServiceDataResultFlag { get; set; }

        [JsonProperty("serviceInputValidFlag")]
        public string ServiceInputValidFlag { get; set; }

        [JsonProperty("serviceMessage")]
        public string ServiceMessage { get; set; }
    }

    public class CacheUpdateTime
    {
        [JsonProperty("DD")]
        public int DD { get; set; }

        [JsonProperty("MM")]
        public int MM { get; set; }

        [JsonProperty("YYYY")]
        public int YYYY { get; set; }

        [JsonProperty("HH")]
        public int HH { get; set; }

        [JsonProperty("MI")]
        public int MI { get; set; }

        [JsonProperty("SECONDS")]
        public int SECONDS { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("calendar")]
        public Calendar Calendar { get; set; }
    }

    public class Calendar
    {
        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("month")]
        public int Month { get; set; }

        [JsonProperty("dayOfMonth")]
        public int DayOfMonth { get; set; }

        [JsonProperty("hourOfDay")]
        public int HourOfDay { get; set; }

        [JsonProperty("minute")]
        public int Minute { get; set; }

        [JsonProperty("second")]
        public int Second { get; set; }
    }
}
