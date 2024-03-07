using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;

namespace TimeTrackerXamarin._UseCases.TimeTracking
{
    public class ContinueLast
    {
        private readonly ITimeTracking timeTracking;

        public ContinueLast(ITimeTracking timeTracking)
        {
            this.timeTracking = timeTracking;
        }

        public Task Exec(bool unblock)
        {
            return timeTracking.ContinueLast(unblock);
        }
    }
}