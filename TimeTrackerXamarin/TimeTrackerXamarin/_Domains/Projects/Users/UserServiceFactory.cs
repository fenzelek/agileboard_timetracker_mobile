using System.Collections.Generic;
using Flurl.Http;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects;
using TimeTrackerXamarin._UseCases.Contracts.Projects.Users;

namespace TimeTrackerXamarin._Domains.Projects.Users
{
    public class UserServiceFactory : IFactory<IUserService>
    {

        private readonly IRemoteUserSource remoteSource;
        private readonly ILocalUserSource localSource;
        private readonly IMapper<List<User>, List<ProjectUser>> userMapper;

        public UserServiceFactory(IRemoteUserSource remoteSource, ILocalUserSource localSource, IMapper<List<User>, List<ProjectUser>> userMapper)
        {
            this.remoteSource = remoteSource;
            this.localSource = localSource;
            this.userMapper = userMapper;
        }

        private IUserRepository CreateRepository(bool connection)
        {
            if (connection)
            {
                return new RemoteUserRepository(remoteSource, localSource, userMapper);
            }

            return new LocalUserRepository(localSource, userMapper);
        }

        public IUserService Create(bool connection)
        {
            var repository = CreateRepository(connection);
            return new UserService(repository);
        }
    }
}