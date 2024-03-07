using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SQLite;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects.Users;

namespace TimeTrackerXamarin._Domains.Projects.Users
{
    public class LocalUserDataSource : ILocalUserSource
    {
        private readonly SQLiteAsyncConnection db;
        private readonly ILogger logger;

        public LocalUserDataSource(IDatabaseConnector connector)
        {
            db = connector.Create();
            db.CreateTableAsync<ProjectUser>().Wait();
        }

        public async Task<List<ProjectUser>> GetProjectUsers(int projectId)
        {
            try
            {
                var list = await db.Table<ProjectUser>().Where((user) => user.project_id == projectId).ToListAsync();
                if (list == null || list.Count == 0) return new List<ProjectUser>();

                return list;
            }
            catch(Exception err)
            {
                logger.Error("There was an error.", err);
                return new List<ProjectUser>();
            }
        }

        public async Task<bool> SaveProjectUsers(List<ProjectUser> projectUsers)
        {
            try
            {
                await db.ExecuteAsync("DELETE FROM ProjectUser WHERE project_id=?", projectUsers[0].project_id);
                int rowsAdded = 0;
                foreach (ProjectUser user in projectUsers)
                {
                    user.userDB = JsonConvert.SerializeObject(user.user.data);
                    if (await db.InsertOrReplaceAsync(user) == 1)
                        rowsAdded++;
                }
                if (rowsAdded == projectUsers.Count)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception err)
            {
                logger.Error("There was an error.", err);
                return false;
            }
        }
    }
}