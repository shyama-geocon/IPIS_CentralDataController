using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpisCentralDisplayController.ntes
{
    public class NtesApiResponse951
    {
        [JsonProperty("vTrainList")]
        public List<NtesTrain951> VTrainList { get; set; }

        [JsonProperty("vRescheduledTrainList")]
        public List<NtesTrain951> VRescheduledTrainList { get; set; }

        [JsonProperty("vCancelledTrainList")]
        public List<NtesTrain951> VCancelledTrainList { get; set; }

        [JsonProperty("vCancelTrainDueToCS")]
        public List<NtesTrain951> VCancelTrainDueToCS { get; set; }

        [JsonProperty("vCancelTrainDueToCD")]
        public List<NtesTrain951> VCancelTrainDueToCD { get; set; }

        [JsonProperty("vCancelTrainDueToDiversion")]
        public List<NtesTrain951> VCancelTrainDueToDiversion { get; set; }

        [JsonProperty("vTrainListDueToCS")]
        public List<NtesTrain951> VTrainListDueToCS { get; set; }

        [JsonProperty("vTrainListDueToCD")]
        public List<NtesTrain951> VTrainListDueToCD { get; set; }

        [JsonProperty("vTrainListDueToDV")]
        public List<NtesTrain951> VTrainListDueToDV { get; set; }

        [JsonProperty("restServiceMessage")]
        public RestServiceMessage RestServiceMessage { get; set; }

        [JsonProperty("cacheUpdateTime")]
        public CacheUpdateTime CacheUpdateTime { get; set; }
    }
}
