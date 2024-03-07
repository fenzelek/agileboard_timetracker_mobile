using System;
using System.Collections.Generic;
using System.Text;

namespace TimeTrackerXamarin._UseCases.Contracts.TimeTracking
{
    public class TimeTrackerStats
    {
        public long tracked_summary { get; set; }
        public long activity_summary { get; set; }
        public long activity_level { get; set; }
        public long time_usage { get; set; }
    }
}
