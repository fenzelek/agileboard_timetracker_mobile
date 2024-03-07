using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects;

namespace TimeTrackerXamarin._UseCases.Projects
{
    public class GetProjects
    {
        private readonly IFactory<IProjectService> projectServiceFactory;
        private IProjectService projectService;

        public GetProjects(IFactory<IProjectService> projectServiceFactory)
        {
            this.projectServiceFactory = projectServiceFactory;
        }

        public void SwitchConnection(bool connection)
        {
            projectService = projectServiceFactory.Create(connection);
        }

        public Task<List<Project>> GetAll(int companyId)
        {
            return projectService.GetProjects(companyId);
        }
    }
}