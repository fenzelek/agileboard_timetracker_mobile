using Prism.AppModel;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin._UseCases.Projects;
using TimeTrackerXamarin._UseCases.TimeTracking;
using TimeTrackerXamarin.i18n;
using TimeTrackerXamarin.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TimeTrackerXamarin.ViewModels
{
    public partial class ProjectsListViewModel : ObservableObject, IPageLifecycleAware
    {
        [ObservableProperty]
        private long ticketTime;
        
        [ObservableProperty]
        private long totalTime;

        [ObservableProperty]
        private bool isButtonChanging;

        [ObservableProperty]
        private bool isPlayEnabled;

        [ObservableProperty]
        private bool isTracking;
        
        [ObservableProperty]
        private Ticket currentTask;
        
        [ObservableProperty]
        private List<ProjectWithSummary> listProject;

        [ObservableProperty]
        private bool isBusy;

        private readonly INavigationService navigationService;
        private readonly IErrorHandler errorHandler;
        private readonly IToastNotification toast;
        private readonly TimeSummary timeSummary;
        private readonly StartTracking startTracking;
        private readonly StopTracking stopTracking;
        private readonly CurrentTracking currentTracking;
        private readonly ITranslationManager translationManager;
        private readonly IMessagingCenter messagingCenter;
        private readonly Timer timer;
        private readonly GetProjects getProjects;
        private readonly GetUnfinishedFrame getUnfinishedFrame;
        private readonly GetTickets getTickets;
        private readonly GetTimeSummary getTimeSummary;
        
        public ProjectsListViewModel(INavigationService navigationService, IErrorHandler errorHandler, GetProjects getProjects, GetUnfinishedFrame getUnfinishedFrame, GetTickets getTickets, IToastNotification toast, StartTracking startTracking, StopTracking stopTracking, CurrentTracking currentTracking, ITranslationManager translationManager, GetTimeSummary getTimeSummary,IMessagingCenter messagingCenter)
        {
            this.navigationService = navigationService;
            this.errorHandler = errorHandler;
            this.toast = toast;
            this.startTracking = startTracking;
            this.stopTracking = stopTracking;
            this.currentTracking = currentTracking;
            this.translationManager = translationManager;
            this.getTimeSummary = getTimeSummary;
            this.messagingCenter = messagingCenter;
            this.getProjects = getProjects;
            this.getUnfinishedFrame = getUnfinishedFrame;
            this.getTickets = getTickets;
            TotalTime = 0;
            messagingCenter.Subscribe<App, TimeUpdatedMessage>(this, "TimeUpdated", (sender, message) =>
            {
                TotalTime = message.TotalTime;
                TicketTime = message.TotalTaskTime;
            });
            IsTracking = false;
            GetCurrentTask();
            GetProjects();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            TotalTime += 1;
        }

        private async void GetProjects()
        {
            try
            {
                IsBusy = true;
                var connection = Connectivity.NetworkAccess == NetworkAccess.Internet;
                getProjects.SwitchConnection(connection);
                getTimeSummary.SetConnection(connection);
                
                var companyId = Preferences.Get("current_company_id", string.Empty);

                var projects = await getProjects.GetAll(int.Parse(companyId));
                if(projects?.Count == 1)
                {
                    ShowTaskList(projects[0].id.ToString());                                       
                }
                var summary = await getTimeSummary.Get(int.Parse(Preferences.Get("current_company_id", "")));

                if (projects == null)
                {
                    ListProject = new List<ProjectWithSummary>();
                    IsBusy = false;
                    return;
                }
                ListProject = projects.Select(p =>
                {
                    if (!summary.Projects.TryGetValue(p.id, out var total))
                    {
                        total = 0;
                    }
                    return new ProjectWithSummary
                    {
                        company_id = p.company_id,
                        id = p.id,
                        name = p.name,
                        summary = total
                    };
                }).ToList();
                IsBusy = false;
            }
            catch (Exception err)
            {
                await navigationService.NavigateAsync("/NavigationPage/CompanyList");
                toast.ShowWarn(translationManager.Translate("views.projectslist.select-company-again"));
            }
        }

        private async void GetCurrentTask()
        {
            try
            {
                var connection = Connectivity.NetworkAccess == NetworkAccess.Internet;
                getTickets.SwitchConnection(false);
                getTimeSummary.SetConnection(connection);
                getUnfinishedFrame.SetConnection(connection);
            
                var companyId = int.Parse(Preferences.Get("current_company_id", ""));
                TotalTime = await getTimeSummary.GetTodayTotal(companyId);
                var currTrackTask = await getUnfinishedFrame.Get();
        
                if (currTrackTask != null)
                {
                    IsTracking = true;
                    CurrentTask = await getTickets.GetOne(currTrackTask.taskId);
                    TicketTime = await getTimeSummary.GetTaskTotal(currTrackTask.taskId, true);
                }
                else
                {
                    CurrentTask = null;
                    IsPlayEnabled = false;
                    IsTracking = false;
                }
            }
            catch (Exception e)
            {
                errorHandler.Handle(e);
            }
        }
        
        [RelayCommand]
        async void ShowTaskList(string project_id) { 
            if (project_id == null) return;

            try
            { 
                var parameters = new NavigationParameters
                {
                    { "project_id", project_id }
                };
                await navigationService.NavigateAsync(nameof(TaskList), parameters);
            }
            catch (Exception err) 
            {
                errorHandler.Handle(err);
            }
        }
        
        [RelayCommand]
        async Task<bool> Track(string state)
        {
            if (CurrentTask == null) return false;
            IsButtonChanging = true;
            var companyId = int.Parse(Preferences.Get("current_company_id", ""));
            var projectId = CurrentTask.project_id;
            var ticketId = CurrentTask.id;

            try
            {
                switch (state)
                {
                    case "start":
                        await startTracking.Exec(companyId, projectId, ticketId);
                        IsTracking = true;
                        IsButtonChanging = false;
                        return true;
                    case "stop":
                        await stopTracking.Exec();
                        IsTracking = false;
                        IsButtonChanging = false;
                        return true;
                    default:
                        IsButtonChanging = false;
                        return false;
                }
            }
            catch (Exception e)
            {
                errorHandler.Handle(e);
                return false;
            }
        }

        public async void OnAppearing()
        {
            GetCurrentTask();
            var summary = await getTimeSummary.Get(int.Parse(Preferences.Get("current_company_id", "")));
            
            if (ListProject == null)
            {
                ListProject = new List<ProjectWithSummary>();
                return;
            }
            ListProject = ListProject.Select(p =>
            {
                if (summary == null) return new ProjectWithSummary { company_id = p.company_id, id = p.id, name = p.name, summary = 0};
                if (!summary.Projects.TryGetValue(p.id, out var total))
                {
                    total = 0;
                }
                return new ProjectWithSummary
                {
                    company_id = p.company_id,
                    id = p.id,
                    name = p.name,
                    summary = total
                };
            }).ToList();
        }

        public void OnDisappearing()
        {
         //   timer.Stop();
        }
    }
}
