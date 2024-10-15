using Newtonsoft.Json;
using System;

namespace IpisCentralDisplayController.ntes
{
    public class NtesTrain951
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

        [JsonProperty("trainSubType")]
        public string TrainSubType { get; set; }

        [JsonProperty("trainSubTypeName")]
        public string TrainSubTypeName { get; set; }

        [JsonProperty("trainSubTypeNameHindi")]
        public string TrainSubTypeNameHindi { get; set; }

        [JsonProperty("trainStartDate")]
        public DateTime? TrainStartDate { get; set; }

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
        public DateTime? STA { get; set; }

        [JsonProperty("STA_HHMM")]
        public string STA_HHMM { get; set; }

        [JsonProperty("STA_HHMMDDMMM")]
        public string STA_HHMMDDMMM { get; set; }

        [JsonProperty("STA_HHMMDDMM")]
        public string STA_HHMMDDMM { get; set; }

        [JsonProperty("ETA")]
        public DateTime? ETA { get; set; }

        [JsonProperty("ETA_HHMM")]
        public string ETA_HHMM { get; set; }

        [JsonProperty("ETA_HHMMDDMMM")]
        public string ETA_HHMMDDMMM { get; set; }

        [JsonProperty("ETA_HHMMDDMM")]
        public string ETA_HHMMDDMM { get; set; }

        [JsonProperty("isArrived")]
        public bool IsArrived { get; set; }

        [JsonProperty("delayArr")]
        public string DelayArr { get; set; }

        [JsonProperty("isDeparted")]
        public bool IsDeparted { get; set; }

        [JsonProperty("stnDClassFlag")]
        public int StnDClassFlag { get; set; }

        [JsonProperty("STD")]
        public DateTime? STD { get; set; }

        [JsonProperty("STD_HHMM")]
        public string STD_HHMM { get; set; }

        [JsonProperty("STD_HHMMDDMMM")]
        public string STD_HHMMDDMMM { get; set; }

        [JsonProperty("STD_HHMMDDMM")]
        public string STD_HHMMDDMM { get; set; }

        [JsonProperty("ETD")]
        public DateTime? ETD { get; set; }

        [JsonProperty("ETD_HHMM")]
        public string ETD_HHMM { get; set; }

        [JsonProperty("ETD_HHMMDDMMM")]
        public string ETD_HHMMDDMMM { get; set; }

        [JsonProperty("ETD_HHMMDDMM")]
        public string ETD_HHMMDDMM { get; set; }

        [JsonProperty("delayDep")]
        public string DelayDep { get; set; }

        [JsonProperty("ADFlag")]
        public string ADFlag { get; set; }

        [JsonProperty("expectedTime")]
        public string ExpectedTime { get; set; }

        [JsonProperty("expectedDelay")]
        public string ExpectedDelay { get; set; }

        [JsonProperty("trainStatus")]
        public string TrainStatus { get; set; }

        [JsonProperty("expectedDateTime")]
        public DateTime? ExpectedDateTime { get; set; }

        [JsonProperty("platformNo")]
        public string PlatformNo { get; set; }

        [JsonProperty("trainReversalFlag")]
        public int TrainReversalFlag { get; set; }

        [JsonProperty("arrivalCoachClass")]
        public string ArrivalCoachClass { get; set; }

        [JsonProperty("arrivalCoachPosition")]
        public string ArrivalCoachPosition { get; set; }

        [JsonProperty("departureCoachClass")]
        public string DepartureCoachClass { get; set; }

        [JsonProperty("departureCoachPosition")]
        public string DepartureCoachPosition { get; set; }

        [JsonProperty("arrPWDCoachPosition")]
        public string ArrPWDCoachPosition { get; set; }

        [JsonProperty("depPWDCoachPosition")]
        public string DepPWDCoachPosition { get; set; }

        [JsonProperty("coachPosition")]
        public string CoachPosition { get; set; }

        [JsonProperty("coachClass")]
        public string CoachClass { get; set; }

        [JsonProperty("schStation")]
        public string SchStation { get; set; }

        [JsonProperty("schStationDate")]
        public DateTime? SchStationDate { get; set; }

        [JsonProperty("schStationEvent")]
        public string SchStationEvent { get; set; }

        [JsonProperty("trainClassOfTravel")]
        public string TrainClassOfTravel { get; set; }

        [JsonProperty("actSrc")]
        public string ActSrc { get; set; }

        [JsonProperty("actSrcName")]
        public string ActSrcName { get; set; }

        [JsonProperty("actSrcNameHindi")]
        public string ActSrcNameHindi { get; set; }

        [JsonProperty("actDstn")]
        public string ActDstn { get; set; }

        [JsonProperty("actDstnName")]
        public string ActDstnName { get; set; }

        [JsonProperty("actDstnNameHindi")]
        public string ActDstnNameHindi { get; set; }

        [JsonProperty("exceptionFlag")]
        public int ExceptionFlag { get; set; }

        [JsonProperty("exceptionMsg")]
        public string ExceptionMsg { get; set; }

        [JsonProperty("exceptionMsgShort")]
        public string ExceptionMsgShort { get; set; }

        [JsonProperty("reschedule")]
        public bool Reschedule { get; set; }

        [JsonProperty("srcChange")]
        public bool SrcChange { get; set; }

        [JsonProperty("diverted")]
        public bool Diverted { get; set; }

        [JsonProperty("dstnChange")]
        public bool DstnChange { get; set; }

        [JsonProperty("showExpectedTimeFlag")]
        public bool ShowExpectedTimeFlag { get; set; }

        [JsonProperty("stnSrNo")]
        public int StnSrNo { get; set; }

        [JsonProperty("reservedTrainFlag")]
        public bool ReservedTrainFlag { get; set; }
    }
}
