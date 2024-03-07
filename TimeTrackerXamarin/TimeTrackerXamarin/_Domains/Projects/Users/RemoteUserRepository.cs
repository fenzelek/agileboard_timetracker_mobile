using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using TimeTrackerXamarin._Domains.API;
using TimeTrackerXamarin._Domains.API.dto;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects.Users;

namespace TimeTrackerXamarin._Domains.Projects.Users
{
    public class RemoteUserRepository : IUserRepository
    {
        private readonly IRemoteUserSource remoteSource;
        private readonly ILocalUserSource localSource;
        private readonly IMapper<List<User>, List<ProjectUser>> userMapper;
        
        public RemoteUserRepository(IRemoteUserSource remoteSource, ILocalUserSource localSource, IMapper<List<User>, List<ProjectUser>> userMapper)
        {
            this.remoteSource = remoteSource;
            this.localSource = localSource;
            this.userMapper = userMapper;
        }

        public async Task<List<User>> GetProjectUsers(int companyId, int projectId)
        {
            var users = await remoteSource.GetProjectUsers(companyId, projectId);
            await localSource.SaveProjectUsers(users);
            return userMapper.Map(users);
        }
    }
}