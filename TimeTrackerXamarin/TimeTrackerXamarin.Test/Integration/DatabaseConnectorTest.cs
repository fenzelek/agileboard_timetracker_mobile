using SQLite;
using TimeTrackerXamarin.Services;
using Xunit;

namespace TimeTrackerXamarin.Test.Integration
{
    public class DatabaseConnectorTest
    {
        private readonly DatabaseConnector dbConnector;

        public DatabaseConnectorTest()
        {
            dbConnector = new DatabaseConnector();
        }
        /*
        * @feature Services
        * @scenario Get database connector
        * @case Database connector
        */
        [Fact]
        public void Create_SqliteAsyncConnection()
        {
            //GIVEN
            var dbName = "timetracker.db3";
            
            //WHEN
            var result = dbConnector.Create();

            //THEN
            Assert.IsType<SQLiteAsyncConnection>(result);
            Assert.Contains(dbName, result.DatabasePath);
        }
    }
}