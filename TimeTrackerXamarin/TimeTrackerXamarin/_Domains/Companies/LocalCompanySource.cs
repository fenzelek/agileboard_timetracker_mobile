using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Companies;

namespace TimeTrackerXamarin._Domains.Companies
{
    public class LocalCompanySource: ILocalCompanySource
    {
        private readonly SQLiteAsyncConnection db;

        public LocalCompanySource(IDatabaseConnector dbConnector)
        {
            db = dbConnector.Create();
            db.CreateTableAsync<Company>().Wait();
        }

        public Task<List<Company>> GetCompanies()
        {
            return Task.FromResult(db.Table<Company>().ToListAsync().Result);
        }

        public async Task<bool> SaveCompanies(List<Company> companies)
        {
            await db.DropTableAsync<Company>();
            await db.CreateTableAsync<Company>();

            await db.InsertAllAsync(companies,false);            
            return true;
        }
    }
}
