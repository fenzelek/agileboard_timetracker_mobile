using System.Collections.Generic;

namespace TimeTrackerXamarin._UseCases.Contracts.TimeTracking
{
    public class TimeSummary
    { 
        public Dictionary<int, long> Companies { get; set; }
        public Dictionary<int, long> Projects { get; set; }
        public Dictionary<int, long> Tickets { get; set; }
    }
}