using System.Collections.Generic;
using Newtonsoft.Json;
using TimeTrackerXamarin._UseCases.Contracts;

namespace TimeTrackerXamarin._Domains.Projects.Users
{
    public class ProjectUserMapper:IMapper<List<User>,List<ProjectUser>>
    {
        public List<User> Map(List<ProjectUser> projectUsers)
        {
            if (projectUsers == null || projectUsers.Count < 1) return null;
            List<User> result = new List<User>();
            foreach(ProjectUser user in projectUsers)
            {
                string usrDB = JsonConvert.SerializeObject(user.user.data);
                user.userDB = usrDB;
                result.Add(user.user.data);
            }
            return result;
        }
        public List<User> MapDB(List<ProjectUser> projectUsers)
        {
            if (projectUsers == null || projectUsers.Count < 1) return null;
            List<User> result = new List<User>();
            foreach (ProjectUser user in projectUsers)
            {                
                result.Add(JsonConvert.DeserializeObject<User>(user.userDB));
            }
            return result;
        }
    }
}
