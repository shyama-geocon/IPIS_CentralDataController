using IpisCentralDisplayController.Helpers;
using IpisCentralDisplayController.models;

namespace IpisCentralDisplayController.Managers
{
    public class StationInfoManager
    {
        private readonly IJsonHelper _jsonHelper;
        private readonly string _stationInfoKey = "stationInfo";
        public StationInfo CurrentStationInfo { get; private set; }

        public StationInfoManager(IJsonHelper jsonHelper)
        {
            _jsonHelper = jsonHelper;
            CurrentStationInfo = LoadStationInfo();
        }

        public bool IsStationInfoAvailable()
        {
            return CurrentStationInfo != null;
        }

        public StationInfo LoadStationInfo()
        {
            return _jsonHelper.Load<StationInfo>(_stationInfoKey);
        }

        public void SaveStationInfo(StationInfo stationInfo)
        {
            _jsonHelper.Save(_stationInfoKey, stationInfo);
            CurrentStationInfo = stationInfo;
        }

        //public void DeleteStationInfo()
        //{
        //    _jsonHelper.Delete(_stationInfoKey);
        //    CurrentStationInfo = null;
        //}
    }
}
