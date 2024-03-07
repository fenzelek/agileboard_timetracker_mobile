using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AppCenter.Crashes;
using TimeTrackerXamarin._Domains.API;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin._UseCases.Projects;
using TimeTrackerXamarin._UseCases.TimeTracking;
using TimeTrackerXamarin.i18n;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TimeTrackerXamarin.ViewModels
{
    public partial class TaskListViewModel : ObservableObject, INavigationAware
    {
        #region UIControls

        [ObservableProperty]
        private string footerText;

        [ObservableProperty]
        private bool isButtonChanging;

        [ObservableProperty]
        private bool detailsVisible;

        [ObservableProperty]
        private TicketDetails currentTicketDetails;

        private List<BindableTicket> basicTickets = new List<BindableTicket>();
        
        [ObservableProperty]
        private List<BindableTicket> listTask;

        [ObservableProperty]
        private bool isBusy = false;

        [ObservableProperty]
        private string currentProjectID;

        [ObservableProperty]
        private long ticketTime;

        [ObservableProperty]
        private long totalTime;

        [ObservableProperty]
        private bool isTracking;

        [ObservableProperty]
        private Ticket currentTaskTracking;

        [ObservableProperty]
        private Ticket currentTask;
        
        [ObservableProperty]
        private List<User> availableUsers;

        [ObservableProperty]
        private string searchText;

        [ObservableProperty]
        private bool isTaskListEmpty = false;
        
        [ObservableProperty]
        private string sortIcon;
        
        private Sort selectedSort;
        
        [ObservableProperty]
        private User selectedUser;
        
        private bool isListLoaded;

        #endregion

        #region Dependencies

        private INavigationService navigationService;
        private readonly ITranslationManager translation;
        private readonly IToastNotification toast;
        private readonly GetTickets getTickets;
        private readonly GetUsers getUsers;
        private readonly StartTracking startTracking;
        private readonly StopTracking stopTracking;
        private readonly IErrorHandler errorHandler;
        private readonly GetTimeSummary getTimeSummary;
        private readonly GetUnfinishedFrame getUnfinishedFrame;

        #endregion

        public TaskListViewModel(
            INavigationService navigationService,
            ITranslationManager translation,
            IToastNotification toast,
            GetTickets getTickets,
            GetUsers getUsers,
            StartTracking startTracking,
            StopTracking stopTracking,
            IErrorHandler errorHandler,
            IMessagingCenter messagingCenter, GetTimeSummary getTimeSummary, GetUnfinishedFrame getUnfinishedFrame)
        {
            this.navigationService = navigationService;
            this.translation = translation;
            this.toast = toast;
            this.getTickets = getTickets;
            this.getUsers = getUsers;
            DetailsVisible = false;
            messagingCenter.Subscribe<App, TimeUpdatedMessage>(this, "TimeUpdated", (sender, message) =>
            {
                TotalTime = message.TotalTime;
                TicketTime = message.TotalTaskTime;
            });
            messagingCenter.Subscribe<App>(this, "isListLoaded", (sender) => isListLoaded = !isListLoaded);
            this.startTracking = startTracking;
            this.stopTracking = stopTracking;
            this.errorHandler = errorHandler;
            this.getTimeSummary = getTimeSummary;
            this.getUnfinishedFrame = getUnfinishedFrame;
            this.selectedSort = Sort.Ascending;
            SortIcon = "\uf15d";
            isListLoaded = false;
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            return;
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (isListLoaded) return;
            if (parameters.TryGetValue("project_id", out currentProjectID))
            {
                CurrentProjectID = parameters.GetValue<string>("project_id");
                getList();
            }
            else
            {
                navigationService.NavigateAsync("/NavigationPage/ProjectsList");
                toast.ShowWarn(translation.Translate("views.tasklist.alert.unexpected"));
            }
        }

        
        private void addUserFilter(int? id)
        {
            if (id < 0) id = null;
            ListTask = basicTickets.FindAll(t => t.assigned_id == id);
        }

        private void addSprintFilter(int? id)
        {
            ListTask = basicTickets.FindAll(t => t.sprint_id == id);
        }

        [RelayCommand]
        void CloseDetails()
        {
            DetailsVisible = false;
        }

        [RelayCommand]
        private void SortToggle()
        {
            if (ListTask == null) return;
            
            IsBusy = true;
            var tempList = ListTask;
            ListTask = null;
            if (selectedSort == Sort.Descending)
            {
                selectedSort = Sort.Ascending;
                SortIcon = "\uf881";
                ListTask = tempList.OrderBy(t => t.id).ToList();
            }
            else
            {
                selectedSort = Sort.Descending;
                SortIcon = "\uf15d";
                ListTask = tempList.OrderByDescending(t => t.id).ToList();
            }
            
            IsBusy = false;
        }
        
        partial void OnSearchTextChanged(string value)
        {
            IsBusy = true;
            ListTask = basicTickets.FindAll(t =>
                t.title.ToLower().Contains(value.ToLower()) || t.name.ToLower().Contains(value.ToLower()));
            IsBusy = false;
        }
        
        partial void OnSelectedUserChanged(User value)
        {
            if (value == null) ListTask = basicTickets;
            else addUserFilter(value.id);
        }
        
        [RelayCommand]
        void IOSSelected(int taskId)
        {
            var bindable = ListTask.FirstOrDefault((x) => x.id == taskId);
            if (bindable != null)
            {
                ListTask.ForEach((x) => x.Selected = false);
                bindable.Selected = true;
            }
            
            CurrentTask = bindable;
        }
        
        [RelayCommand]
        public async Task QuickTrack(int taskId)
        {
            var bindable = ListTask.FirstOrDefault((x) => x.id == taskId);
            ListTask.ForEach((x) => x.Selected = false);
            CurrentTask = bindable;
            await Track("start");
        }
        
        public async void getList()
        {
            IsBusy = true;

            try
            {
                var companyId = Preferences.Get("current_company_id", "");

                var connection = Connectivity.NetworkAccess == NetworkAccess.Internet;
                getTickets.SwitchConnection(connection);
                getTimeSummary.SetConnection(connection);
                getUnfinishedFrame.SetConnection(connection);
                basicTickets = (await getTickets.GetAll(int.Parse(CurrentProjectID), int.Parse(companyId))).Select(
                    (t) =>
                        new BindableTicket
                        {
                            assigned_id = t.assigned_id,
                            id = t.id,
                            name = t.name,
                            project_id = t.project_id,
                            sprint_id = t.sprint_id,
                            title = t.title,
                            CurrentlyTracking = false
                        }).ToList();
                var currTicket = basicTickets.FirstOrDefault((x) => x.CurrentlyTracking == true);
                if (currTicket != null)
                {
                    currTicket.CurrentlyTracking = false;
                }

                if (basicTickets.Count > 0)
                    FooterText = translation.Translate("views.tasklist.listfull");
                else
                    FooterText = translation.Translate("views.tasklist.listempty");

                TotalTime = await getTimeSummary.GetTodayTotal(int.Parse(Preferences.Get("current_company_id", "")));

                var currTrackTask = await getUnfinishedFrame.Get();
                if (currTrackTask != null)
                {
                    var y = basicTickets.FirstOrDefault((x) => x.id == currTrackTask.taskId);
                    if (y != null)
                    {
                        y.CurrentlyTracking = true;
                    }

                    CurrentTask = CurrentTaskTracking = await getTickets.GetOne(currTrackTask.taskId);
                    TicketTime = await getTimeSummary.GetTaskTotal(currTrackTask.taskId, true);
                    IsTracking = true;
                }

                AvailableUsers = new List<User>();
                ListTask = basicTickets;

                selectedSort = Sort.Ascending;
                SortToggle();

                connection = Connectivity.NetworkAccess == NetworkAccess.Internet;
                getUsers.SwitchConnection(connection);

                List<User> templist = await getUsers.GetProjectUsers(
                    int.Parse(Preferences.Get("current_company_id", "")),
                    int.Parse(CurrentProjectID));
                if (templist == null)
                    templist = new List<User>();
                templist.Add(new User()
                {
                    id = -1, email = "", avatar = "", role = "",
                    first_name = translation.Translate("views.tasklist.unassigneduser"), last_name = ""
                });
                AvailableUsers = templist;
                isListLoaded = true;
            }
            catch (Exception e)
            {
                errorHandler.Handle(e);
            }

            IsTaskListEmpty = ListTask.Count == 0;
            
            IsBusy = false;
        }

        [RelayCommand]
        async void GetTicketDetails(string ticketId)
        {
            if (string.IsNullOrEmpty(ticketId)) return;
           
            IsBusy = true;
            
            var connection = Connectivity.NetworkAccess == NetworkAccess.Internet;
            getTickets.SwitchConnection(connection);

            try
            {
                CurrentTicketDetails = await getTickets.GetDetails(int.Parse(CurrentProjectID), int.Parse(ticketId),
                    int.Parse(Preferences.Get("current_company_id", "")));
            }
            catch (Exception e)
            {
                errorHandler.Handle(e);
                IsBusy = false;
                return;
            }

            DetailsVisible = true;
            IsBusy = false;
        }

        [RelayCommand]
        async void GetCurrentDetails(string ticketId)
        {
            if (string.IsNullOrEmpty(ticketId)) return;

            IsBusy = true;            
            getTickets.SwitchConnection(false);

            try
            {
                CurrentTicketDetails = await getTickets.GetDetails(0, int.Parse(ticketId),0);
            }
            catch (Exception e)
            {
                errorHandler.Handle(e);
                IsBusy = false;
                return;
            }

            DetailsVisible = true;
            IsBusy = false;
        }

        [RelayCommand]
        public async Task<bool> Track(string state)
        {
            if (IsButtonChanging || IsBusy) return false;
            if (CurrentTask == null)
            {
                toast.ShowWarn(translation.Translate("views.tasklist.alert.unselectedtask"));
                return false;
            }

            IsButtonChanging = true;


            var companyId = int.Parse(Preferences.Get("current_company_id", ""));
            var projectId = CurrentTask.project_id;
            var ticketId = CurrentTask.id;
            var connection = Connectivity.NetworkAccess == NetworkAccess.Internet;
            getTimeSummary.SetConnection(connection);

            try
            {
                switch (state)
                {
                    case "start":
                    {
                        var prevTicket = basicTickets.FirstOrDefault((x) => x.CurrentlyTracking);
                        if (prevTicket != null)
                        {
                            prevTicket.CurrentlyTracking = false;
                        }

                        var currTicket = basicTickets.FirstOrDefault((x) => x.id == CurrentTask.id);
                        if (currTicket != null)
                        {
                            currTicket.CurrentlyTracking = true;
                        }

                        if (IsTracking)
                            await stopTracking.Exec();
                        await startTracking.Exec(companyId, projectId, ticketId);
                        IsTracking = true;
                        CurrentTaskTracking = CurrentTask;
                        TicketTime = await getTimeSummary.GetTaskTotal(CurrentTaskTracking.id, true);
                        IsButtonChanging = false;
                        SaveTrackingTask(CurrentTaskTracking);
                        return true;
                    }
                    case "stop":
                    {
                        var currTicket = basicTickets.FirstOrDefault((x) => x.CurrentlyTracking == true);
                        if (currTicket != null)
                        {
                            currTicket.CurrentlyTracking = false;
                        }

                        await stopTracking.Exec();
                        IsTracking = false;
                        CurrentTask = null;
                        CurrentTaskTracking = null;
                        IsButtonChanging = false;
                        return true;
                    }
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

        private async void SaveTrackingTask(Ticket ticket)
        {
            var connection = Connectivity.NetworkAccess == NetworkAccess.Internet;
            if (!connection) return;
            getTickets.SwitchConnection(connection);
            CurrentTicketDetails = await getTickets
                .GetDetails(ticket.project_id, ticket.id, int.Parse(Preferences.Get("current_company_id", "")))
                .ConfigureAwait(false);
        }
    }
}