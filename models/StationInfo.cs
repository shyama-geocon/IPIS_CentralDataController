namespace IpisCentralDisplayController.models
{
    public class StationInfo
    {
        public string StationCode { get; set; }
        public RegionalLanguage RegionalLanguage { get; set; }
        public string StationNameEnglish { get; set; }
        public string StationNameHindi { get; set; }
        public string StationNameRegional { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public int NumberOfPlatforms { get; set; }
        public int NumberOfSplPlatforms { get; set; }
        public int NumberOfStationEntrances { get; set; }
        public int NumberOfPlatformBridges { get; set; }
    }
}
