using System.Collections.Generic;
using Moq;
using TimeTrackerXamarin._Domains.Projects.Users;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects.Users;
using Xunit;

namespace TimeTrackerXamarin.Test.Unit.Domain.Projects.Users
{
    public class RemoteUserRepositoryTest
    {

        private readonly RemoteUserRepository repository;
        private readonly Mock<ILocalUserSource> localSource;
        private readonly Mock<IRemoteUserSource> remoteSource;


        public RemoteUserRepositoryTest()
        {
            localSource = new Mock<ILocalUserSource>();
            remoteSource = new Mock<IRemoteUserSource>();
            var userMapper = new Mock<IMapper<List<User>, List<ProjectUser>>>();

            repository = new RemoteUserRepository(remoteSource.Object, localSource.Object, userMapper.Object);
        }

        /*
         * @feature Projects
         * @scenario Get all project users from repository
         * @case Lists users from remote data source
         */
        [Fact]
        public async void GetAll_ListUsers()
        {
            // GIVEN
            var companyId = 1;
            var projectId = 2;
            var expectedUsers = new List<ProjectUser>
            {
                new ProjectUser
                {
                    id = 3
                }
            };
            remoteSource.Setup(mock => mock.GetProjectUsers(companyId, projectId))
                .ReturnsAsync(() => expectedUsers);

            // WHEN
            await repository.GetProjectUsers(companyId, projectId);

            // THEN
            remoteSource.Verify(mock => mock.GetProjectUsers(companyId, projectId), Times.Once);
            localSource.Verify(mock => mock.SaveProjectUsers(expectedUsers), Times.Once);
            localSource.Verify(mock => mock.GetProjectUsers(It.IsAny<int>()), Times.Never);
        }
    }
}