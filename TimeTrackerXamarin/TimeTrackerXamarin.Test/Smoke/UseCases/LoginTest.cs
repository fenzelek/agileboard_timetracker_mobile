using System.Threading.Tasks;
using Moq;
using TimeTrackerXamarin._Domains.Auth.Dto;
using TimeTrackerXamarin._UseCases.Auth;
using TimeTrackerXamarin._UseCases.Contracts.Auth;
using Xunit;

namespace TimeTrackerXamarin.Test.Smoke.UseCases
{
    public class LoginTest
    {
        private Mock<IAuthService> authService; 
        private Login login;
        public LoginTest()
        {
            authService = new Mock<IAuthService>();
            login = new Login(authService.Object);
        }

        /**
         * @feature: TimeTracking
         * @scenario: Save frame of time without end
         * @case: Return created frame of time without end
         */
        [Fact]
        public async void Login()
        {
            //GIVEN
            LoginDto loginDto = new LoginDto { email = "user@site.com", password = "password" };
            authService.Setup((auth) => auth.Login(loginDto)).Returns(Task.FromResult(true));

            //WHEN
            // bool result = await login.Exec(loginDto);

            //THEN
            // Assert.True(result);
        }
    }
}
