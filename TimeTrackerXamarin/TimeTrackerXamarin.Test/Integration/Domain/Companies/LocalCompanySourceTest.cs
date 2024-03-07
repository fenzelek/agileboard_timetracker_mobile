using System.Collections.Generic;
using Moq;
using SQLite;
using TimeTrackerXamarin._Domains.Companies;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Companies;
using Xunit;

namespace TimeTrackerXamarin.Test.Integration.Domain.Companies
{
    public class LocalCompanySourceTest
    {
        private Mock<IDatabaseConnector> databaseConnectorMock;
        private LocalCompanySource localCompanySource; 
        
        //todo integration DB test
        public LocalCompanySourceTest()
        {
            databaseConnectorMock = new Mock<IDatabaseConnector>();
            databaseConnectorMock.Setup((db) => db.Create()).Returns(new SQLiteAsyncConnection("local.db3"));
            
            localCompanySource = new LocalCompanySource(databaseConnectorMock.Object);
            
        }

        /**
         * @feature Companies
         * @scenario Get companies from local database
         * @case Companies returned
         */
        [Fact]
        public async void GetCompanies_listCompanies()
        {
            //GIVEN
            List<Company> expectedList = new List<Company>
            {
                new Company{id=1, name="Company"}
            };
            var data = new SQLiteAsyncConnection("local.db3");
            await data.InsertAsync(expectedList[0]);
            
            
            //WHEN
            databaseConnectorMock.Setup((db) => db.Create()).Returns(new SQLiteAsyncConnection("local.db3"));
            var result = await localCompanySource.GetCompanies();
            
            //THEN
            Assert.Equal(expectedList[0].name, result[0].name);
        }
    }
}