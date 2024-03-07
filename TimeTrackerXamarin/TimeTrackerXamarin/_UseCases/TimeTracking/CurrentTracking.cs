using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;

namespace TimeTrackerXamarin._UseCases.TimeTracking
{
    public class CurrentTracking
    {
        private readonly ITimeTracking timeTracking;

        public CurrentTracking(ITimeTracking timeTracking)
        {
            this.timeTracking = timeTracking;
        }
    }
}
