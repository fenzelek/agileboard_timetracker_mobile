using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin.Config;
using UIKit;
using Prism.Ioc;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.LocalNotification;
using TimeTrackerXamarin.i18n;

namespace TimeTrackerXamarin.iOS.Services
{
    public class ForegroundService: IForegroundIos
    {
        private CancellationTokenSource tokenSource;
        private long foregroundTime = -1;
        private BlockingState blockingState = BlockingState.Never;
        private bool returnedFromForeground = false;
        private IConfiguration configuration => ContainerLocator.Container.Resolve<IConfiguration>();

        public bool IsTracking()
        {
            return tokenSource != null;
        }
        public void StartService()
        {
            if (tokenSource != null) tokenSource.Cancel();
            tokenSource = new CancellationTokenSource();
            ContainerLocator.Container.Resolve<IMessagingCenter>().Subscribe<App>((App)App.Current, "disposeService", (sender) => StopService());
            ContainerLocator.Container.Resolve<IMessagingCenter>().Subscribe<App, BlockingState>(this, "BlockingState", (sender, state) =>
            {
                if ((blockingState == BlockingState.Foreground && state == BlockingState.Always) || (returnedFromForeground && state == BlockingState.Foreground))
                {
                    returnedFromForeground = true;
                }
                else
                {
                    returnedFromForeground = false;
                }
                blockingState = state;
                if (state == BlockingState.Foreground)
                {
                    foregroundTime = DateTimeOffset.Now.ToUnixTimeSeconds();
                }
            });

            ScheduleInactivityNotification();
            
            Task.Run(() =>
            {
                try
                {
                    Loop().Wait();
                }
                catch (Exception ex)
                {
                    // ignored
                }
                finally
                {
                    if (tokenSource.IsCancellationRequested)
                    {
                        MessagingCenter.Send<App>((App)App.Current, "ServiceStopped");
                    }
                }
            }, tokenSource.Token);
        }
        async Task Loop()
        {
            var token = tokenSource.Token;
            await Task.Run(async () =>
            {
                while (true)
                {
                    await Tick();
                    await Task.Delay(1000, token);
                }
            }, token);
        }
        private async Task Tick()
        {
            var timeTracking = ContainerLocator.Container.Resolve<ITimeTracking>();
            var messagingCenter = ContainerLocator.Container.Resolve<IMessagingCenter>();

            var currentTracking = await timeTracking.GetCurrentTracking();
            if (currentTracking == null)
            {
                return;
            }

            var currentFrame = currentTracking.Frame;
            var now = DateTimeOffset.Now.ToUnixTimeSeconds();
            var time = now - currentFrame.from;
            var blocked = blockingState == BlockingState.Always;
            if (blockingState == BlockingState.Foreground)
            {
                var uninterrupted = now - foregroundTime;
                if (uninterrupted == 0)
                {
                    return;
                }
                if (uninterrupted >= configuration.FrameTimeout || returnedFromForeground)
                {
                    blocked = true;
                }
            }

            var timeUpdatedMessage = new TimeUpdatedMessage
            {
                TaskName = currentTracking.TaskTitle,
                TotalTaskTime = currentTracking.TaskTime + time,
                TotalTime = currentTracking.TotalTime + time
            };
            Device.BeginInvokeOnMainThread(() =>
            {
                messagingCenter.Send<App, TimeUpdatedMessage>((App)App.Current, "TimeUpdated", timeUpdatedMessage);
            });

            if (!blocked && time >= configuration.FrameLength)
            {
                //todo
                timeTracking.EndFrame(now);
            }
        }
        async void ScheduleInactivityNotification()
        {
            var timeTracking = ContainerLocator.Container.Resolve<ITimeTracking>();
            var currentTracking = await timeTracking.GetCurrentTracking();
            if (currentTracking == null)
            {
                return;
            }
            var totalTracked = currentTracking.TotalTime;
            var remainingTime = configuration.InactivityNotificationTime - totalTracked;

            if (remainingTime <= 0)
            {
                return;
            }
            
            var now = DateTime.Now;
            var notificationTime = now.AddSeconds(remainingTime);
            var message = ContainerLocator.Container.Resolve<ITranslationManager>()
                .Translate("reminder-notification");

            var notificationCenter = ContainerLocator.Container.Resolve<INotificationManager<AppDelegate>>();
            notificationCenter.CancelScheduledNotifications();
            notificationCenter.SendNotification("TimeTracker", message, notificationTime);
        }

        public void StopService()
        {
            if (tokenSource != null)
            {
                ContainerLocator.Container.Resolve<INotificationManager<AppDelegate>>().CancelScheduledNotifications();
                tokenSource.Token.ThrowIfCancellationRequested();
                tokenSource.Cancel();
                tokenSource = null;
            }
        }        
    }
}