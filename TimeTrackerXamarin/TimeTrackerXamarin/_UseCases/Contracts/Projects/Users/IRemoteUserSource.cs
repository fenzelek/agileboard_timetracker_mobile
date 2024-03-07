using System.Collections.Generic;
using System.Threading.Tasks;

namespace TimeTrackerXamarin._UseCases.Contracts.Projects.Users
{
    public interface IRemoteUserSource
    {
        Task<List<ProjectUser>> GetProjectUsers(int companyId, int projectId);
    }
}