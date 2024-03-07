using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects;

namespace TimeTrackerXamarin._UseCases.Projects
{
    public class GetSprints
    {
        private readonly ISprintService sprintService;

        public GetSprints(ISprintService sprintService)
        {
            this.sprintService = sprintService;
        }

        public Task<List<Sprint>> GetAll(int projectId, int companyId)
        {
            return sprintService.GetSprints(projectId, companyId);
        }
    }
}