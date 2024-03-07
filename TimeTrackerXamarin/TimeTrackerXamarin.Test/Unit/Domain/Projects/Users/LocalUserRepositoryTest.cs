using System.Collections.Generic;
using Moq;
using TimeTrackerXamarin._Domains.Projects.Users;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects.Users;
using Xunit;

namespace TimeTrackerXamarin.Test.Unit.Domain.Projects.Users
{
    public class LocalUserRepositoryTest
    {

        private readonly LocalUserRepository repository;
        private readonly Mock<ILocalUserSource> source;

        public LocalUserRepositoryTest()
        {
            source = new Mock<ILocalUserSource>();
            var userMapper = new Mock<IMapper<List<User>, List<ProjectUser>>>();

            repository = new LocalUserRepository(source.Object, userMapper.Object);
        }

        /*
         * @feature Projects
         * @scenario Get all project users from repository
         * @case Lists users from local data source
         */
        [Fact]
        public async void GetAll_ListUsers()
        {
            // GIVEN
            var companyId = 1;
            var projectId = 2;

            // WHEN
            await repository.GetProjectUsers(companyId, projectId);

            // THEN
            source.Verify(mock => mock.GetProjectUsers(projectId), Times.Once);
            source.Verify(mock => mock.SaveProjectUsers(It.IsAny<List<ProjectUser>>()), Times.Never);
        }
    }
}