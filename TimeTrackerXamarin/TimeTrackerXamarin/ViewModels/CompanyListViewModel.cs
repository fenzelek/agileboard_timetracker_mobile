using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TimeTrackerXamarin._UseCases.Companies;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Companies;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin._UseCases.Projects;
using TimeTrackerXamarin._UseCases.TimeTracking;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TimeTrackerXamarin.ViewModels
{
    public partial class CompanyListViewModel : ObservableObject
    {
        private readonly INavigationService navigationService;
        private readonly IErrorHandler errorHandler;
        private readonly RemoveFrame removeFrame;
        private readonly GetCompanies getCompanies;
        private readonly GetTickets tickets;
        private readonly GetUnfinishedFrame getUnfinishedFrame;
        private readonly GetTimeSummary getTimeSummary;
        private readonly ContinueLast continueLast;
        private readonly IMessagingCenter messagingCenter;
        private readonly EndCurrentFrame endCurrentFrame;
        private TimeFrame unfinishedTask;

        [ObservableProperty]
        private long loggedTime;

        [ObservableProperty]
        private long totalTime = 0;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool isVisibleContinue;

        [ObservableProperty]
        private TicketDetails currTicket;
        
        [ObservableProperty]
        private List<Company> companyList;

        public CompanyListViewModel(INavigationService navigationService, IErrorHandler errorHandler,
            RemoveFrame removeFrame, GetCompanies getCompanies, GetTickets tickets,
            GetUnfinishedFrame getUnfinishedFrame, IMessagingCenter messagingCenter, EndCurrentFrame endCurrentFrame,
            GetTimeSummary getTimeSummary, ContinueLast continueLast)
        {
            this.navigationService = navigationService;
            this.errorHandler = errorHandler;
            this.removeFrame = removeFrame;
            this.getCompanies = getCompanies;
            this.tickets = tickets;
            this.getUnfinishedFrame = getUnfinishedFrame;
            this.messagingCenter = messagingCenter;
            this.endCurrentFrame = endCurrentFrame;
            this.getTimeSummary = getTimeSummary;
            this.continueLast = continueLast;
            CompanyList = new List<Company>();
            IsVisibleContinue = false;
            MessagingCenter.Subscribe<App, TimeUpdatedMessage>(this, "TimeUpdated",
                (sender, message) => { LoggedTime = message.TotalTime - TotalTime; });
            getData();
        }


        async void getData()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var connection = Connectivity.NetworkAccess == NetworkAccess.Internet;
                getCompanies.SwitchConnection(connection);
                tickets.SwitchConnection(connection);
                getTimeSummary.SetConnection(connection);
                getUnfinishedFrame.SetConnection(connection);

                CompanyList = await getCompanies.GetAll();
                unfinishedTask = await getUnfinishedFrame.Get();
                if (unfinishedTask == null)
                {
                    IsVisibleContinue = false;
                    IsBusy = false;
                    TryToNavigate();
                    return;
                }

                var companyId = int.Parse(Preferences.Get("current_company_id", ""));
                IsVisibleContinue = true;

                CurrTicket = await tickets.GetDetails(unfinishedTask.projectId, unfinishedTask.taskId, companyId);
                TotalTime = await getTimeSummary.GetTodayTotal(companyId);
                LoggedTime = DateTimeOffset.Now.ToUnixTimeSeconds() - unfinishedTask.from;

//              LoggedTime -= TotalTime;
                //if (Device.RuntimePlatform == Device.iOS)
                    await continueLast.Exec(false);
                IsBusy = false;
            }
            catch (Exception e)
            {
                errorHandler.Handle(e);
                isBusy = false;
            }
        }

        private void TryToNavigate()
        {
            IsBusy = true;
            Device.BeginInvokeOnMainThread(() =>
            {
                if (!Preferences.ContainsKey("current_company_id") ||
                    !Preferences.ContainsKey("current_company_name")) return;
                var id = Preferences.Get("current_company_id", "");
                var name = Preferences.Get("current_company_name", "");
                if (id == "" || name == "") return;
                navigationService.NavigateAsync("/NavigationPage/ProjectsList");
            });
            IsBusy = false;
        }

        [RelayCommand]
        private async void Continue() {
            try
            {
                IsVisibleContinue = false;
                messagingCenter.Send<App, BlockingState>((App)App.Current, "BlockingState", BlockingState.Never);
                //await continueLast.Exec(true);
                TryToNavigate();
            }
            catch (Exception e)
            {
                errorHandler.Handle(e);
            }
        }

        [RelayCommand]
        private async void Stop()
        {
            try
            {

                IsVisibleContinue = false;
                await endCurrentFrame.Exec();
                messagingCenter.Send<App>((App)App.Current, "ServiceStopped");
                TryToNavigate();
                IsBusy = true;
            }
            catch (Exception e)
            {
                errorHandler.Handle(e);
            }
        }

        [RelayCommand]
        private async void Delete()
        {
            try
            {
                messagingCenter.Send<App, BlockingState>((App)App.Current, "BlockingState", BlockingState.Never);
                removeFrame.SetConnection(false);
                await removeFrame.Exec(unfinishedTask.Id);
                IsVisibleContinue = false;
                messagingCenter.Send<App>((App)App.Current, "ServiceStopped");
                TryToNavigate();
            }
            catch (Exception e)
            {
                errorHandler.Handle(e);
            }
        }

        [RelayCommand]
        async void GoToProjectList(string id)
        {
            Company current = CompanyList.First(x => x.id.ToString() == id);
            Preferences.Set("current_company_id", current.id.ToString());
            Preferences.Set("current_company_name", current.name);
            await navigationService.NavigateAsync("/NavigationPage/ProjectsList");
        }
    }
}