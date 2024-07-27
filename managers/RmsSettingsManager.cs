using IpisCentralDisplayController.Helpers;
using IpisCentralDisplayController.Models;
using System;

namespace IpisCentralDisplayController.Managers
{
    public class RmsSettingsManager
    {
        private readonly IJsonHelper _jsonHelper;
        private readonly string _rmsSettingsKey = "rmsSettings";

        public RmsSettingsManager(IJsonHelper jsonHelper)
        {
            _jsonHelper = jsonHelper;
        }

        public RmsSettings LoadRmsSettings()
        {
            return _jsonHelper.Load<RmsSettings>(_rmsSettingsKey) ?? GetDefaultRmsSettings();
        }

        public void SaveRmsSettings(RmsSettings settings)
        {
            _jsonHelper.Save(_rmsSettingsKey, settings);
        }

        public RmsSettings GetDefaultRmsSettings()
        {
            return new RmsSettings
            {
                Server1Ip = "192.168.1.1",
                Server1ApiEndpoint = "/api/v1",
                Server1ApiKey = "",
                Server2Ip = "192.168.1.2",
                Server2ApiEndpoint = "/api/v1",
                Server2ApiKey = ""
            };
        }

        public bool TestConnection(string serverIp, string apiEndpoint, string apiKey)
        {
            // Implement logic to test connection to the RMS server
            // This can include sending a simple HTTP request to the server
            // Return true if the connection is successful, otherwise false
            try
            {
                // Placeholder for actual connection test logic
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
