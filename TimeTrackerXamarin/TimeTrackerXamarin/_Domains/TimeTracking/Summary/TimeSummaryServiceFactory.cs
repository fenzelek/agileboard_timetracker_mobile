using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking.Summary;

namespace TimeTrackerXamarin._Domains.TimeTracking.Summary
{
    public class TimeSummaryServiceFactory : IFactory<ITimeSummaryService>
    {
        private readonly IRemoteTimeSummarySource remoteSource;
        private readonly ILocalTimeSummarySource localSource;

        public TimeSummaryServiceFactory(IRemoteTimeSummarySource remoteSource, ILocalTimeSummarySource localSource)
        {
            this.remoteSource = remoteSource;
            this.localSource = localSource;
        }
        
        private ITimeSummaryRepository CreateRepository(bool connection)
        {
            if (connection)
            {
                return new RemoteTimeSummaryRepository(remoteSource, localSource);
            }

            return new LocalTimeSummaryRepository(localSource);
        }

        public ITimeSummaryService Create(bool connection)
        {
            var repository = CreateRepository(connection);
            return new TimeSummaryService(repository);
        }
    }
}