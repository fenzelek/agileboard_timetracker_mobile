using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using SQLite;
using TimeTrackerXamarin._Domains.API;
using TimeTrackerXamarin._Domains.API.dto;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects;

namespace TimeTrackerXamarin._Domains.Projects
{
    public class RemoteProjectSource : IRemoteProjectSource
    {

        private readonly ITokenService tokenService;
        private readonly IFlurlClient client;

        public RemoteProjectSource(IFlurlClient client, ITokenService tokenService)
        {
            this.tokenService = tokenService;
            this.client = client;
        }
       
        public async Task<List<Project>> GetAll(int companyId)
        {
            return await RequestHelper.HandleRequest(
                action: async () =>
                {
                    var response = await client.Request("projects")
                        .WithOAuthBearerToken(tokenService.Get())
                        .SetQueryParams(new
                        {
                            status = "opened",
                            selected_company_id = companyId,
                            has_access = 1
                        })
                        .GetJsonAsync<JSONDataDto<List<Project>>>();
                    var data = response.data;
                    return data;
                },
                unexpectedError: e => throw e);
        }
    }
}