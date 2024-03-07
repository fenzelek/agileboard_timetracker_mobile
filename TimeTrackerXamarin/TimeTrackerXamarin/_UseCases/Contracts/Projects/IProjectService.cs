using System.Collections.Generic;
using System.Threading.Tasks;

namespace TimeTrackerXamarin._UseCases.Contracts.Projects
{
    public interface IProjectService
    {
        Task<List<Project>> GetProjects(int companyId);
    }
}