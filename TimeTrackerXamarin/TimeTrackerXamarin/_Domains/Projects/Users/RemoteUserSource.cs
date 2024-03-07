using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using TimeTrackerXamarin._Domains.API;
using TimeTrackerXamarin._Domains.API.dto;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects.Users;

namespace TimeTrackerXamarin._Domains.Projects.Users
{
    public class RemoteUserSource : IRemoteUserSource
    {
        private readonly IFlurlClient client;
        private readonly ITokenService tokenService;

        public RemoteUserSource(IFlurlClient client, ITokenService tokenService)
        {
            this.client = client;
            this.tokenService = tokenService;
        }

        public async Task<List<ProjectUser>> GetProjectUsers(int companyId, int projectId)
        {
            return await RequestHelper.HandleRequest(
                action: async () =>
                {
                    var result = await client.Request("projects", projectId, "users")
                        .WithOAuthBearerToken(tokenService.Get())
                        .SetQueryParams(new
                        {
                            selected_company_id = companyId,
                        })
                        .GetJsonAsync<JSONDataDto<List<ProjectUser>>>();

                    return result.data;
                },
                unexpectedError: e => throw e);
        }
    }
}