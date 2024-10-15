using IpisCentralDisplayController.Helpers;
using IpisCentralDisplayController.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpisCentralDisplayController.managers
{
    public class CAPServerSettingsManager
    {
        private readonly IJsonHelper _jsonHelper;
        private readonly string _capSettingsKey = "capServerSettings";

        public CAPServerSettingsManager(IJsonHelper jsonHelper)
        {
            _jsonHelper = jsonHelper;
        }

        // Load CAP server settings
        public CAPServerSettings LoadCapServerSettings()
        {
            return _jsonHelper.Load<CAPServerSettings>(_capSettingsKey) ?? new CAPServerSettings();
        }

        // Save CAP server settings
        public void SaveCapServerSettings(CAPServerSettings settings)
        {
            _jsonHelper.Save(_capSettingsKey, settings);
        }

        // Update CAP server settings
        public void UpdateCapServerSettings(CAPServerSettings settings)
        {
            SaveCapServerSettings(settings);
        }

        // Test connection (simulate the connection check for the CAP server)
        public bool TestConnection(CAPServerSettings settings)
        {
            if (string.IsNullOrEmpty(settings.ApiUrl))
            {
                throw new ArgumentException("API URL cannot be empty.");
            }

            return true;
        }
    }
}
