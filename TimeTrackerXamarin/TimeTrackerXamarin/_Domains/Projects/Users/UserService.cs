using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects;
using TimeTrackerXamarin._UseCases.Contracts.Projects.Users;

namespace TimeTrackerXamarin._Domains.Projects.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository repository;

        public UserService(IUserRepository repository)
        {
            this.repository = repository;
        }

        public Task<List<User>> GetProjectUsers(int companyId, int projectId)
        {
            return repository.GetProjectUsers(companyId, projectId);
        }
    }
}