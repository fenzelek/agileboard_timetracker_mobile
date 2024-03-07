using System;
using System.Threading;
using System.Threading.Tasks;
using Foundation;
using Prism.Ioc;
using Prism;
using SQLitePCL;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin._UseCases.TimeTracking;
using TimeTrackerXamarin.iOS.Services;
using UIKit;
using UserNotifications;
using Xamarin.Essentials;
using Xamarin.Forms;
using CoreLocation;
using Plugin.LocalNotification;
using TimeTrackerXamarin.i18n;

namespace TimeTrackerXamarin.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IUNUserNotificationCenterDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App(new iOSInitializer()));
            SetServiceMethods();
            var manager = ContainerLocator.Container.Resolve<INotificationManager<AppDelegate>>();
            if (manager is iOSNotificationManager iosManager)
            {
                iosManager.Initialize(this);
            }

            UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;
            return base.FinishedLaunching(app, options);
        }

        public override void DidEnterBackground(UIApplication uiApplication)
        {
            base.DidEnterBackground(uiApplication);
            ContainerLocator.Container.Resolve<IMessagingCenter>().Send<App, BlockingState>((App)App.Current, "BlockingState", BlockingState.Foreground);
        }
        private void SetServiceMethods()
        {
            MessagingCenter.Subscribe<App>((App)App.Current, "ServiceStarted",  async (sender) =>
            {
                ContainerLocator.Container.Resolve<IForegroundIos>().StartService();
            });
            MessagingCenter.Subscribe<App>((App)App.Current, "ServiceStopped", (sender) =>
            {
                ContainerLocator.Container.Resolve<IForegroundIos>().StopService();
                MessagingCenter.Send<App>((App)App.Current, "disposeService");
            });
        }
        public override void WillTerminate(UIApplication uiApplication)
        {
            base.WillTerminate(uiApplication);

            var getUnfinishedFrame = ContainerLocator.Container.Resolve<GetUnfinishedFrame>();
            getUnfinishedFrame.SetConnection(false);

            var unfinishedFrame = Task.Run(async () => await getUnfinishedFrame.Get()).Result;
            if (unfinishedFrame == null)
            {
                return;
            }
            var message = ContainerLocator.Container.Resolve<ITranslationManager>().Translate("ios-app-terminated");
            ContainerLocator.Container.Resolve<INotificationManager<AppDelegate>>().SendNotification("TimeTracker", message);
        }
        
        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, System.Action<UNNotificationPresentationOptions> completionHandler)
        {
            completionHandler(UNNotificationPresentationOptions.Alert);
        }
    }

    public class iOSInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IForegroundIos, ForegroundService>();
            containerRegistry.Register<ISettingsHelper, SettingsHelper>();
            containerRegistry.RegisterSingleton<INotificationManager<AppDelegate>, iOSNotificationManager>();
            containerRegistry.RegisterSingleton<IForegroundServiceController, ForegroundServiceController>();
        }
    }
}