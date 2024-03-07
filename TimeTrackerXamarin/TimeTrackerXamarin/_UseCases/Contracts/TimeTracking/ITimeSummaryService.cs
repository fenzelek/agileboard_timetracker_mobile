using System.Collections.Generic;
using System.Threading.Tasks;

namespace TimeTrackerXamarin._UseCases.Contracts.TimeTracking
{
    public interface ITimeSummaryService
    {
        Task<List<TrackHistory>> GetTrackHistory(int companyId, string from, string to);
        Task<TimeSummary> GetTimeSummary(int companyId);
        Task<long> GetTodaySum(int companyId);
        Task<long> GetTaskSum(int taskId, bool addUnfinished);
    }
}