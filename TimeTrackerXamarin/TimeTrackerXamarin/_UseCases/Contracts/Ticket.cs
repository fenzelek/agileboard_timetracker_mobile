using SQLite;
using System;
using System.Collections.Generic;
using System.Text;


namespace TimeTrackerXamarin._UseCases.Contracts
{
    public class Ticket
    {
        [PrimaryKey]
        public int id { get; set; }
        public int project_id { get; set; }    
        public int? assigned_id { get; set; }
        public int? sprint_id { get; set; }
        public string name { get; set; }
        public string title { get; set; }
    }
}
