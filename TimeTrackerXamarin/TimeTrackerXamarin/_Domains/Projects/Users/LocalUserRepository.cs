using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects.Users;

namespace TimeTrackerXamarin._Domains.Projects.Users
{
    public class LocalUserRepository : IUserRepository
    {
        private readonly ILocalUserSource source;
        private readonly IMapper<List<User>, List<ProjectUser>> userMapper;

        public LocalUserRepository(ILocalUserSource source, IMapper<List<User>, List<ProjectUser>> userMapper)
        {
            this.source = source;
            this.userMapper = userMapper;
        }

        public async Task<List<User>> GetProjectUsers(int companyId, int projectId)
        {
            var projectUsers = await source.GetProjectUsers(projectId);
            return userMapper.MapDB(projectUsers);
        }
    }
}