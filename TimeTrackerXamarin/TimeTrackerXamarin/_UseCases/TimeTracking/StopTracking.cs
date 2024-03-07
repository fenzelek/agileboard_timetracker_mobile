using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using Xamarin.Forms;

namespace TimeTrackerXamarin._UseCases.TimeTracking
{
    public class StopTracking
    {
        private readonly ITimeTracking timeTracking;

        public StopTracking(ITimeTracking timeTracking)
        {
            this.timeTracking = timeTracking;
        }

        public Task<bool> Exec()
        {            
            return timeTracking.Stop();
        }
    }
}
