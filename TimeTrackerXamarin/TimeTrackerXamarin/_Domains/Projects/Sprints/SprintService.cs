using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using TimeTrackerXamarin._Domains.API;
using TimeTrackerXamarin._Domains.API.dto;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects;

namespace TimeTrackerXamarin._Domains.Projects.Sprints
{
    public class SprintService : ISprintService
    {
        private readonly IFlurlClient client;
        private readonly ITokenService tokenService;

        public SprintService(IFlurlClient client, ITokenService tokenService)
        {
            this.client = client;
            this.tokenService = tokenService;
        }

        public async Task<List<Sprint>> GetSprints(int projectId, int companyId)
        {
            return await RequestHelper.HandleRequest(
                action: async () =>
                {
                    var result = await client.Request("projects", projectId, "sprints")
                        .WithOAuthBearerToken(tokenService.Get())
                        .SetQueryParams(new
                        {
                            selected_company_id = companyId,
                            status = "active"
                        })
                        .GetJsonAsync<JSONDataDto<List<Sprint>>>();

                    return result.data;
                },
                unexpectedError: e => throw e);
        }
    }
}