using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using SQLite;
using TimeTrackerXamarin._Domains.API;
using TimeTrackerXamarin._Domains.API.dto;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects;

namespace TimeTrackerXamarin._Domains.Projects
{
    public class RemoteProjectRepository : IProjectRepository
    {
        private readonly IRemoteProjectSource remoteSource;
        private readonly ILocalProjectSource localSource;

        public RemoteProjectRepository(IRemoteProjectSource remoteSource, ILocalProjectSource localSource)
        {
            this.remoteSource = remoteSource;
            this.localSource = localSource;
        }

        public async Task<List<Project>> GetAll(int companyId)
        {
            var projects = await remoteSource.GetAll(companyId);
            await localSource.SaveProjects(projects);
            return projects;
        }
    }
}