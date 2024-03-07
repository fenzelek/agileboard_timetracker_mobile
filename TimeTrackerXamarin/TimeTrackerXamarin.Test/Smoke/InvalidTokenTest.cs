using Moq;
using Prism.Navigation;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Auth;
using TimeTrackerXamarin.i18n;
using TimeTrackerXamarin.Services;
using Xunit;

namespace TimeTrackerXamarin.Test.Smoke
{
    public class InvalidTokenTest
    {
        private readonly Mock<ITranslationManager> translationManager;
        private readonly Mock<ILogger> logger;
        private readonly Mock<IToastNotification> toastNotification;
        private readonly Mock<IAuthService> authService;
        private readonly Mock<INavigationService> navigationService;

        private readonly ErrorHandler errorHandler;

        public InvalidTokenTest()
        {
            translationManager = new Mock<ITranslationManager>();
            logger = new Mock<ILogger>();
            toastNotification = new Mock<IToastNotification>();
            authService = new Mock<IAuthService>();
            navigationService = new Mock<INavigationService>();

            errorHandler = new ErrorHandler(translationManager.Object, toastNotification.Object, logger.Object,
                authService.Object, navigationService.Object);
        }

        [Fact]
        public void ErrorHandlerLogoutOnInvalidToken()
        {
            //GIVEN
            var exception = new ApiErrorException("api.auth.invalid_token");
            
            //WHEN
            errorHandler.Handle(exception);
            
            //THEN
            authService.Verify(x => x.Logout(), Times.Once);
        }
    }
}