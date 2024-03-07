using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects;

namespace TimeTrackerXamarin._UseCases.Projects
{
    public class GetUsers
    {
        private readonly IFactory<IUserService> userServiceFactory;
        private IUserService userService;

        public GetUsers(IFactory<IUserService> userServiceFactory)
        {
            this.userServiceFactory = userServiceFactory;
        }

        public void SwitchConnection(bool connection)
        {
            userService = userServiceFactory.Create(connection);
        }

        public Task<List<User>> GetProjectUsers(int companyId, int projectId)
        {
            return userService.GetProjectUsers(companyId, projectId);
        }
    }
}