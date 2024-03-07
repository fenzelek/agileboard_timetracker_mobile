using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Prism;
using Prism.Ioc;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Flurl.Http;
using Flurl.Http.Configuration;
using TimeTrackerXamarin._Domains.API;
using TimeTrackerXamarin._Domains.Auth;
using TimeTrackerXamarin._Domains.Companies;
using TimeTrackerXamarin._Domains.Projects;
using TimeTrackerXamarin._Domains.Projects.Tickets;
using TimeTrackerXamarin._Domains.Projects.Users;
using TimeTrackerXamarin._Domains.TimeTracking;
using TimeTrackerXamarin._UseCases.Auth;
using TimeTrackerXamarin._UseCases.Companies;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Auth;
using TimeTrackerXamarin._UseCases.Contracts.Companies;
using TimeTrackerXamarin._UseCases.Contracts.Projects;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin._UseCases.Projects;
using TimeTrackerXamarin._UseCases.TimeTracking;
using TimeTrackerXamarin.Config;
using TimeTrackerXamarin.i18n;
using TimeTrackerXamarin.Services;
using TimeTrackerXamarin.Themes;
using TimeTrackerXamarin.ViewModels;
using TimeTrackerXamarin.Views;
using Xamarin.Essentials;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;
using Sprint = TimeTrackerXamarin._UseCases.Contracts.Sprint;
using Ticket = TimeTrackerXamarin._UseCases.Contracts.Ticket;
using User = TimeTrackerXamarin._UseCases.Contracts.User;
using TimeTrackerXamarin._Domains.Projects.Sprints;
using TimeTrackerXamarin._Domains.Projects.Tickets.Mapper;
using TimeTrackerXamarin._Domains.TimeTracking.Frame;
using TimeTrackerXamarin._Domains.TimeTracking.Summary;
using TimeTrackerXamarin._UseCases;
using TimeTrackerXamarin._UseCases.Contracts.Projects.Tickets;
using TimeTrackerXamarin._UseCases.Contracts.Projects.Users;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking.Frame;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking.Summary;
using Prism.Navigation;
using System.Threading.Tasks;

[assembly: ExportFont("fontawesome.otf", Alias = "FA")]
[assembly: ExportFont("fontawesomesolid.otf", Alias = "FAsolid")]

namespace TimeTrackerXamarin
{
    public partial class App
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }       

        protected override async void OnInitialized()
        {
            InitializeComponent();
            if(Device.RuntimePlatform == Device.iOS)
                Application.Current.MainPage = new LoginPage();
            else
            {
                await   NavigationService.NavigateAsync("/LoginPage");
            }
            Container.Resolve<IThemeManager>().UpdateAppTheme(Current.RequestedTheme);
            if (await Permissions.CheckStatusAsync<Permissions.LocationAlways>() != PermissionStatus.Granted)                
                await NavigationService.NavigateAsync("/PermissionView");            
            Connectivity.ConnectivityChanged += ResumeOnline;
            Current.RequestedThemeChanged += SwapTheme;
            ContainerLocator.Container.Resolve<IMessagingCenter>().Send<App, BlockingState>((App)App.Current, "BlockingState", BlockingState.Always);
        }

        private void CheckLanguage()
        {
            var languages = Language.AllLanguages;
            var iso = CultureInfo.CurrentUICulture.Name;
            string culture;
            if (Preferences.ContainsKey("language"))
            {
                culture = Preferences.Get("language", "en-US");
            }
            else
            {
                culture = iso;
            }

            var language = languages.FirstOrDefault((x) => x.Culture.Name == culture);
            if (language != null)
            {
                Container.Resolve<ITranslationManager>().SetLanguage(language);
            }
            else
            {
                Container.Resolve<ITranslationManager>().SetLanguage(Language.English);
            }
            Preferences.Set("remindMessage", Container.Resolve<ITranslationManager>().Translate("reminder-notifcation"));
        }

        private async void ResumeOnline(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == NetworkAccess.Internet)
                await Container.Resolve<ITimeTracking>().Sync();
        }

        private void SwapTheme(object sender, AppThemeChangedEventArgs e)
        {
            Container.Resolve<IThemeManager>().UpdateAppTheme(e.RequestedTheme);
        }

        protected override async void OnResume()
        {
            base.OnResume();            
            if(Device.RuntimePlatform == Device.iOS)
            {
                MessagingCenter.Send(App.Current, "disposeService");
                await Task.Delay(500);
                MessagingCenter.Send(App.Current, "ServiceStarted");
            }

            if (await Permissions.CheckStatusAsync<Permissions.LocationAlways>() != PermissionStatus.Granted)
                await ContainerLocator.Container.Resolve<INavigationService>().NavigateAsync("/PermissionView");
            
            await Container.Resolve<ITimeTracking>().Sync().ConfigureAwait(false);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //Config
            containerRegistry.RegisterSingleton<IConfiguration, PropertiesConfiguration>();
            Container.Resolve<IConfiguration>().Load();

            var debug = Container.Resolve<IConfiguration>().IsDebug;
            Microsoft.AppCenter.AppCenter.SetEnabledAsync(!debug);
            Container.Resolve<ILogger>().SetDebug(debug);
            
            //Services
            containerRegistry.Register<Application>(() => Current);
            containerRegistry.Register<IDatabaseFlushService, DatabaseFlushService>();
            containerRegistry.Register<FlushDatabase>();
            containerRegistry.Register<IErrorHandler, ErrorHandler>();
            containerRegistry.Register<IThemeManager, ThemeManager>();
            containerRegistry.RegisterSingleton<IDatabaseConnector, DatabaseConnector>();

            //Analytics
            containerRegistry.RegisterSingleton<IAppCenterAnalytics, AppCenterAnalytics>();
            Microsoft.AppCenter.AppCenter.Start("android=7b359046-e756-4ae5-b1d2-ed58fc69c508;" +
                  "ios=7cc54f3e-ee72-40e9-aaf7-aa1f233d5da5;", 
                typeof(Analytics), typeof(Crashes));
            
            //Navigation
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();
            containerRegistry.RegisterForNavigation<ThemeAwareNavigationPage>("NavigationPage");
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<TaskList, TaskListViewModel>();
            containerRegistry.RegisterForNavigation<ProjectsList, ProjectsListViewModel>();
            containerRegistry.RegisterForNavigation<UserSettings, UserSettingsViewModel>();
            containerRegistry.RegisterForNavigation<CompanyList, CompanyListViewModel>();
            containerRegistry.RegisterForNavigation<TimeSummaryPage, TimeSummaryViewModel>();
            containerRegistry.RegisterForNavigation<TaskCreate, TaskCreateViewModel>();
            containerRegistry.RegisterForNavigation<PermissionView, PermissionViewModel>();
            containerRegistry.RegisterForNavigation<DebugPage, DebugPanelViewModel>();
            containerRegistry.RegisterForNavigation<LogsPage, LogsPageViewModel>();
            
            //Requests
            containerRegistry.RegisterSingleton<IFlurlClientFactory, FlurlClientFactory>();
            containerRegistry.RegisterSingleton<IFlurlClient>(() =>
            {
                //if we use more than one api we cannot do it like that, for now its fine
                var url = Container.Resolve<IConfiguration>().ApiUrl;
                return Container.Resolve<IFlurlClientFactory>().Get(url);
            });

            //Notifications
            containerRegistry.Register<IPushNotification, PushNotification>();

            //Localization
            containerRegistry.Register<ILocalization, Localization>();
            
            //Mapper
            containerRegistry.Register<IMapper<List<Ticket>, List<Sprint>>, TicketListMapper>();
            containerRegistry.Register<IMapper<List<User>, List<ProjectUser>>, ProjectUserMapper>();
            containerRegistry.Register<IMapper<TicketDetails, TicketDetailsDto>, TicketDetailsMapper>();
            containerRegistry.Register<IMapper<TicketDetails, Ticket>, TicketDetailsFromTicketMapper>();

            //Auth
            containerRegistry.Register<IAuthService, AuthService>();
            containerRegistry.Register<ITokenService, TokenService>();
            containerRegistry.Register<Login>();
            containerRegistry.Register<Logout>();
            containerRegistry.Register<LoggedUser>();
            containerRegistry.Register<CheckLogin>();

            //TimeTracking
            containerRegistry.RegisterSingleton<ITimeTracking, TimeTrackingService>();
            containerRegistry.Register<StartTracking>();
            containerRegistry.Register<StopTracking>();
            containerRegistry.Register<CurrentTracking>();
            containerRegistry.Register<ContinueLast>();
            
            //Time summary
            containerRegistry.Register<IRemoteTimeSummarySource, RemoteTimeSummarySource>();
            containerRegistry.Register<ILocalTimeSummarySource, LocalTimeSummarySource>();
            containerRegistry.Register<IFactory<ITimeSummaryService>, TimeSummaryServiceFactory>();
            containerRegistry.Register<GetTimeSummary>();
            
            //Frames
            containerRegistry.Register<IRemoteFrameSource, RemoteFrameSource>();
            containerRegistry.Register<ILocalFrameSource, LocalFrameSource>();
            containerRegistry.Register<RemoveFrame>();
            containerRegistry.Register<GetUnfinishedFrame>();
            containerRegistry.Register<IFactory<IFrameService>, FrameServiceFactory>();

            //Companies
            containerRegistry.Register<IFactory<ICompanyService>, CompanyServiceFactory>();
            containerRegistry.Register<GetCompanies>();
            containerRegistry.Register<ILocalCompanySource, LocalCompanySource>();
            containerRegistry.Register<IRemoteCompanySource, RemoteCompanySource>();
            
            //Projects
            containerRegistry.Register<IRemoteProjectSource, RemoteProjectSource>();
            containerRegistry.Register<ILocalProjectSource, LocalProjectSource>();
            containerRegistry.Register<IFactory<IProjectService>, ProjectServiceFactory>();
            containerRegistry.Register<GetProjects>();
            
            //Users
            containerRegistry.Register<IRemoteUserSource, RemoteUserSource>();
            containerRegistry.Register<ILocalUserSource, LocalUserDataSource>();
            containerRegistry.Register<IFactory<IUserService>, UserServiceFactory>();
            containerRegistry.Register<GetUsers>();
            
            //Sprints
            containerRegistry.Register<ISprintService, SprintService>();
            containerRegistry.Register<GetSprints>();
            
            //Tickets
            containerRegistry.Register<IRemoteTicketSource, RemoteTicketSource>();
            containerRegistry.Register<ILocalTicketSource, LocalTicketSource>();
            containerRegistry.Register<IFactory<ITicketService>, TicketServiceFactory>();
            containerRegistry.Register<GetTickets>();
            containerRegistry.Register<CreateTicket>();

            //Xamarin Utils
            containerRegistry.Register<IMessagingCenter>(() => MessagingCenter.Instance);
            containerRegistry.Register<IGeolocation, GeolocationImplementation>();
            containerRegistry.Register<IConnectivity, ConnectivityImplementation>();
            containerRegistry.Register<IPreferences, PreferencesImplementation>();
            containerRegistry.Register<IPermissions, PermissionsImplementation>();
            
            //Debug
            containerRegistry.Register<IDebugTools, DebugTools>();
        }

        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry) 
        {
            base.RegisterRequiredTypes(containerRegistry);

            containerRegistry.RegisterSingleton<ITranslationManager, TranslationManager>();
            CheckLanguage();
            containerRegistry.RegisterSingleton<IToastNotification, ToastNotification>();
            containerRegistry.RegisterSingleton<ILogStorage, FileLogStorage>();
            containerRegistry.RegisterSingleton<ILogger, Logger>();
                
            containerRegistry.RegisterScoped<INavigationService, AwarePageNavigationService>();
            containerRegistry.Register<INavigationService, AwarePageNavigationService>(NavigationServiceName);
        }
    }
}