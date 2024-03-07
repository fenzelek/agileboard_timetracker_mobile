using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects;

namespace TimeTrackerXamarin._Domains.Projects
{
    public class LocalProjectSource : ILocalProjectSource
    {
        private readonly SQLiteAsyncConnection db;

        public LocalProjectSource(IDatabaseConnector connector)
        {
            db = connector.Create();
            db.CreateTableAsync<Project>();
        }
        
        public async Task<List<Project>> GetAll(int companyId)
        {
            return await db.Table<Project>().Where((project) => project.company_id == companyId).ToListAsync();
        }

        public async Task<bool> SaveProjects(List<Project> projects)
        {
            if (projects.Count == 0)
            {
                return true;
            }
            await db.ExecuteAsync("DELETE FROM Project WHERE company_id=?", projects[0].company_id);
            int rowsAdded = 0;
            foreach (Project project in projects)
            {
                if (await db.InsertOrReplaceAsync(project) == 1)
                    rowsAdded++;
            }
            return rowsAdded == projects.Count;
        }
    }
}