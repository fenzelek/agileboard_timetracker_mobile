using System.Collections.Generic;
using Moq;
using TimeTrackerXamarin._Domains.Projects.Users;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects.Users;
using Xunit;

namespace TimeTrackerXamarin.Test.Unit.Domain.Projects.Users
{
    public class UserServiceFactoryTest
    {
        private readonly UserServiceFactory factory;
        private readonly Mock<IRemoteUserSource> remote;
        private readonly Mock<ILocalUserSource> local;

        public UserServiceFactoryTest()
        {
            remote = new Mock<IRemoteUserSource>();
            local = new Mock<ILocalUserSource>();
            var userMapper = new Mock<IMapper<List<User>, List<ProjectUser>>>();

            factory = new UserServiceFactory(remote.Object, local.Object, userMapper.Object);
        }

        /*
         * @feature Projects
         * @scenario Create user repository
         * @case With internet connection, creates remote repository                                             
         */
        [Fact]
        public async void Create_RemoteRepository()
        {
            //GIVEN
            var companyId = 1;
            var projectId = 2;
            var connection = true;
            var expectedUsers = new List<ProjectUser>
            {
                new ProjectUser
                {
                    id = 3
                }
            };
            remote.Setup(mock => mock.GetProjectUsers(companyId, projectId))
                .ReturnsAsync(() => expectedUsers);

            //WHEN
            var result = factory.Create(connection);
            await result.GetProjectUsers(companyId, projectId);

            //THEN
            remote.Verify(mock => mock.GetProjectUsers(companyId, projectId), Times.Once);
            local.Verify(mock => mock.SaveProjectUsers(expectedUsers), Times.Once);
            local.Verify(mock => mock.GetProjectUsers(projectId), Times.Never);
        }

        /*
         * @feature Projects
         * @scenario Create user repository
         * @case Without internet connection, creates local repository                                           
         */
        [Fact]
        public async void Create_LocalRepository()
        {
            //GIVEN
            var companyId = 1;
            var projectId = 2;
            var connection = false;
            var expectedUsers = new List<ProjectUser>
            {
                new ProjectUser
                {
                    id = 3
                }
            };
            local.Setup(mock => mock.GetProjectUsers(projectId))
                .ReturnsAsync(() => expectedUsers);

            //WHEN
            var result = factory.Create(connection);
            await result.GetProjectUsers(companyId, projectId);

            //THEN
            remote.Verify(mock => mock.GetProjectUsers(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            local.Verify(mock => mock.SaveProjectUsers(It.IsAny<List<ProjectUser>>()), Times.Never);
            local.Verify(mock => mock.GetProjectUsers(projectId), Times.Once);
        }
        
    }
}