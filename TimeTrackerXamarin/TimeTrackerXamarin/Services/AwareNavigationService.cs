using System;
using System.Threading.Tasks;
using Prism.Behaviors;
using Prism.Common;
using Prism.Ioc;
using Prism.Navigation;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin.i18n;

namespace TimeTrackerXamarin.Services
{

    public class AwarePageNavigationService : PageNavigationService
    {
        private readonly IToastNotification toastNotification;
        private readonly ITranslationManager translationManager;
        private readonly ILogger logger;

        public AwarePageNavigationService(IContainerProvider container, IApplicationProvider applicationProvider,
            IPageBehaviorFactory pageBehaviorFactory, IToastNotification toastNotification,
            ITranslationManager translationManager, ILogger logger) : base(container, applicationProvider, pageBehaviorFactory)
        {
            this.toastNotification = toastNotification;
            this.translationManager = translationManager;
            this.logger = logger;
        }

        protected override Task<INavigationResult> GoBackInternal(INavigationParameters parameters,
            bool? useModalNavigation, bool animated)
        {
            return Handle(base.GoBackInternal(parameters, useModalNavigation, animated));
        }

        protected override Task<INavigationResult> GoBackToRootInternal(INavigationParameters parameters)
        {
            return Handle(base.GoBackToRootInternal(parameters));
        }

        protected override Task<INavigationResult> NavigateInternal(string name, INavigationParameters parameters,
            bool? useModalNavigation, bool animated)
        {
            return Handle(base.NavigateInternal(name, parameters, useModalNavigation, animated));
        }

        protected override Task<INavigationResult> NavigateInternal(Uri uri, INavigationParameters parameters,
            bool? useModalNavigation, bool animated)
        {
            return Handle(base.NavigateInternal(uri, parameters, useModalNavigation, animated));
        }

        private async Task<INavigationResult> Handle(Task<INavigationResult> navigationResult)
        {
            var result = await navigationResult;
            if (!result.Success)
            {
                logger.Error("There was an error while navigating.", result.Exception);
                toastNotification.ShowError(translationManager.Translate("internal-error"));
            }

            return result;
        }
    }
}