using System;
using System.Collections.Generic;
using System.Text;
using TimeTrackerXamarin._Domains.API.dto;

namespace TimeTrackerXamarin._UseCases.Contracts
{
    public class TicketDetails: Ticket
    {
        public string description { get; set; }
        public long estimate_time { get; set; }
        public string sprint_name { get; set; }
        public string assigned_user_fullname { get; set; }
        public string reporting_user_fullname { get; set; }
        public long alltime { get; set; }    
        public long mytime { get; set; }    
    }
}
