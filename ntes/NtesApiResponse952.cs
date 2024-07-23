using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpisCentralDisplayController.ntes
{
    public class NtesApiResponse952
    {
        [JsonProperty("vTrainList")]
        public List<NtesTrain952> VTrainList { get; set; }

        [JsonProperty("restServiceMessage")]
        public RestServiceMessage RestServiceMessage { get; set; }

        [JsonProperty("cacheUpdateTime")]
        public CacheUpdateTime CacheUpdateTime { get; set; }
    }
}
