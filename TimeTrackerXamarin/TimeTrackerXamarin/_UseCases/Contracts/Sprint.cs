using System;
using System.Collections.Generic;
using System.Text;
using TimeTrackerXamarin._Domains.API.dto;

namespace TimeTrackerXamarin._UseCases.Contracts
{
    public class Sprint
    {
        public int id { get; set; }
        public string name { get; set; }
        public int? project_id { get; set; }
        public JSONDataDto<List<Ticket>> tickets { get; set; }
    }
}
