using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using LabelHtml.Forms.Plugin.Droid;
using Prism;
using Prism.Ioc;
using System;
using System.ComponentModel;
using Android.Content;
using Prism.Navigation;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin.Droid.Services;
using TimeTrackerXamarin.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using TimeTrackerXamarin.i18n;
using ForegroundService = TimeTrackerXamarin.Droid.Services.ForegroundService;


namespace TimeTrackerXamarin.Droid
{
    [Activity(Theme = "@style/MainTheme", LaunchMode= LaunchMode.SingleInstance,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                               ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private Intent intent;
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            HtmlLabelRenderer.Initialize();
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            UserDialogs.Init(this);
            ProjectsList.EmulateBackPressed = OnBackPressed;
            LoadApplication(new App(new AndroidInitializer()));
            intent = new Intent(this, typeof(ForegroundService));
            SetServiceMethods();
        }

        protected override async void OnResume()
        {
            base.OnResume();
        }
        protected override void OnDestroy()
        {
            ContainerLocator.Container.Resolve<IMessagingCenter>().Send<App, BlockingState>((App)App.Current, "BlockingState", BlockingState.Foreground);
            ContainerLocator.Container.Resolve<IMessagingCenter>().Send<App, int>((App)App.Current, "isBlocked", 1);
            
            base.OnDestroy();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            Android.Content.PM.Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        void SetServiceMethods()
        {             
            ContainerLocator.Container.Resolve<IMessagingCenter>().Subscribe<App>(this, "ServiceStarted", (sender) =>
            {
                ContainerLocator.Container.Resolve<IForegroundServiceController>().StartService();               
            });
            ContainerLocator.Container.Resolve<IMessagingCenter>().Subscribe<App>(this, "ServiceStopped" , (sender) =>
            {
                ContainerLocator.Container.Resolve<IForegroundServiceController>().StopService();                
            });
        }

        private bool IsServiceRunning(Type cls)
        {
            ActivityManager manager = (ActivityManager)GetSystemService(Context.ActivityService);
            //find better solution for GetRunningServices, that is deprecated
            foreach (var service in manager.GetRunningServices(int.MaxValue))
            {
                if (service.Service.ClassName.Equals(Java.Lang.Class.FromType(cls).CanonicalName))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register any platform specific implementations
            // containerRegistry.Register<IForegroundService, Services.ForegroundService>();
            containerRegistry.Register<ISettingsHelper, SettingsHelper>();
            containerRegistry.RegisterSingleton<IForegroundServiceController, ForegroundServiceController>();
        }
    }
}