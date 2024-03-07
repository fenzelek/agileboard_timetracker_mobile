using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeTrackerXamarin._UseCases.Contracts.Companies
{
    public class Company
    {
        // [PrimaryKey]
        public int id { get; set; }
        public string name { get; set; }
    }
}
