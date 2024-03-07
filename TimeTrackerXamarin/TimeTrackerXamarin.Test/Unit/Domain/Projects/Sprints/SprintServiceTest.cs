using System.Collections.Generic;
using Flurl.Http;
using Flurl.Http.Testing;
using Moq;
using TimeTrackerXamarin._Domains.API.dto;
using TimeTrackerXamarin._Domains.Projects.Sprints;
using TimeTrackerXamarin._UseCases.Contracts;
using Xunit;

namespace TimeTrackerXamarin.Test.Unit.Domain.Projects.Sprints
{
    public class SprintServiceTest
    {
        private readonly SprintService service;
        private readonly IFlurlClient client;
        private readonly Mock<ITokenService> tokenService;

        public SprintServiceTest()
        {
            client = new FlurlClient("http://api.api");
            tokenService = new Mock<ITokenService>();
            service = new SprintService(client, tokenService.Object);
        }

        /*
         * @feature Projects
         * @scenario Get all sprints
         * @case Gets sprints from remote api    
         */
        [Fact]
        public async void GetSprints_All()
        {
            using var httpTest = new HttpTest();
            // GIVEN
            var companyId = 1;
            var projectId = 3;
            var expectedSprint = new Sprint
            {
                id = 1,
                project_id = projectId,
                name = "sprint",
                tickets = new JSONDataDto<List<Ticket>>
                {
                    data = new List<Ticket>()
                }
            };

            var response = new JSONDataDto<List<Sprint>>
            {
                data = new List<Sprint>
                {
                    expectedSprint
                }
            };

            httpTest
                .ForCallsTo($"*projects/{projectId}/sprints*")
                .WithQueryParam("selected_company_id", companyId)
                .RespondWithJson(response);

            // WHEN
            var result = await service.GetSprints(projectId, companyId);

            // THEN
            Assert.NotEmpty(result);
            Assert.Single(result);
            Assert.Equivalent(expectedSprint, result[0]);
        }
    }
}