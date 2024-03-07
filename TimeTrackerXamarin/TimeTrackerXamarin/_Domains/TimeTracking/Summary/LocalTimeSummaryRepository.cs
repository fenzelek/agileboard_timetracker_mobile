using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking.Summary;

namespace TimeTrackerXamarin._Domains.TimeTracking.Summary
{
    public class LocalTimeSummaryRepository : ITimeSummaryRepository
    {
        private readonly ILocalTimeSummarySource source;

        public LocalTimeSummaryRepository(ILocalTimeSummarySource source)
        {
            this.source = source;
        }

        public Task<List<TrackHistory>> GetTrackHistory(int companyId, string from, string to)
        {
            return source.GetTrackHistory(companyId, from, to);
        }

        public Task<TimeSummary> GetTimeSummary(int companyId)
        {
            return source.GetTimeSummary(companyId);
        }

        public Task<long> GetTodaySum(int companyId)
        {
            return source.GetTodaySum(companyId);
        }

        public Task<long> GetTaskSum(int taskId, bool addUnfinished)
        {
            return source.GetTaskSum(taskId, addUnfinished);
        }
    }
}