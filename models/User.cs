using System;

namespace IpisCentralDisplayController.models
{
    public class User
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Designation { get; set; }
        public UserCategory Category { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastLogin { get; set; }
    }
}
