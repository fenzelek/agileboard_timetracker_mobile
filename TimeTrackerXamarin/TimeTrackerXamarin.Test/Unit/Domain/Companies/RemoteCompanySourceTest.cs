using System.Collections.Generic;
using Flurl.Http;
using Flurl.Http.Testing;
using Moq;
using Newtonsoft.Json;
using TimeTrackerXamarin._Domains.API.dto;
using TimeTrackerXamarin._Domains.Companies;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Companies;
using Xunit;

namespace TimeTrackerXamarin.Test.Unit.Domain.Companies
{
    public class RemoteCompanySourceTest
    {
        private readonly Mock<IFlurlClient> flurlClient;
        private readonly Mock<ITokenService> tokenService;
        private HttpTest httpTest;
        private RemoteCompanySource remoteCompanySource;

        public RemoteCompanySourceTest()
        {
            flurlClient = new Mock<IFlurlClient>();
            httpTest = new HttpTest();
            tokenService = new Mock<ITokenService>();

            remoteCompanySource = new RemoteCompanySource(new FlurlClient("http://api.com"), tokenService.Object);
        }

        /*
         * @feature Company
         * @scenario Get all companies
         * @case List companies
         */
        [Fact]
        public async void GetAll_listCompanies()
        {
            using (HttpTest test = new HttpTest())
            {
                //GIVEN
                var expectedCompanyList =
                    new JSONDataDto<List<Company>>
                    {
                        data = new List<Company>
                        {
                            new Company { id = 1, name = "Company" }
                        }
                    };
                httpTest.RespondWith(JsonConvert.SerializeObject(expectedCompanyList));

                tokenService.Setup((x) => x.Get()).Returns("token");


                //WHEN
                test.RespondWith(JsonConvert.SerializeObject(expectedCompanyList));
                var result = await remoteCompanySource.GetAll();


                //THEN
                Assert.Equal(expectedCompanyList.data[0].id, result[0].id);
                Assert.Equal(expectedCompanyList.data[0].name, result[0].name);
            }
        }
    }
}