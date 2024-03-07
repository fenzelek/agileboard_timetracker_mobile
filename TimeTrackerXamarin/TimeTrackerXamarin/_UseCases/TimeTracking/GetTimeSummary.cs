using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;

namespace TimeTrackerXamarin._UseCases.TimeTracking
{
    public class GetTimeSummary
    {
        private readonly IFactory<ITimeSummaryService> factory;
        private ITimeSummaryService service;
        
        public GetTimeSummary(IFactory<ITimeSummaryService> factory)
        {
            this.factory = factory;
        }
        
        public void SetConnection(bool connection)
        {
            service = factory.Create(connection);
        }

        public Task<List<TrackHistory>> GettTrackHistory(int companyId, string from, string to)
        {
            return service.GetTrackHistory(companyId, from, to);
        }

        public Task<TimeSummary> Get(int companyId)
        {
            return service.GetTimeSummary(companyId);
        }
        
        public Task<long> GetTodayTotal(int companyId)
        {
            return service.GetTodaySum(companyId);
        }

        public Task<long> GetTaskTotal(int taskId, bool addUnfinished)
        {
            return service.GetTaskSum(taskId, addUnfinished);
        }
        
    }
}