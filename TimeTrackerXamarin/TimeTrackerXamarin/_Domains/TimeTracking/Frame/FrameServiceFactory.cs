using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking.Frame;

namespace TimeTrackerXamarin._Domains.TimeTracking.Frame
{
    public class FrameServiceFactory : IFactory<IFrameService>
    {
        private readonly IRemoteFrameSource remoteSource;
        private readonly ILocalFrameSource localSource;

        public FrameServiceFactory(IRemoteFrameSource remoteSource, ILocalFrameSource localSource)
        {
            this.remoteSource = remoteSource;
            this.localSource = localSource;
        }

        private IFrameRepository CreateRepository(bool connection)
        {
            if (connection)
            {
                return new RemoteFrameRepository(remoteSource, localSource);
            }

            return new LocalFrameRepository(localSource);
        }

        public IFrameService Create(bool connection)
        {
            var repository = CreateRepository(connection);
            return new FrameService(repository);
        }
    }
}