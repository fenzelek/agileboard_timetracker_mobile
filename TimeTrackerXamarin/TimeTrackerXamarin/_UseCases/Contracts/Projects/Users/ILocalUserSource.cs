using System.Collections.Generic;
using System.Threading.Tasks;

namespace TimeTrackerXamarin._UseCases.Contracts.Projects.Users
{
    public interface ILocalUserSource
    {
        Task<List<ProjectUser>> GetProjectUsers(int projectId);
        Task<bool> SaveProjectUsers(List<ProjectUser> projectUsers);
    }
}