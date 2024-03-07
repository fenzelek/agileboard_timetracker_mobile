using System;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;
using Prism.Navigation;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Auth;
using TimeTrackerXamarin.i18n;
using Xamarin.Forms;

namespace TimeTrackerXamarin.Services
{
    public class ErrorHandler : IErrorHandler
    {
        private readonly IToastNotification toast;
        private readonly ITranslationManager translation;
        private readonly ILogger logger;
        private readonly IAuthService authService;
        private readonly INavigationService navigationService;
        public ErrorHandler(ITranslationManager translation, IToastNotification toast, ILogger logger, IAuthService authService, INavigationService navigationService)
        {
            this.translation = translation;
            this.toast = toast;
            this.logger = logger;
            this.authService = authService;
            this.navigationService = navigationService;
        }

        public void Handle(Exception ex)
        {
            switch (ex)
            {
                case ApiErrorException apiEx:
                    var messageKey = apiEx.Code;
                    logger.Error("Api error.", apiEx);
                    toast.ShowError(translation.Translate(messageKey));
                    if (messageKey == "api.auth.invalid_token")
                    {
                        Device.InvokeOnMainThreadAsync(async () => {
                            await authService.Logout();
                            await navigationService.NavigateAsync("/LoginPage");
                        });
                    }
                    break;
                default:
                    //TODO maybe pop up would be better?
                    logger.Error("Unexpected error.", ex);
                    toast.ShowError(translation.Translate("internal-error"));
                    break;
            }
            Crashes.TrackError(ex);
        }
    }
}