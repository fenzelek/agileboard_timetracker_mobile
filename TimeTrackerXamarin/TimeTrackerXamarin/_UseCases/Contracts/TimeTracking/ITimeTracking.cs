using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimeTrackerXamarin._UseCases.Contracts.TimeTracking
{
    public interface ITimeTracking
    {
        Task<bool> Start(int company_id, int project_id, int ticket_id);
        Task<bool> ContinueLast(bool unblock = true);
        Task<bool> Stop();
        Task EndFrame(long endTime, bool createNew= true);
        Task<CurrentTrackingInfo> GetCurrentTracking();
        Task<bool> Sync();
    }
}
