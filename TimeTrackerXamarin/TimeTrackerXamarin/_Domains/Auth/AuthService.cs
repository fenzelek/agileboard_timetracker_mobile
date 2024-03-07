using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using TimeTrackerXamarin._Domains.API;
using TimeTrackerXamarin._Domains.API.dto;
using TimeTrackerXamarin._Domains.Auth.Dto;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Auth;
using TimeTrackerXamarin.Config;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace TimeTrackerXamarin._Domains.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService tokenService;
        private readonly IFlurlClient client;
        private readonly IPreferences preferences;
        private readonly IDatabaseFlushService flushService;
        private readonly Application application;

        public AuthService(ITokenService tokenService, IFlurlClient client, IPreferences preferences,
            IDatabaseFlushService flushService, Application application)
        {
            this.tokenService = tokenService;
            this.client = client;
            this.preferences = preferences;
            this.flushService = flushService;
            this.application = application;
        }

        public async Task Login(LoginDto data)
        {
            if (data.email == "" || data.password == "")
            {
                throw new ApiErrorException("api.auth.empty_data");
            }

            await RequestHelper.HandleRequest(
                action: async () =>
                {
                    var result = await client.Request("auth")
                        .PostJsonAsync(data)
                        .ReceiveJson<JSONDataDto<TokenDto>>();

                    tokenService.Set(result.data.token);
                },
                unexpectedError: ex => throw ex);
        }

        public async Task Logout()
        {
            await flushService.Flush();
            preferences.Remove("current_user_name");
            preferences.Remove("current_company_name");
            preferences.Remove("current_company_id");
            application.Properties.Remove("saved_user");
            tokenService.Remove();
        }

        public async Task<User> Current()
        {
            return await RequestHelper.HandleRequest(
                action: async () =>
                {
                    var response = await client.Request("users", "current")
                        .WithOAuthBearerToken(tokenService.Get())
                        .GetJsonAsync<JSONDataDto<User>>();

                    return response.data;
                },
                unexpectedError: ex => throw ex);
        }

        public Task<bool> CheckLogin()
        {
            return Task.FromResult(tokenService.Get() != "");
        }

        public async Task<string> SaveAvatar(string avatar)
        {
            var img = await RequestHelper.HandleRequest(
                action: async () => await client.Request("users", "avatar", avatar)
                    .WithOAuthBearerToken(tokenService.Get())
                    .GetBytesAsync(),
                unexpectedError: ex => throw ex);
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                avatar);
            File.WriteAllBytes(filePath, img);
            return filePath;
        }
    }
}