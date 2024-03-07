using System.Collections.Generic;
using System.Threading.Tasks;

namespace TimeTrackerXamarin._UseCases.Contracts.Projects
{
    public interface IProjectRepository
    {
        Task<List<Project>> GetAll(int companyId);
    }
}