using System;
using System.Collections.Generic;
using System.Text;


namespace TimeTrackerXamarin._UseCases.Contracts
{
    public class User
    {
        public int id { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public int? role_id { get; set; }
        public string avatar { get; set; }
        public bool? deleted { get; set; }
        public bool? activated { get; set; }
        public string role { get; set; }        
    }
}
