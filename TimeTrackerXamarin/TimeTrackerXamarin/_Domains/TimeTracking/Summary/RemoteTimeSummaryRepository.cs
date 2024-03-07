using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking.Summary;

namespace TimeTrackerXamarin._Domains.TimeTracking.Summary
{
    public class RemoteTimeSummaryRepository : ITimeSummaryRepository
    {
        private readonly IRemoteTimeSummarySource remoteSource;
        private readonly ILocalTimeSummarySource localSource;

        public RemoteTimeSummaryRepository(IRemoteTimeSummarySource remoteSource, ILocalTimeSummarySource localSource)
        {
            this.remoteSource = remoteSource;
            this.localSource = localSource;
        }

        public Task<List<TrackHistory>> GetTrackHistory(int companyId, string from, string to)
        {
            return remoteSource.GetTrackHistory(companyId, from, to);
        }

        public Task<TimeSummary> GetTimeSummary(int companyId)
        {
            return remoteSource.GetTimeSummary(companyId);
        }

        public Task<long> GetTodaySum(int companyId)
        {
            return remoteSource.GetTodaySum(companyId);
        }

        public Task<long> GetTaskSum(int taskId, bool addUnfinished)
        {
            return localSource.GetTaskSum(taskId, addUnfinished);
        }
    }
}