using IpisCentralDisplayController.Helpers;
using IpisCentralDisplayController.models;
using System;
using System.Collections.Generic;
using System.Linq;
using NAudio.CoreAudioApi;

namespace IpisCentralDisplayController.Managers
{
    public class AudioSettingsManager
    {
        private readonly IJsonHelper _jsonHelper;
        private readonly string _audioSettingsKey = "audioSettings";

        public AudioSettingsManager(IJsonHelper jsonHelper)
        {
            _jsonHelper = jsonHelper;
        }

        public AudioSettings LoadAudioSettings()
        {
            return _jsonHelper.Load<AudioSettings>(_audioSettingsKey) ?? GetDefaultAudioSettings();
        }

        public void SaveAudioSettings(AudioSettings settings)
        {
            _jsonHelper.Save(_audioSettingsKey, settings);
        }

        public AudioSettings GetDefaultAudioSettings()
        {
            return new AudioSettings
            {
                MicInterface = "Default",
                MicVolume = 50,
                MonitorInterface = "Default",
                MonitorVolume = 50,
                MonitorEqualizer = new List<double> { 0, 0, 0, 0, 0 },
                AudioOutInterface = "Default",
                AudioOutVolume = 50,
                AudioOutEqualizer = new List<double> { 0, 0, 0, 0, 0 }
            };
        }

        public void ResetToDefault()
        {
            var defaultSettings = GetDefaultAudioSettings();
            SaveAudioSettings(defaultSettings);
        }

        public void UpdateSpecificSetting(Action<AudioSettings> updateAction)
        {
            var settings = LoadAudioSettings();
            updateAction(settings);
            SaveAudioSettings(settings);
        }

        public bool ValidateSettings(AudioSettings settings)
        {
            return settings.MicVolume >= 0 && settings.MicVolume <= 100 &&
                   settings.MonitorVolume >= 0 && settings.MonitorVolume <= 100 &&
                   settings.AudioOutVolume >= 0 && settings.AudioOutVolume <= 100 &&
                   settings.MonitorEqualizer.All(value => value >= -10 && value <= 10) &&
                   settings.AudioOutEqualizer.All(value => value >= -10 && value <= 10);
        }

        //public List<string> RefreshInterfaces()
        //{
        //    // Dummy implementation, replace with actual logic to fetch audio interfaces
        //    return new List<string> { "Default", "Interface1", "Interface2", "Interface3" };
        //}

        public List<string> RefreshInterfaces()
        {
            var deviceNames = new List<string>();

            // Enumerate output devices
            using (var enumerator = new MMDeviceEnumerator())
            {
                var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
                foreach (var device in devices)
                {
                    deviceNames.Add(device.FriendlyName);
                }
            }

            // Optionally, enumerate input devices (microphones)
            using (var enumerator = new MMDeviceEnumerator())
            {
                var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
                foreach (var device in devices)
                {
                    deviceNames.Add(device.FriendlyName);
                }
            }

            return deviceNames;
        }

        public void ApplySettings(AudioSettings settings)
        {
            // Apply microphone settings
            ApplyMicSettings(settings.MicInterface, settings.MicVolume);

            // Apply monitor audio settings
            ApplyMonitorSettings(settings.MonitorInterface, settings.MonitorVolume, settings.MonitorEqualizer);

            // Apply audio output settings
            ApplyAudioOutSettings(settings.AudioOutInterface, settings.AudioOutVolume, settings.AudioOutEqualizer);
        }

        private void ApplyMicSettings(string micInterface, int micVolume)
        {
            // Implement the logic to set the microphone interface and volume
        }

        private void ApplyMonitorSettings(string monitorInterface, int monitorVolume, List<double> equalizer)
        {
            // Implement the logic to set the monitor interface, volume, and equalizer settings
        }

        private void ApplyAudioOutSettings(string audioOutInterface, int audioOutVolume, List<double> equalizer)
        {
            // Implement the logic to set the audio out interface, volume, and equalizer settings
        }
    }
}
