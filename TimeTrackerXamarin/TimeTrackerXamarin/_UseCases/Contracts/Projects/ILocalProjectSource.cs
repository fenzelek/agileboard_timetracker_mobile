using System.Collections.Generic;
using System.Threading.Tasks;

namespace TimeTrackerXamarin._UseCases.Contracts.Projects
{
    public interface ILocalProjectSource
    {
        Task<List<Project>> GetAll(int companyId);
        Task<bool> SaveProjects(List<Project> projects);
    }
}