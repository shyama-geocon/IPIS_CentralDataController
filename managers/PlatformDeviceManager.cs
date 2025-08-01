using System;
using System.Collections.Generic;
using System.Linq;
using IpisCentralDisplayController.Helpers;
using IpisCentralDisplayController.models;

namespace IpisCentralDisplayController.Managers
{
    public class PlatformDeviceManager
    {
        private readonly IJsonHelper _jsonHelper;
        private readonly string _platformsKey = "platformInfo";

        public PlatformDeviceManager(IJsonHelper jsonHelper)
        {
            _jsonHelper = jsonHelper;
            CurrentPlatformInfo = LoadPlatforms();
        }

        public List<Platform> CurrentPlatformInfo { get; private set; }

        public List<Platform> LoadPlatforms()
        {
            return _jsonHelper.Load<List<Platform>>(_platformsKey) ?? new List<Platform>();
        }

        public void SavePlatforms(List<Platform> platforms)
        {
            _jsonHelper.Save(_platformsKey, platforms);
        }

        public void AddPlatform(Platform platform)
        {
            var platforms = LoadPlatforms();
            platforms.Add(platform);
            SavePlatforms(platforms);
            CurrentPlatformInfo = platforms;
        }

        public void UpdatePlatform(Platform platform)
        {
            var platforms = LoadPlatforms();
            var existingPlatform = platforms.FirstOrDefault(p => p.PlatformNumber == platform.PlatformNumber);
            if (existingPlatform != null)
            {
                existingPlatform.PlatformType = platform.PlatformType;
                existingPlatform.Description = platform.Description;
                existingPlatform.Subnet = platform.Subnet;
                SavePlatforms(platforms);
                CurrentPlatformInfo = platforms;
            }
        }

        public void DeletePlatform(string platformNumber)
        {
            var platforms = LoadPlatforms();
            var platform = platforms.FirstOrDefault(p => p.PlatformNumber == platformNumber);
            if (platform != null)
            {
                platforms.Remove(platform);
                SavePlatforms(platforms);
                CurrentPlatformInfo = platforms;
            }
        }

        public bool AddDevice(string platformNumber, Device device)
        {
            var platforms = LoadPlatforms();
            var platform = platforms.FirstOrDefault(p => p.PlatformNumber == platformNumber);
            if (platform != null)
            {
                if (device.DeviceType == DeviceType.PrimaryServer && platformNumber != "0")
                {
                    throw new InvalidOperationException("Primary server can only be on platform 0.");
                }

                //if (platform.Devices.Any(d => d.DeviceType == DeviceType.CDS || d.DeviceType == DeviceType.PDC) && (device.DeviceType == DeviceType.CDS || device.DeviceType == DeviceType.PDC))
                //{
                //    throw new InvalidOperationException("There can only be one CDS or PDC on a platform.");
                //}

                platform.Devices.Add(device);
                SavePlatforms(platforms);
                CurrentPlatformInfo = platforms;
                return true;
            }
            return false;
        }

        public bool UpdateDevice(string platformNumber, Device device)
        {
            var platforms = LoadPlatforms();
            var platform = platforms.FirstOrDefault(p => p.PlatformNumber == platformNumber);
            if (platform != null)
            {
                var existingDevice = platform.Devices.FirstOrDefault(d => d.Id == device.Id);
                if (existingDevice != null)
                {
                    if (device.DeviceType == DeviceType.PrimaryServer && platformNumber != "0")
                    {
                        throw new InvalidOperationException("Primary server can only be on platform 0.");
                    }

                    if (platform.Devices.Any(d => (d.DeviceType == DeviceType.CDS || d.DeviceType == DeviceType.PDC) && d.Id != device.Id) && (device.DeviceType == DeviceType.CDS || device.DeviceType == DeviceType.PDC))
                    {
                        throw new InvalidOperationException("There can only be one CDS or PDC on a platform.");
                    }

                    existingDevice.DeviceType = device.DeviceType;
                    existingDevice.IpAddress = device.IpAddress;
                    existingDevice.Status = device.Status;
                    existingDevice.IsEnabled = device.IsEnabled;
                    existingDevice.Updated = DateTime.Now;
                    SavePlatforms(platforms);
                    CurrentPlatformInfo = platforms;
                    return true;
                }
            }
            return false;
        }

        public void DeleteDevice(string platformNumber, int deviceId)
        {
            var platforms = LoadPlatforms();
            var platform = platforms.FirstOrDefault(p => p.PlatformNumber == platformNumber);
            if (platform != null)
            {
                var device = platform.Devices.FirstOrDefault(d => d.Id == deviceId);
                if (device != null)
                {
                    platform.Devices.Remove(device);
                    SavePlatforms(platforms);
                    CurrentPlatformInfo = platforms;
                }
            }
        }

        public string CalculateNextIpAddress(Platform platform, DeviceType deviceType)
        {
            string subnet = platform.Subnet.TrimEnd('.');
            List<int> assignedIps = platform.Devices.Select(d => int.Parse(d.IpAddress.Split('.').Last())).ToList();
            int nextIp = GetNextIp(deviceType, assignedIps);
            return $"{subnet}.{nextIp}";
        }

        private int GetNextIp(DeviceType deviceType, List<int> assignedIps)
        {
            int start, end;
            switch (deviceType)
            {
                case DeviceType.CDS:
                case DeviceType.PDC:
                    start = 252; end = 252; break;
                case DeviceType.PrimaryServer:
                    start = 253; end = 253; break;
                case DeviceType.SecondaryServer:
                    start = 254; end = 254; break;
                case DeviceType.CGDB:
                    start = 2; end = 39; break;
                case DeviceType.AGDB:
                    start = 131; end = 160; break;
                case DeviceType.PFDB:
                    start = 161; end = 190; break;
                case DeviceType.OVD:
                    start = 40; end = 70; break;
                case DeviceType.IVD:
                    start = 71; end = 100; break;
                case DeviceType.SLDB:
                case DeviceType.MLDB:
                    start = 101; end = 130; break;
                case DeviceType.LED_TV:
                    start = 191; end = 220; break;
                case DeviceType.NW_SW:
                    start = 221; end = 250; break;
                default:
                    throw new InvalidOperationException("Unknown device type.");
            }

            for (int ip = start; ip <= end; ip++)
            {
                if (!assignedIps.Contains(ip))
                {
                    return ip;
                }
            }

            throw new InvalidOperationException("No available IP addresses for the specified device type.");
        }
    }
}
