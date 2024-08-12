using IpisCentralDisplayController.Helpers;
using IpisCentralDisplayController.models;
using IpisCentralDisplayController.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IpisCentralDisplayController.Managers
{
    public class StationManager
    {
        private readonly IJsonHelper _jsonHelper;
        private readonly string _stationsKey = "stations";

        public StationManager(IJsonHelper jsonHelper)
        {
            _jsonHelper = jsonHelper;
        }

        public List<Station> LoadStations()
        {
            return _jsonHelper.Load<List<Station>>(_stationsKey) ?? new List<Station>();
        }

        public void SaveStations(List<Station> stations)
        {
            _jsonHelper.Save(_stationsKey, stations);
        }

        public void AddStation(Station station)
        {
            var stations = LoadStations();
            if (stations.Any(s => s.StationCode == station.StationCode))
            {
                throw new Exception("Station with this code already exists.");
            }
            stations.Add(station);
            SaveStations(stations);
        }

        public void UpdateStation(Station station)
        {
            var stations = LoadStations();
            var existingStation = stations.FirstOrDefault(s => s.StationCode == station.StationCode);
            if (existingStation == null)
            {
                throw new Exception("Station not found.");
            }

            // Update station properties here
            existingStation.StationNameEnglish = station.StationNameEnglish;
            existingStation.StationNameHindi = station.StationNameHindi;
            existingStation.StationNameAssamese = station.StationNameAssamese;
            existingStation.StationNameBangla = station.StationNameBangla;
            existingStation.StationNameDogri = station.StationNameDogri;
            existingStation.StationNameGujarati = station.StationNameGujarati;
            existingStation.StationNameKannada = station.StationNameKannada;
            existingStation.StationNameKonkani = station.StationNameKonkani;
            existingStation.StationNameMalayalam = station.StationNameMalayalam;
            existingStation.StationNameMarathi = station.StationNameMarathi;
            existingStation.StationNameManipuri = station.StationNameManipuri;
            existingStation.StationNameNepali = station.StationNameNepali;
            existingStation.StationNameOdia = station.StationNameOdia;
            existingStation.StationNamePunjabi = station.StationNamePunjabi;
            existingStation.StationNameSanskrit = station.StationNameSanskrit;
            existingStation.StationNameSindhi = station.StationNameSindhi;
            existingStation.StationNameTamil = station.StationNameTamil;
            existingStation.StationNameTelugu = station.StationNameTelugu;
            existingStation.StationNameUrdu = station.StationNameUrdu;

            SaveStations(stations);
        }

        public void DeleteStation(string stationCode)
        {
            var stations = LoadStations();
            var station = stations.FirstOrDefault(s => s.StationCode == stationCode);
            if (station == null)
            {
                throw new Exception("Station not found.");
            }
            stations.Remove(station);
            SaveStations(stations);
        }

        public void DeleteAllStations()
        {
            var stations = LoadStations();
            stations.Clear();
            SaveStations(stations);
        }

        public Station FindStationByCode(string stationCode)
        {
            var stations = LoadStations();
            return stations.FirstOrDefault(s => s.StationCode == stationCode);
        }
    }
}
