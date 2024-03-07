using System.Collections.Generic;
using System.Threading.Tasks;

namespace TimeTrackerXamarin._UseCases.Contracts.Projects.Users
{
    public interface IUserRepository
    {
        Task<List<User>> GetProjectUsers(int companyId, int projectId);
    }
}