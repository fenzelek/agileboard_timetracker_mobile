using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects;

namespace TimeTrackerXamarin._Domains.Projects
{
    public class LocalProjectRepository : IProjectRepository
    {
        private readonly ILocalProjectSource source;

        public LocalProjectRepository(ILocalProjectSource source)
        {
            this.source = source;
        }

        public Task<List<Project>> GetAll(int companyId)
        {
            return source.GetAll(companyId);
        }

        public Task<List<Sprint>> GetSprints(int projectId)
        {
            throw new NotImplementedException("Sprints are not saved locally.");
        }
    }
}