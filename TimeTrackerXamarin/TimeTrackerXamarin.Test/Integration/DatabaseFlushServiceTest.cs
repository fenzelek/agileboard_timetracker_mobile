using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using SQLite;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Companies;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin.Services;
using Xunit;

namespace TimeTrackerXamarin.Test.Integration
{
    public class DatabaseFlushServiceTest
    {
        private readonly Mock<IDatabaseConnector> databaseConnector;
        private SQLiteAsyncConnection localDatabase;
        private readonly DatabaseFlushService databaseFlushService;

        public DatabaseFlushServiceTest()
        {
            localDatabase = new SQLiteAsyncConnection(":memory:");
            databaseConnector = new Mock<IDatabaseConnector>();
            databaseConnector.Setup((conn) => conn.Create()).Returns(localDatabase);

            databaseFlushService = new DatabaseFlushService(databaseConnector.Object);
        }

        
        /*
        * @feature Services
        * @scenario Database flush data
        * @case Database has no data
        */
        [Fact]
        public async void Flush_void()
        {
            //GIVEN
            await SeedDatabase(localDatabase);
            
            //WHEN
            await databaseFlushService.Flush();

            //THEN
            List<int> checkSum = new List<int>()
            {
                await localDatabase.Table<Ticket>().CountAsync(),
                await localDatabase.Table<TicketDetails>().CountAsync(),
                await localDatabase.Table<ProjectUser>().CountAsync(),
                await localDatabase.Table<TimeFrame>().CountAsync(),
                await localDatabase.Table<Project>().CountAsync(),
                await localDatabase.Table<Company>().CountAsync()
            };
            Assert.Equal(0, checkSum.Sum());

        }


        private async Task SeedDatabase(SQLiteAsyncConnection conn)
        {
            await conn.CreateTableAsync<Ticket>();
            await conn.CreateTableAsync<TicketDetails>();
            await conn.CreateTableAsync<ProjectUser>();
            await conn.CreateTableAsync<TimeFrame>();
            await conn.CreateTableAsync<Project>();
            await conn.CreateTableAsync<Company>();
            await conn.InsertAsync(new Ticket { name = "Test" });
            await conn.InsertAsync(new TicketDetails() { name = "Test" });
            await conn.InsertAsync(new ProjectUser() { userDB = "Test" });
            await conn.InsertAsync(new TimeFrame() { gpsPositionDB = "Test" });
            await conn.InsertAsync(new Project() { name = "Test" });
            await conn.InsertAsync(new Company() { name = "Test" });
        }


    }
}