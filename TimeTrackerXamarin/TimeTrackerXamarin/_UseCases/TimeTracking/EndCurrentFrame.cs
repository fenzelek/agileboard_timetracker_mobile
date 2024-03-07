using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;

namespace TimeTrackerXamarin._UseCases.TimeTracking
{
    public class EndCurrentFrame
    {
        private readonly ITimeTracking timeTracking;

        public EndCurrentFrame(ITimeTracking timeTracking)
        {
            this.timeTracking = timeTracking;
            
        }

        public Task Exec()
        {
            return timeTracking.EndFrame(DateTimeOffset.Now.ToUnixTimeSeconds(), false);
        }
    }
}
