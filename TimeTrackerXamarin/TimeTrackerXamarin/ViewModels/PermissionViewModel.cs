using Prism.Mvvm;
using Prism.Navigation;
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Prism.Ioc;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin.i18n;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TimeTrackerXamarin.ViewModels
{
    public partial class PermissionViewModel : ObservableObject
    {
        private readonly INavigationService navigationService;
        private readonly IErrorHandler errorHandler;
        
        public PermissionViewModel(INavigationService navigationService, IErrorHandler errorHandler)
        {
            this.navigationService = navigationService;
            this.errorHandler = errorHandler;
        }

        [RelayCommand]
        async void GetPermission(object obj)
        {
            try
            {
                if (await Permissions.CheckStatusAsync<Permissions.LocationAlways>() == PermissionStatus.Granted)
                {
                    await navigationService.NavigateAsync("/LoginPage");
                    return;
                }
                ContainerLocator.Container.Resolve<ISettingsHelper>().OpenSettings();
                await navigationService.NavigateAsync("/LoginPage");
            }
            catch (Exception err)
            {
                errorHandler.Handle(err);
            }
        }
    }
}