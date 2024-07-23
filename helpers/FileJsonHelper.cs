using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace IpisCentralDisplayController.Helpers
{
    public static class FileJsonHelper
    {
        public static void Save<T>(string filePath, T value)
        {
            string json = JsonConvert.SerializeObject(value, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static T Load<T>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return default;
            }

            string json = File.ReadAllText(filePath);
            return string.IsNullOrEmpty(json) ? default : JsonConvert.DeserializeObject<T>(json);
        }

        public static void Delete(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public static void Update<T>(string filePath, T value)
        {
            Save(filePath, value);
        }
    }
}
