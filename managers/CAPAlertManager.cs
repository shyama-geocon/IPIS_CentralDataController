using IpisCentralDisplayController.Models;
using IpisCentralDisplayController.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using IpisCentralDisplayController.models;

namespace IpisCentralDisplayController.managers
{
    public class CAPAlertManager
    {
        private readonly IJsonHelper _jsonHelper;
        private readonly string _capAlertsKey = "capAlerts";

        public CAPAlertManager(IJsonHelper jsonHelper)
        {
            _jsonHelper = jsonHelper;
        }

        public List<CAPAlert> LoadCAPAlerts()
        {
            return _jsonHelper.Load<List<CAPAlert>>(_capAlertsKey) ?? new List<CAPAlert>();
        }

        public void SaveCAPAlerts(List<CAPAlert> capAlerts)
        {
            _jsonHelper.Save(_capAlertsKey, capAlerts);
        }

        public void AddCAPAlert(CAPAlert capAlert)
        {
            var capAlerts = LoadCAPAlerts();
            if (capAlerts.Any(c => c.Identifier == capAlert.Identifier))
            {
                throw new Exception("CAP Alert with this identifier already exists.");
            }
            capAlerts.Add(capAlert);
            SaveCAPAlerts(capAlerts);
        }

        public void UpdateCAPAlert(CAPAlert capAlert)
        {
            var capAlerts = LoadCAPAlerts();
            var existingCAPAlert = capAlerts.FirstOrDefault(c => c.Identifier == capAlert.Identifier);
            if (existingCAPAlert == null)
            {
                throw new Exception("CAP Alert not found.");
            }
            // Update CAP Alert properties here
            existingCAPAlert.Sender = capAlert.Sender;
            existingCAPAlert.Sent = capAlert.Sent;
            existingCAPAlert.Status = capAlert.Status;
            existingCAPAlert.MsgType = capAlert.MsgType;
            existingCAPAlert.Source = capAlert.Source;
            existingCAPAlert.Scope = capAlert.Scope;
            existingCAPAlert.Restriction = capAlert.Restriction;
            existingCAPAlert.Address = capAlert.Address;
            existingCAPAlert.Note = capAlert.Note;
            existingCAPAlert.References = capAlert.References;
            existingCAPAlert.Incidents = capAlert.Incidents;
            existingCAPAlert.Info = capAlert.Info;

            SaveCAPAlerts(capAlerts);
        }

        public void DeleteCAPAlert(string identifier)
        {
            var capAlerts = LoadCAPAlerts();
            var capAlert = capAlerts.FirstOrDefault(c => c.Identifier == identifier);
            if (capAlert == null)
            {
                throw new Exception("CAP Alert not found.");
            }
            capAlerts.Remove(capAlert);
            SaveCAPAlerts(capAlerts);
        }

        public void DeleteAllCAPAlerts()
        {
            var capAlerts = LoadCAPAlerts();
            capAlerts.Clear();
            SaveCAPAlerts(capAlerts);
        }

        public CAPAlert FindCAPAlertByIdentifier(string identifier)
        {
            var capAlerts = LoadCAPAlerts();
            return capAlerts.FirstOrDefault(c => c.Identifier == identifier);
        }

        public void ActivateCAPAlert(string identifier, bool isActive)
        {
            var capAlert = FindCAPAlertByIdentifier(identifier);
            if (capAlert == null)
            {
                throw new Exception("CAP Alert not found.");
            }
            // Assuming we add an IsActive property to CAPAlert
            capAlert.Info.Certainty = isActive ? "Active" : "Inactive";
            SaveCAPAlerts(LoadCAPAlerts());
        }

        public void AssignCAPAlertCategory(string identifier, string category)
        {
            var capAlert = FindCAPAlertByIdentifier(identifier);
            if (capAlert == null)
            {
                throw new Exception("CAP Alert not found.");
            }
            capAlert.Info.Category = category;
            SaveCAPAlerts(LoadCAPAlerts());
        }
    }
}
