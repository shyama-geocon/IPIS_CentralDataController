using IpisCentralDisplayController.Helpers;
using IpisCentralDisplayController.models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IpisCentralDisplayController.managers
{
    public class ActiveTrainManager
    {
        private readonly IJsonHelper _jsonHelper;
        private readonly string _activeTrainKey = "activeTrains";

        public ActiveTrainManager(IJsonHelper jsonHelper)
        {
            _jsonHelper = jsonHelper;
        }

        public List<ActiveTrain> LoadActiveTrains()
        {
            return _jsonHelper.Load<List<ActiveTrain>>(_activeTrainKey) ?? new List<ActiveTrain>();
        }

        public void SaveActiveTrains(List<ActiveTrain> activeTrains)
        {
            _jsonHelper.Save(_activeTrainKey, activeTrains);
        }

        public void AddActiveTrain(ActiveTrain activeTrain)
        {
            var activeTrains = LoadActiveTrains();
            if (activeTrains.Any(t => t.TrainNumber == activeTrain.TrainNumber && t.Ref == activeTrain.Ref))
            {
                throw new Exception("Train with this number and source already exists.");
            }
            activeTrains.Add(activeTrain);
            SaveActiveTrains(activeTrains);
        }

        public void UpdateActiveTrain(ActiveTrain activeTrain)
        {
            var activeTrains = LoadActiveTrains();
            var existingActiveTrain = activeTrains.FirstOrDefault(t => t.TrainNumber == activeTrain.TrainNumber && t.Ref == activeTrain.Ref);
            if (existingActiveTrain == null)
            {
                throw new Exception("Train not found.");
            }
            // Update active train properties here
            existingActiveTrain.TrainNameEnglish = activeTrain.TrainNameEnglish;
            existingActiveTrain.TrainNameHindi = activeTrain.TrainNameHindi;
            existingActiveTrain.TrainNameRegional = activeTrain.TrainNameRegional;
            existingActiveTrain.SrcCode = activeTrain.SrcCode;
            existingActiveTrain.SrcNameEnglish = activeTrain.SrcNameEnglish;
            existingActiveTrain.SrcNameHindi = activeTrain.SrcNameHindi;
            existingActiveTrain.SrcNameRegional = activeTrain.SrcNameRegional;
            existingActiveTrain.DestCode = activeTrain.DestCode;
            existingActiveTrain.DestNameEnglish = activeTrain.DestNameEnglish;
            existingActiveTrain.DestNameHindi = activeTrain.DestNameHindi;
            existingActiveTrain.DestNameRegional = activeTrain.DestNameRegional;
            existingActiveTrain.STA = activeTrain.STA;
            existingActiveTrain.STD = activeTrain.STD;
            //existingActiveTrain.AD = activeTrain.AD;
            existingActiveTrain.SelectedADOption = activeTrain.SelectedADOption;
            existingActiveTrain.Status = activeTrain.Status;
            existingActiveTrain.LateBy = activeTrain.LateBy;
            existingActiveTrain.ETA = activeTrain.ETA;
            existingActiveTrain.ETD = activeTrain.ETD;
            existingActiveTrain.PFNo = activeTrain.PFNo;
            existingActiveTrain.CoachSequence = activeTrain.CoachSequence;
            existingActiveTrain.CoachList = activeTrain.CoachList;
            existingActiveTrain.TADDB_Update = activeTrain.TADDB_Update;
            existingActiveTrain.CGDB_Update = activeTrain.CGDB_Update;
            existingActiveTrain.Announce_Update = activeTrain.Announce_Update;

            SaveActiveTrains(activeTrains);
        }

        public void DeleteActiveTrain(string trainNumber, TrainSource source)
        {
            var activeTrains = LoadActiveTrains();
            var activeTrain = activeTrains.FirstOrDefault(t => t.TrainNumber == trainNumber && t.Ref == source);
            if (activeTrain == null)
            {
                throw new Exception("Train not found.");
            }
            activeTrains.Remove(activeTrain);
            SaveActiveTrains(activeTrains);
        }

        public void DeleteAllActiveTrains()
        {
            var activeTrains = LoadActiveTrains();
            activeTrains.Clear();
            SaveActiveTrains(activeTrains);
        }

        public ActiveTrain FindActiveTrainByNumber(string trainNumber, TrainSource source)
        {
            var activeTrains = LoadActiveTrains();
            return activeTrains.FirstOrDefault(t => t.TrainNumber == trainNumber && t.Ref == source);
        }
    }
}
