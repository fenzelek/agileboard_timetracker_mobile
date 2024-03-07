using System.Collections.Generic;
using System.Threading.Tasks;

namespace TimeTrackerXamarin._UseCases.Contracts.TimeTracking.Summary
{
    public interface ITimeSummaryRepository
    {
        Task<List<TrackHistory>> GetTrackHistory(int companyId, string from, string to);
        Task<TimeSummary> GetTimeSummary(int companyId);
        Task<long> GetTodaySum(int companyId);
        Task<long> GetTaskSum(int taskId, bool addUnfinished);
    }
}