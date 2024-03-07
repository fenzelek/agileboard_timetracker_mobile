using System.Collections.Generic;
using System.Threading.Tasks;

namespace TimeTrackerXamarin._UseCases.Contracts.TimeTracking.Summary
{
    public interface IRemoteTimeSummarySource
    {
        Task<List<TrackHistory>> GetTrackHistory(int companyId, string from, string to);
        Task<TimeSummary> GetTimeSummary(int companyId);
        Task<long> GetTodaySum(int companyId);
    }
}