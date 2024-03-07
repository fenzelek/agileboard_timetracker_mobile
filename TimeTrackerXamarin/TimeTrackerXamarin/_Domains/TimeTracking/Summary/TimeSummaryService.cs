using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking.Summary;

namespace TimeTrackerXamarin._Domains.TimeTracking.Summary
{
    public class TimeSummaryService : ITimeSummaryService
    {

        private readonly ITimeSummaryRepository repository;

        public TimeSummaryService(ITimeSummaryRepository repository)
        {
            this.repository = repository;
        }

        public Task<List<TrackHistory>> GetTrackHistory(int companyId, string from, string to)
        {
            return repository.GetTrackHistory(companyId, from, to);
        }

        public Task<TimeSummary> GetTimeSummary(int companyId)
        {
            return repository.GetTimeSummary(companyId);
        }

        public Task<long> GetTodaySum(int companyId)
        {
            return repository.GetTodaySum(companyId);
        }

        public Task<long> GetTaskSum(int taskId, bool addUnfinished)
        {
            return repository.GetTaskSum(taskId, addUnfinished);
        }
    }
}