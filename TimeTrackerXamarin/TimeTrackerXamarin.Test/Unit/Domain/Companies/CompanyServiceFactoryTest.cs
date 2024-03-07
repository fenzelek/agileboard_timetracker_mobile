using Moq;
using TimeTrackerXamarin._Domains.Companies;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Companies;
using Xunit;

namespace TimeTrackerXamarin.Test.Unit.Domain.Companies
{
    public class CompanyServiceFactoryTest
    {
        private IFactory<ICompanyService> factory;
        private readonly Mock<IRemoteCompanySource> remoteMock;
        private readonly Mock<ILocalCompanySource> localMock;
        public CompanyServiceFactoryTest()
        {
            remoteMock = new Mock<IRemoteCompanySource>();
            localMock = new Mock<ILocalCompanySource>();
            factory = new CompanyServiceFactory(remoteMock.Object,localMock.Object); 
        }

        [Fact]
        public void Create_RemoteRepository()
        {
            //GIVEN
            bool connection = true;

            //WHEN
            var result = factory.Create(connection);
            result.GetCompanies();
            
            //THEN
            var type = result.GetType();
            Assert.IsType<CompanyService>(result);
            remoteMock.Verify((mock) => mock.GetAll(), Times.Once);
            localMock.Verify((mock)=> mock.GetCompanies(), Times.Never);
        }
        [Fact]
        public void Create_LocalRepository()
        {
            //GIVEN
            bool connection = false;

            //WHEN
            var result = factory.Create(connection);
            var companies = result.GetCompanies();

            //THEN
            var type = result.GetType();
            Assert.IsType<CompanyService>(result);
            remoteMock.Verify((mock) => mock.GetAll(), Times.Never);
            localMock.Verify((mock)=> mock.GetCompanies(), Times.Once);
        }
    }
}