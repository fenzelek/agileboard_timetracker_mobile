using System;
using Prism.Ioc;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TimeTrackerXamarin._Domains.TimeTracking;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin._UseCases.Contracts;
using Xamarin.Forms;

namespace TimeTrackerXamarin._UseCases.TimeTracking
{
    public class StartTracking
    {
        private readonly ITimeTracking timeTracking;

        public StartTracking(ITimeTracking timeTracking)
        {
            this.timeTracking = timeTracking;
        }

        public Task<bool> Exec(int company_id, int project_id, int ticket_id)
        {            
            return timeTracking.Start(company_id, project_id, ticket_id);
        }
    }
}
