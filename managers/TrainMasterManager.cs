using IpisCentralDisplayController.Helpers;
using IpisCentralDisplayController.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpisCentralDisplayController.managers
{
    public class TrainMasterManager
    {
        private readonly IJsonHelper _jsonHelper;
        private readonly string _trainMasterKey = "trainMaster";

        public TrainMasterManager(IJsonHelper jsonHelper)
        {
            _jsonHelper = jsonHelper;
        }

        public List<TrainMaster> LoadTrainMasters()
        {
            return _jsonHelper.Load<List<TrainMaster>>(_trainMasterKey) ?? new List<TrainMaster>();
        }

        public void SaveTrainMasters(List<TrainMaster> trainMasters)
        {
            _jsonHelper.Save(_trainMasterKey, trainMasters);
        }

        public void AddTrainMaster(TrainMaster trainMaster)
        {
            var trainMasters = LoadTrainMasters();
            if (trainMasters.Any(t => t.TrainNumber == trainMaster.TrainNumber))
            {
                throw new Exception("Train with this number already exists.");
            }
            trainMasters.Add(trainMaster);
            SaveTrainMasters(trainMasters);
        }

        public void UpdateTrainMaster(TrainMaster trainMaster)
        {
            var trainMasters = LoadTrainMasters();
            var existingTrainMaster = trainMasters.FirstOrDefault(t => t.TrainNumber == trainMaster.TrainNumber);
            if (existingTrainMaster == null)
            {
                throw new Exception("Train not found.");
            }
            // Update train properties here
            existingTrainMaster.TrainNameEnglish = trainMaster.TrainNameEnglish;
            existingTrainMaster.TrainNameHindi = trainMaster.TrainNameHindi;
            existingTrainMaster.TrainNameRegional = trainMaster.TrainNameRegional;
            existingTrainMaster.SrcCode = trainMaster.SrcCode;
            existingTrainMaster.SrcNameEnglish = trainMaster.SrcNameEnglish;
            existingTrainMaster.SrcNameHindi = trainMaster.SrcNameHindi;
            existingTrainMaster.SrcNameRegional = trainMaster.SrcNameRegional;
            existingTrainMaster.DestCode = trainMaster.DestCode;
            existingTrainMaster.DestNameEnglish = trainMaster.DestNameEnglish;
            existingTrainMaster.DestNameHindi = trainMaster.DestNameHindi;
            existingTrainMaster.DestNameRegional = trainMaster.DestNameRegional;
            existingTrainMaster.STA = trainMaster.STA;
            existingTrainMaster.STD = trainMaster.STD;
            existingTrainMaster.DaysOfDeparture = trainMaster.DaysOfDeparture;
            existingTrainMaster.DaysOfArrival = trainMaster.DaysOfArrival;
            existingTrainMaster.Platform = trainMaster.Platform;
            existingTrainMaster.CoachSequence = trainMaster.CoachSequence;
            existingTrainMaster.TrainType = trainMaster.TrainType;

            SaveTrainMasters(trainMasters);
        }

        public void DeleteTrainMaster(string trainNumber)
        {
            var trainMasters = LoadTrainMasters();
            var trainMaster = trainMasters.FirstOrDefault(t => t.TrainNumber == trainNumber);
            if (trainMaster == null)
            {
                throw new Exception("Train not found.");
            }
            trainMasters.Remove(trainMaster);
            SaveTrainMasters(trainMasters);
        }

        public void DeleteAllTrainMasters()
        {
            var trainMasters = LoadTrainMasters();
            trainMasters.Clear();
            SaveTrainMasters(trainMasters);
        }

        public TrainMaster FindTrainMasterByNumber(string trainNumber)
        {
            var trainMasters = LoadTrainMasters();
            return trainMasters.FirstOrDefault(t => t.TrainNumber == trainNumber);
        }
    }
}
