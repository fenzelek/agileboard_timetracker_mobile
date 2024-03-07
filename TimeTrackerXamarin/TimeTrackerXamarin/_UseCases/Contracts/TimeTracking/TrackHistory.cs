using System;
using System.Collections.Generic;
using System.Text;

namespace TimeTrackerXamarin._UseCases.Contracts.TimeTracking
{
    public class TrackHistory
    {
        public DateTime date { get; set; }
        public long tracked { get; set; }
    }
}
