using System.Collections.Generic;
using Flurl.Http;
using Flurl.Http.Testing;
using Moq;
using TimeTrackerXamarin._Domains.API.dto;
using TimeTrackerXamarin._Domains.Projects.Users;
using TimeTrackerXamarin._UseCases.Contracts;
using Xunit;

namespace TimeTrackerXamarin.Test.Unit.Domain.Projects.Users
{
    public class RemoteUserSourceTest
    {
        private readonly RemoteUserSource source;
        private readonly IFlurlClient client;
        private readonly Mock<ITokenService> tokenService;

        public RemoteUserSourceTest()
        {
            client = new FlurlClient("http://api.api");
            tokenService = new Mock<ITokenService>();
            source = new RemoteUserSource(client, tokenService.Object);
        }

        /*
         * @feature Projects
         * @scenario Get all users
         * @case Gets users from remote api 
         */
        [Fact]
        public async void GetAll_ListUsers()
        {
            using var httpTest = new HttpTest();
            // GIVEN
            var companyId = 1;
            var expectedUser = new ProjectUser
            {
                id = 1,
                user_id = 2,
                project_id = 3
            };

            var response = new JSONDataDto<List<ProjectUser>>
            {
                data = new List<ProjectUser>
                {
                    expectedUser
                }
            };

            httpTest
                .ForCallsTo()
                .WithQueryParam("selected_company_id", companyId)
                .RespondWithJson(response);

            // WHEN
            var result = await source.GetProjectUsers(companyId, expectedUser.project_id);

            // THEN
            Assert.NotEmpty(result);
            Assert.Single(result);
            Assert.Equal(expectedUser.id, result[0].id);
            Assert.Equal(expectedUser.user_id, result[0].user_id);
            Assert.Equal(expectedUser.project_id, result[0].project_id);
        }
    }
}