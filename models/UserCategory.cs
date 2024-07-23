using IpisCentralDisplayController.models;
using System;
using System.Collections.Generic;

namespace IpisCentralDisplayController.models
{
    public class UserCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<UserRights> Rights { get; set; }
    }
}
