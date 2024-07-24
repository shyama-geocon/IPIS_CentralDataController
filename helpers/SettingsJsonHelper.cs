using System;
using Newtonsoft.Json;

namespace IpisCentralDisplayController.Helpers
{
    public static class SettingsJsonHelper
    {
        public static void Save<T>(string key, T value)
        {
            string json = JsonConvert.SerializeObject(value);
            Properties.Settings.Default[key] = json;
            Properties.Settings.Default.Save();
        }

        public static T Load<T>(string key)
        {
            try
            {
                string json = Properties.Settings.Default[key]?.ToString();
                return string.IsNullOrEmpty(json) ? default : JsonConvert.DeserializeObject<T>(json);
            }
            catch(Exception ex)
            {
                return default;
            }           
        }

        public static void Delete(string key)
        {
            Properties.Settings.Default[key] = null;
            Properties.Settings.Default.Save();
        }

        public static void Update<T>(string key, T value)
        {
            Save(key, value);
        }
    }
}
