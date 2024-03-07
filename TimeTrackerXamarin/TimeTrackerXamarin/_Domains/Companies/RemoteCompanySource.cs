using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using TimeTrackerXamarin._Domains.API;
using TimeTrackerXamarin._Domains.API.dto;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Companies;

namespace TimeTrackerXamarin._Domains.Companies
{
    public class RemoteCompanySource : IRemoteCompanySource
    {
        private readonly IFlurlClient client;
        private readonly ITokenService tokenService;

        public RemoteCompanySource(IFlurlClient client, ITokenService tokenService)
        {
            this.client = client; ;
            this.tokenService = tokenService;
        }

        public async Task<List<Company>> GetAll()
        {
            return await RequestHelper.HandleRequest(
                action: async () =>
                {
                    var result = await client.Request("users", "current", "companies")
                        .WithOAuthBearerToken(tokenService.Get())
                        .GetJsonAsync<JSONDataDto<List<Company>>>();

                    var data = result.data;
                    return data;
                },
                unexpectedError: ex=> throw ex);
        }
    }
}