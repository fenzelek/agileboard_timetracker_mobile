using System;
using System.Collections.Generic;
using System.Text;

namespace TimeTrackerXamarin._UseCases.Contracts.TimeTracking
{
    public class TimeSummaryData
    {
        public Dictionary<string, long> companies { get; set; }
        public Dictionary<string, long> projects { get; set; }
        public Dictionary<string, long> tickets { get; set; }
    }
}
