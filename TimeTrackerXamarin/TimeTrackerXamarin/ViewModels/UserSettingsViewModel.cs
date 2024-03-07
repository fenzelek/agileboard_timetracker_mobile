using Newtonsoft.Json;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Acr.UserDialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AppCenter.Crashes;
using TimeTrackerXamarin._Domains.API;
using TimeTrackerXamarin._UseCases;
using TimeTrackerXamarin._UseCases.Auth;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin.i18n;
using Xamarin.Essentials;
using Xamarin.Forms;
using User = TimeTrackerXamarin._UseCases.Contracts.User;
using TimeTrackerXamarin.iOS.Services;

namespace TimeTrackerXamarin.ViewModels
{
    public partial class UserSettingsViewModel : ObservableObject
    {

        #region Properties

        [ObservableProperty]
        private User currentUser;

        [ObservableProperty]
        private string avatarUrl;

        [ObservableProperty]
        private List<Language> languages;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private Language currentLanguage;
        
        [ObservableProperty]
        private string currentCompany;

        [ObservableProperty]
        private bool isMailDotted = true;

        public string Version { get; } = $"Ver: {VersionTracking.CurrentVersion} ({VersionTracking.CurrentBuild})";

        #endregion

        #region Dependencies

        private readonly Logout logout;
        private readonly IErrorHandler errorHandler;
        private readonly LoggedUser loggedUser;
        private readonly INavigationService navigationService;
        private readonly FlushDatabase flushDatabase;
        private readonly ITranslationManager translation;
        private readonly Avatar avatar;
        private readonly IToastNotification toast;
        private readonly ITimeTracking timeTracking;
        private readonly ILogger logger;

        #endregion

        public UserSettingsViewModel(Logout logout, IErrorHandler errorHandler, LoggedUser loggedUser,
            INavigationService navigationService, FlushDatabase flushDatabase, ITranslationManager translation,
            Avatar avatar, IToastNotification toast, ITimeTracking timeTracking, ILogger logger)
        {
            this.logout = logout;
            this.errorHandler = errorHandler;
            this.loggedUser = loggedUser;
            this.navigationService = navigationService;
            this.flushDatabase = flushDatabase;
            this.translation = translation;
            this.avatar = avatar;
            this.toast = toast;
            this.timeTracking = timeTracking;
            this.logger = logger;
            Languages = Language.AllLanguages.ToList();
            CurrentLanguage = translation.Language;
            CurrentCompany = Preferences.Get("current_company_name", translation.Translate("views.usersettings.none"));
            getData();
        }

        [RelayCommand]
        void ChangeCompany()
        {
            var confirm = new ConfirmConfig
            {
                CancelText = translation.Translate("views.usersettings.company-change.cancel"),
                OkText = translation.Translate("views.usersettings.company-change.confirm"),
                Message = translation.Translate("views.usersettings.company-change.message"),
                Title = translation.Translate("views.usersettings.company-change.title"),
                OnAction = (b) =>
                {
                    if (!b) return;

                    timeTracking.Stop();
                    flushDatabase.Flush();
                    Preferences.Remove("current_company_name");
                    Preferences.Remove("current_company_id");
                    navigationService.NavigateAsync("/NavigationPage/CompanyList");
                }
            };
            UserDialogs.Instance.Confirm(confirm);
        }

        [RelayCommand]
        void MailDotted()
        {
            IsMailDotted = !IsMailDotted;
        }
        
        private async void getData()
        {
            try
            {
                if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    if (!Application.Current.Properties.ContainsKey("saved_user"))
                    {
                        CurrentUser = new User();
                        AvatarUrl = "default_avatar.jpg";
                        return;
                    }

                    CurrentUser =
                        JsonConvert.DeserializeObject<User>(Application.Current.Properties["saved_user"].ToString());
                    if (string.IsNullOrEmpty(CurrentUser.avatar))
                    {
                        AvatarUrl = "default_avatar.jpg";
                        return;
                    }

                    try
                    {
                        AvatarUrl = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                            CurrentUser.avatar);
                    }
                    catch (Exception e)
                    {
                        AvatarUrl = "default_avatar.jpg";
                    }

                    return;
                }

                CurrentUser = await loggedUser.Get();
                Application.Current.Properties.Remove("saved_user");
                Application.Current.Properties.Add("saved_user", JsonConvert.SerializeObject(CurrentUser));

                if (string.IsNullOrEmpty(CurrentUser.avatar))
                {
                    AvatarUrl = "default_avatar.jpg";
                    return;
                }

                try
                {
                    AvatarUrl = await avatar.Save(CurrentUser.avatar);
                }
                catch (ApiErrorException e)
                {
                    AvatarUrl = "default_avatar.jpg";
                }
            }
            catch (Exception e)
            {
                errorHandler.Handle(e);
            }
        }

        [RelayCommand]
        private async void ClearCache()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet) return;
            var confirm = new ConfirmConfig
            {
                CancelText = translation.Translate("views.usersettings.wipe.cancel"),
                OkText = translation.Translate("views.usersettings.wipe.confirm"),
                Message = translation.Translate("views.usersettings.wipe.message"),
                Title = translation.Translate("views.usersettings.wipe.title")
            };
            var confirmed = await UserDialogs.Instance.ConfirmAsync(confirm);
            if (!confirmed)
            {
                return;
            }

            try
            {
                await flushDatabase.Flush();
                toast.ShowInfo(translation.Translate("views.usersettings.alert.okclear"));
            }
            catch (Exception e)
            {
                logger.Error("There was an error.", e);
                toast.ShowError(translation.Translate("views.usersettings.alert.errclear"));
            }
        }

        [RelayCommand]
        private async void LogOut()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet) return;
            IsBusy = true;
            try
            {
                await logout.Exec();
                await navigationService.NavigateAsync("/NavigationPage/LoginPage");
            }
            catch (Exception ex)
            {
                errorHandler.Handle(ex);
            }

            IsBusy = false;
        }

        partial void OnCurrentLanguageChanged(Language language)
        {
            if (language == null || language == translation.Language) return;
            Preferences.Set("language", language.Culture.Name);
            translation.SetLanguage(language);
            TextBindings.Instance.Invalidate();
            toast.ShowInfo(translation.Translate("views.usersettings.alert.languagechanged"));
        }
    }
}