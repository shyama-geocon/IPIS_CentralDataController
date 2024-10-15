using IpisCentralDisplayController.Helpers;
using IpisCentralDisplayController.models;
using IpisCentralDisplayController.Models;

namespace IpisCentralDisplayController.Managers
{
    public class RmsServerManager
    {
        private readonly IJsonHelper _jsonHelper;
        private readonly string _rmsSettingsKey = "rmsSettings"; // Single server settings key

        public RmsServerManager(IJsonHelper jsonHelper)
        {
            _jsonHelper = jsonHelper;
        }

        // Load RMS server settings for a single server
        public RmsServerSettings LoadRmsServerSettings()
        {
            return _jsonHelper.Load<RmsServerSettings>(_rmsSettingsKey) ?? new RmsServerSettings();
        }

        // Save RMS server settings for a single server
        public void SaveRmsServerSettings(RmsServerSettings settings)
        {
            _jsonHelper.Save(_rmsSettingsKey, settings);
        }

        // Update RMS server settings for the single server
        public void UpdateRmsServerSettings(RmsServerSettings settings)
        {
            SaveRmsServerSettings(settings); // Simply save the updated settings
        }

        // Get RMS server settings (this is just a helper method, same as LoadRmsServerSettings)
        public RmsServerSettings GetRmsServerSettings()
        {
            return LoadRmsServerSettings(); // Fetch the current settings
        }
    }
}
