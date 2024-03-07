using System;
using System.Collections.Generic;
using Flurl.Http;
using Flurl.Http.Testing;
using Moq;
using Newtonsoft.Json;
using TimeTrackerXamarin._Domains.API.dto;
using TimeTrackerXamarin._Domains.Auth;
using TimeTrackerXamarin._Domains.Auth.Dto;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin.Test.Mock;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;
using Xunit;

namespace TimeTrackerXamarin.Test.Unit.Domain.Auth
{
    public class AuthServiceTest
    {
        private Mock<ITokenService> tokenService;
        private Mock<IPreferences> preferences;
        private Mock<IDatabaseFlushService> databaseFlushService;
        private readonly AuthService authService;
        private readonly HttpTest httpTest;


        public AuthServiceTest()
        {
            //XamarinFormsMock.Init();
            tokenService = new Mock<ITokenService>();
            preferences = new Mock<IPreferences>();
            databaseFlushService = new Mock<IDatabaseFlushService>();
            httpTest = new HttpTest();

            authService = new AuthService(tokenService.Object, new FlurlClient("http://api.com"), preferences.Object,
                databaseFlushService.Object, null);
            //TODO remove null, mock forms app
        }

        /**
         * @feature: Auth
         * @scenario: Login user
         * @case: Login successfully 
         */
        [Fact]
        public async void Login_true()
        {
            //GIVEN            
            LoginDto loginDto = new LoginDto { email = "email@email.com", password = "password" };
            JSONDataDto<TokenDto> json = new JSONDataDto<TokenDto>
            {
                data = new TokenDto
                {
                    token = "token"
                }
            };
            httpTest.RespondWith(JsonConvert.SerializeObject(json));

            //WHEN
            await authService.Login(loginDto);

            //THEN
            tokenService.Verify((x) => x.Set("token"), Times.Once);
        }


        /**
         * @feature: Auth
         * @scenario: Login user
         * @case: Fail login user with empty login data
         */
        [Fact]
        public void Login_false()
        {
            //GIVEN            
            LoginDto loginDto = new LoginDto { email = "", password = "" };

            //WHEN & THEN
            Assert.ThrowsAsync<Exception>(async () => await authService.Login(loginDto));
        }

        /**
         * @feature: Auth
         * @scenario: Logout user
         * @case: Logout successfully, delete token 
         */
        [Fact(Skip = "Mocking Forms Application breaks other tests.")]
        public async void Logout_Success()
        {
            //GIVEN
            Application.Current.Properties["saved_user"] = "saved_user_data";

            //WHEN
            await authService.Logout();

            //THEN
            databaseFlushService.Verify(x => x.Flush(), Times.Once);
            preferences.Verify(x => x.Remove("current_user_name"), Times.Once);
            preferences.Verify(x => x.Remove("current_company_name"), Times.Once);
            preferences.Verify(x => x.Remove("current_company_id"), Times.Once);
            tokenService.Verify(x => x.Remove(), Times.Once);
            
            Assert.False(Application.Current.Properties.ContainsKey("saved_user"));
        }

        /**
         * @feature: Auth
         * @scenario: Save avatar
         * @case: User avatar saved in cache 
         */
        [Fact]
        public async void SaveAvatar_Success()
        {
            //GIVEN            
            httpTest.RespondWith("OK", status: 200);

            //WHEN
            var result = await authService.SaveAvatar("avatar.jpg");

            //THEN
            tokenService.Verify((x) => x.Get(), Times.Once);
        }

        /**
         * @feature: Auth
         * @scenario: Get current
         * @case: Get data of currently logged user 
         */
        [Fact]
        public async void GetCurrent_Success()
        {
            //GIVEN            
            JSONDataDto<User> expected = new JSONDataDto<User>
            {
                data = new User
                {
                    first_name = "User",
                    last_name = "Name",
                    id = 1,
                    role_id = 0
                }
            };
            httpTest.RespondWith(JsonConvert.SerializeObject(expected), status: 200);

            //WHEN
            var result = await authService.Current();

            //THEN
            Assert.Equal(expected.data.first_name, result.first_name);
            Assert.Equal(expected.data.last_name, result.last_name);
            tokenService.Verify((x) => x.Get(), Times.Once);
        }
    }
}