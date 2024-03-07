using Moq;
using TimeTrackerXamarin._Domains.Projects.Users;
using TimeTrackerXamarin._UseCases.Contracts.Projects.Users;
using Xunit;

namespace TimeTrackerXamarin.Test.Unit.Domain.Projects.Users
{
    public class UserServiceTest
    {

        private readonly UserService service;
        private readonly Mock<IUserRepository> repository;

        public UserServiceTest()
        {
            repository = new Mock<IUserRepository>();
            service = new UserService(repository.Object);
        }

        /*
         * @feature Projects
         * @scenario Get all project users from service
         * @case Lists all project users    
         */
        [Fact]
        public async void GetAll_ListUsers()
        {
            // GIVEN
            var companyId = 1;
            var projectId = 2;

            // WHEN
            await service.GetProjectUsers(companyId, projectId);

            // THEN
            repository.Verify(mock => mock.GetProjectUsers(companyId, projectId), Times.Once);
        }
    }
}