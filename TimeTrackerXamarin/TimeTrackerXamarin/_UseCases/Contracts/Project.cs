using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeTrackerXamarin._UseCases.Contracts
{
    public class Project
    {
        [PrimaryKey,Unique]
        public int id { get; set; }
        public int company_id { get; set; }
        public string name { get; set; }
    }
}
