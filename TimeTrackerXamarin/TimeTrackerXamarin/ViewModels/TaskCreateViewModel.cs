using System;
using Prism.Mvvm;
using Prism.Navigation;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AppCenter.Crashes;
using TimeTrackerXamarin._Domains.API;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Projects;
using TimeTrackerXamarin.i18n;
using TimeTrackerXamarin.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Sprint = TimeTrackerXamarin._UseCases.Contracts.Sprint;

namespace TimeTrackerXamarin.ViewModels
{
    public partial class TaskCreateViewModel : ObservableObject, INavigationAware
    {
        [ObservableProperty]
        private List<Sprint> sprints;
        
        [ObservableProperty]
        private bool isBusy = true;

        [ObservableProperty]
        private bool isSpringSelectionVisible = false;
        
        private string currentProjectId;
        public Sprint SelectedSprint { get; set; }
        public string TaskName { get; set; } = "";
        
        private readonly INavigationService navigationService;
        private readonly CreateTicket createTicket;
        private readonly GetSprints getSprints;
        private readonly IErrorHandler errorHandler;
        private readonly IToastNotification toast;
        private readonly ITranslationManager translation;

        public TaskCreateViewModel(
            INavigationService navigationService, 
            CreateTicket createTicket,
            ITranslationManager translation, 
            GetSprints getSprints,
            IErrorHandler errorHandler,
            IToastNotification toast)
        {
            this.navigationService = navigationService;
            this.createTicket = createTicket;
            this.translation = translation;
            this.getSprints = getSprints;
            this.errorHandler = errorHandler;
            this.toast = toast;
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                navigationService.GoBackAsync();
                toast.ShowWarn(translation.Translate("views.taskcreate.alert.offline"));
                return;
            }

            if (!parameters.TryGetValue("project_id", out currentProjectId))
            {
                navigationService.GoBackAsync();
                toast.ShowWarn(translation.Translate("views.taskcreate.alert.noprojectid"));
                return;
            }

            currentProjectId = parameters.GetValue<string>("project_id");
            getData(currentProjectId);
        }

        private async void getData(string project_id)
        {
            try
            {
                var companyId = int.Parse(Preferences.Get("current_company_id", ""));
                Sprints = await getSprints.GetAll(int.Parse(project_id), companyId);
                if (Sprints.Count == 1)
                {
                    SelectedSprint = Sprints.First();
                    IsSpringSelectionVisible = false;
                }
            }
            catch (Exception e)
            {
                errorHandler.Handle(e);
            }

            IsBusy = false;
            if (Sprints.Count != 1)
            {
                IsSpringSelectionVisible = true;
            }
        }

        [RelayCommand]
        private async void SubmitTask()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await navigationService.GoBackAsync();
                toast.ShowWarn(translation.Translate("views.taskcreate.alert.offline"));
                return;
            }

            try
            {
                if (SelectedSprint?.id == null || TaskName?.Length < 1) return;
                var currentUserId = Preferences.Get("current_user_id", "-1");
                var newTicket = new NewTicket
                {
                    name = TaskName,
                    type_id = 2,
                    estimate_time = 0,
                    sprint_id = SelectedSprint.id,
                    project_id = int.Parse(currentProjectId),
                    reporter_id = int.Parse(currentUserId)
                };
                await createTicket.Create(newTicket, int.Parse(Preferences.Get("current_company_id", "")));
            }
            catch (Exception e)
            {
                errorHandler.Handle(e);
                return;
            }

            toast.ShowInfo(translation.Translate("views.taskcreate.alert.created"));
            MessagingCenter.Send<App>((App)App.Current, "isListLoaded");

            await navigationService.GoBackAsync(("project_id", currentProjectId));
        }
    }
}