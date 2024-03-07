using System.Collections.Generic;
using System.Threading.Tasks;

namespace TimeTrackerXamarin._UseCases.Contracts.Projects
{
    public interface IUserService
    {
        Task<List<User>> GetProjectUsers(int companyId, int projectId);
    }
}