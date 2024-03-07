using System.Collections.Generic;
using System.Threading.Tasks;

namespace TimeTrackerXamarin._UseCases.Contracts.Projects
{
    public interface ISprintService
    {
        Task<List<Sprint>> GetSprints(int projectId, int companyId);
    }
}