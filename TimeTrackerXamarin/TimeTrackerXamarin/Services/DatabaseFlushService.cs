using System;
using System.Threading.Tasks;
using SQLite;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Companies;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;

namespace TimeTrackerXamarin.Services
{
    public class DatabaseFlushService : IDatabaseFlushService
    {

        private readonly SQLiteAsyncConnection db;

        public DatabaseFlushService(IDatabaseConnector connector)
        {
            db = connector.Create();
        }

        public async Task Flush()
        {
            try
            {                
                await db.DeleteAllAsync<Company>();
                await db.DeleteAllAsync<Ticket>();
                await db.DeleteAllAsync<TicketDetails>();
                await db.DeleteAllAsync<Project>();
                await db.DeleteAllAsync<TimeFrame>();
                await db.DeleteAllAsync<ProjectUser>();
            }
            catch(Exception err)
            {

            }
        }
    }
}