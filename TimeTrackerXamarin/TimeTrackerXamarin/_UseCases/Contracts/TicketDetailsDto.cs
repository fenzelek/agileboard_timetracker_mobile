using System;
using System.Collections.Generic;
using System.Text;
using TimeTrackerXamarin._Domains.API.dto;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;

namespace TimeTrackerXamarin._UseCases.Contracts
{
    public class TicketDetailsDto: Ticket
    {
        public string description { get; set; }
        public long estimate_time { get; set; }
        public JSONDataDto<Sprint> sprint { get; set; }
        public JSONDataDto<User> assigned_user { get; set; }
        public JSONDataDto<User> reporting_user { get; set; }
        public JSONDataDto<TimeTrackerStats> stats { get; set; }    
    }
}
