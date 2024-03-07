using Flurl.Http;
using TimeTrackerXamarin._Domains.API;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects;

namespace TimeTrackerXamarin._Domains.Projects
{
    public class ProjectServiceFactory : IFactory<IProjectService>
    {

        private readonly IRemoteProjectSource remoteSource;
        private readonly ILocalProjectSource localSource;


        public ProjectServiceFactory(IRemoteProjectSource remoteSource, ILocalProjectSource localSource)
        {
            this.remoteSource = remoteSource;
            this.localSource = localSource;
        }

        private IProjectRepository CreateRepository(bool connection)
        {
            if (connection)
            {
                return new RemoteProjectRepository(remoteSource, localSource);
            }

            return new LocalProjectRepository(localSource);
        }

        public IProjectService Create(bool connection)
        {
            var repository = CreateRepository(connection);
            return new ProjectService(repository);
        }
    }
}