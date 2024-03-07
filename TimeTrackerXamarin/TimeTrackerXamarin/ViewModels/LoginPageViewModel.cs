using Prism.Commands;
using Prism.Navigation;
using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using TimeTrackerXamarin._Domains.Auth.Dto;
using TimeTrackerXamarin._UseCases.Auth;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.TimeTracking;
using TimeTrackerXamarin.Config;
using TimeTrackerXamarin.i18n;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TimeTrackerXamarin.ViewModels
{
    public partial class LoginPageViewModel : ObservableObject
    {
        public string Version { get; } = $"Ver: {VersionTracking.CurrentVersion} ({VersionTracking.CurrentBuild})";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        
        [ObservableProperty]
        private bool isDebug;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool isPasswordHidden = true;

        [ObservableProperty]
        private string passwordGlyph = "\uf06e";

        private readonly INavigationService navigationService;
        private readonly IToastNotification toast;
        private readonly IErrorHandler errorHandler;
        private readonly ITranslationManager translationManager;
        private readonly CheckLogin checkLogin;
        private readonly Login login;
        private readonly LoggedUser loggedUser;
        private readonly GetUnfinishedFrame getUnfinishedFrame;
        private readonly ILogger logger;

        public LoginPageViewModel(INavigationService navigationService, IToastNotification toast, IErrorHandler errorHandler, ITranslationManager translationManager, CheckLogin checkLogin, Login login, LoggedUser loggedUser, IConfiguration configuration, GetUnfinishedFrame getUnfinishedFrame, ILogger logger)
        {           
            this.navigationService = navigationService;
            this.toast = toast;
            this.errorHandler = errorHandler;
            this.translationManager = translationManager;
            this.checkLogin = checkLogin;
            this.login = login;
            this.loggedUser = loggedUser;
            this.getUnfinishedFrame = getUnfinishedFrame;
            this.logger = logger;
            IsDebug = configuration.IsDebug;
            IsUserLogged();
        }

        [RelayCommand]
        void TogglePasswordVisible()
        {
            IsPasswordHidden = !IsPasswordHidden;
            if (IsPasswordHidden)
            {
                PasswordGlyph = "\uf06e";
                return;
            }

            PasswordGlyph = "\uf070";
        }
        
        [RelayCommand]
        async void Login() {
            IsBusy = true;
            logger.Debug("CheckLogin Start");
            Device.BeginInvokeOnMainThread(async () => {
                try
                {
                    var loginData = new LoginDto()
                    {
                        email = Email,
                        password = Password
                    };
                    
                    await login.Exec(loginData);
                    var currUser = await loggedUser.Get();
                    
                    Preferences.Set("current_user_name", $"{currUser.first_name} {currUser.last_name}");
                    Preferences.Set("current_user_id", currUser.id.ToString());
                    Application.Current.Properties.Remove("saved_user");
                    Application.Current.Properties.Add("saved_user", JsonConvert.SerializeObject(currUser));
                    
                    logger.Info($"Logged in as {currUser.first_name} {currUser.last_name}.");
                    await navigationService.NavigateAsync("/NavigationPage/CompanyList");
                    
                    toast.ShowInfo(translationManager.Translate("views.loginpage.welcome.first"));
                }
                catch (Exception e)
                {
                    errorHandler.Handle(e);
                }
            });
            logger.Debug("CheckLogin End");
            IsBusy = false;
        }        
        async void IsUserLogged() {
            IsBusy = true;
            try
            {
                if (await checkLogin.Exec())
                {
                    if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                    {
                        try
                        {
                            var currUser = await loggedUser.Get();
                            Preferences.Set("current_user_name", $"{currUser.first_name} {currUser.last_name}");
                            Preferences.Set("current_user_id", currUser.id.ToString());
                            Application.Current.Properties.Remove("saved_user");
                            Application.Current.Properties.Add("saved_user", JsonConvert.SerializeObject(currUser));
                            logger.Info($"Logged in as {currUser.first_name} {currUser.last_name}.");
                        }
                        catch (Exception err)
                        {
                            errorHandler.Handle(err);
                            await Task.Delay(1000);
                        }
                        finally
                        {
                            string name = Preferences.Get("current_user_name", "").Split(' ')[0];
                            toast.ShowInfo($"{translationManager.Translate("views.loginpage.welcome.online")} {name}!");    
                        }
                    }
                    else
                    {
                        toast.ShowInfo($"{translationManager.Translate("views.loginpage.welcome.offline")}");
                    }

                    var companyId = Preferences.Get("current_company_id", "");
                    
                    var connection = Connectivity.NetworkAccess == NetworkAccess.Internet;
                    getUnfinishedFrame.SetConnection(connection);
                    var frame = await getUnfinishedFrame.Get();
                    if (string.IsNullOrEmpty(companyId) || frame != null)
                    {
                        await navigationService.NavigateAsync("/NavigationPage/CompanyList");
                        return;
                    }

                    await navigationService.NavigateAsync("/NavigationPage/ProjectsList");
                }
            }
            catch (Exception err)
            {
                errorHandler.Handle(err);
            }

            IsBusy = false;
        }                               

        [RelayCommand]
        async void Redirect(string link) {
            try
            {                
                await Browser.OpenAsync(new Uri("https://app.agileboard.me" + link), BrowserLaunchMode.SystemPreferred);
                
            }
            catch (Exception ex)
            {
                var error = translationManager.Translate("views.loginpage.browser-error");
                var cancel = translationManager.Translate("views.loginpage.cancel");
                await App.Current.MainPage.DisplayAlert("Problem", error, cancel);
            }
        }
    }
}
