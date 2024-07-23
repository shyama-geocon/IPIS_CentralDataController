using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpisCentralDisplayController.ntes
{
    public class NtesTrain952
    {
        [JsonProperty("trainNo")]
        public string TrainNo { get; set; }

        [JsonProperty("trainName")]
        public string TrainName { get; set; }

        [JsonProperty("trainNameHindi")]
        public string TrainNameHindi { get; set; }

        [JsonProperty("trainType")]
        public string TrainType { get; set; }

        [JsonProperty("trainTypeName")]
        public string TrainTypeName { get; set; }

        [JsonProperty("trainTypeNameHindi")]
        public string TrainTypeNameHindi { get; set; }

        [JsonProperty("src")]
        public string Src { get; set; }

        [JsonProperty("srcName")]
        public string SrcName { get; set; }

        [JsonProperty("srcNameHindi")]
        public string SrcNameHindi { get; set; }

        [JsonProperty("dstn")]
        public string Dstn { get; set; }

        [JsonProperty("dstnName")]
        public string DstnName { get; set; }

        [JsonProperty("dstnNameHindi")]
        public string DstnNameHindi { get; set; }

        [JsonProperty("STA")]
        public string STA { get; set; }

        [JsonProperty("STD")]
        public string STD { get; set; }

        [JsonProperty("daysOfDeparture")]
        public string DaysOfDeparture { get; set; }

        [JsonProperty("daysOfArrival")]
        public string DaysOfArrival { get; set; }

        [JsonProperty("stationSchSrNo")]
        public int StationSchSrNo { get; set; }

        [JsonProperty("platformNo")]
        public string PlatformNo { get; set; }

        [JsonProperty("trainReversalFlag")]
        public int TrainReversalFlag { get; set; }

        [JsonProperty("arrivalCoachPosition")]
        public string ArrivalCoachPosition { get; set; }

        [JsonProperty("arrivalCoachClass")]
        public string ArrivalCoachClass { get; set; }

        [JsonProperty("departureCoachPosition")]
        public string DepartureCoachPosition { get; set; }

        [JsonProperty("departureCoachClass")]
        public string DepartureCoachClass { get; set; }

        [JsonProperty("arrPWDCoachPosition")]
        public string ArrPWDCoachPosition { get; set; }

        [JsonProperty("depPWDCoachPosition")]
        public string DepPWDCoachPosition { get; set; }

        [JsonProperty("coachPosition")]
        public string CoachPosition { get; set; }

        [JsonProperty("coachClass")]
        public string CoachClass { get; set; }

        [JsonProperty("trainClassOfTravel")]
        public string TrainClassOfTravel { get; set; }
    }
}
