using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Systems;
using Prism.Ioc;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin.Config;
using TimeTrackerXamarin.i18n;
using Xamarin.Forms;
using OperationCanceledException = System.OperationCanceledException;

namespace TimeTrackerXamarin.Droid.Services
{
    [Service]
    public class ForegroundService : Service
    {
        private CancellationTokenSource tokenSource;
        private long foregroundTime = -1;
        private bool inactivityNotified = false;
        private bool inactivityNotificationEnabled = false;
        private BlockingState blockingState = BlockingState.Never;
        private bool returnedFromForeground = false;
        private IConfiguration configuration => ContainerLocator.Container.Resolve<IConfiguration>();

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        const int ForegroundNotificationRelatedId = 1;
        readonly string ForegroundNotificationRelatedIdString = ForegroundNotificationRelatedId.ToString();
        const string ForegroundNotificationChannelName = "Foreground";
        
        const int RemindingNotificationRelatedId = 2;
        readonly string RemindingNotificationRelatedIdString = RemindingNotificationRelatedId.ToString();
        const string RemindingNotificationChannelName = "Reminding";

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            blockingState = (intent.GetBooleanExtra("initialBlockState", false)) ?  BlockingState.Always: BlockingState.Never;
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
            tokenSource = new CancellationTokenSource();
            CreateNotificationChannel(NotificationImportance.None, ForegroundNotificationRelatedIdString, ForegroundNotificationChannelName);
            CreateNotificationChannel(NotificationImportance.High, RemindingNotificationRelatedIdString, RemindingNotificationChannelName);
            StartForeground(ForegroundNotificationRelatedId, CreateNotification(true, "TimeTracker", "Fetching task data...",ForegroundNotificationRelatedIdString));

            var timeTracking = ContainerLocator.Container.Resolve<ITimeTracking>();
            Task.Run(async () =>
            {
                var current = await timeTracking.GetCurrentTracking();
                if (current == null)
                {
                    return;
                }

                inactivityNotificationEnabled = current.TotalTime < configuration.InactivityNotificationTime;
            });

            Task.Run(() =>
            {
                try
                {
                    Loop().Wait();
                }
                catch (Android.OS.OperationCanceledException)
                {
                }
                finally
                {
                    //StopForeground(true);
                    if (tokenSource.IsCancellationRequested)
                    {
                        MessagingCenter.Send<App>((App)App.Current, "ServiceStopped" );
                    }
                }
            }, tokenSource.Token);
            return StartCommandResult.Sticky;
        }
        
        async Task Loop()
        {
            var token = tokenSource.Token;
            await Task.Run(async () =>
            {
                inactivityNotified = false;
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

            var trackingTimeTotal = currentTracking.TotalTime + time;
            if (inactivityNotificationEnabled && trackingTimeTotal >= configuration.InactivityNotificationTime && !inactivityNotified)
            {
                inactivityNotified = true;
                SendInactivityNotification();
            }
            
            var timeUpdatedMessage = new TimeUpdatedMessage
            {
                TaskName = currentTracking.TaskTitle,
                TotalTaskTime = currentTracking.TaskTime + time,
                TotalTime = currentTracking.TotalTime + time
            };
            
            UpdateTimeNotification(timeUpdatedMessage.TaskName, timeUpdatedMessage.TotalTime, timeUpdatedMessage.TotalTaskTime);
            Device.BeginInvokeOnMainThread(() =>
            {
                messagingCenter.Send<App, TimeUpdatedMessage>((App)App.Current, "TimeUpdated", timeUpdatedMessage);
            });
            
            if (!blocked && time >= configuration.FrameLength)
            {
                SyncingNotification(timeUpdatedMessage.TaskName);
                await timeTracking.EndFrame(now);                
            }
        }

        void SendInactivityNotification()
        {
            var message = ContainerLocator.Container.Resolve<ITranslationManager>()
                .Translate("reminder-notification");
            var remindNotification = CreateNotification(false, "TimeTracker", message, RemindingNotificationRelatedIdString, true);
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.Notify(RemindingNotificationRelatedId, remindNotification);
        }

        void UpdateTimeNotification(string taskName, long totalTime, long taskTime)
        {
            var title = $"Task: {taskName}";
            var content =
                $"Task: {TimeSpan.FromSeconds(taskTime)} Total: {TimeSpan.FromSeconds(totalTime):hh\\:mm}";
            UpdateNotify(true, title, content, ForegroundNotificationRelatedIdString, ForegroundNotificationRelatedId);
        }

        void SyncingNotification(string taskName)
        {
            var syncMessage = ContainerLocator.Container.Resolve<ITranslationManager>().Translate("sync");
            var title = $"Task: {taskName}";
            UpdateNotify(true, title, syncMessage, ForegroundNotificationRelatedIdString, ForegroundNotificationRelatedId); 
        }
        
        public override void OnDestroy()
        {
            if (tokenSource != null)
            {
                tokenSource.Token.ThrowIfCancellationRequested();
                tokenSource.Cancel();
            }

            base.OnDestroy();
        }

        void CreateNotificationChannel(NotificationImportance importance, string id, string name)
        {
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                if (notificationManager.GetNotificationChannel(id) != null)
                {
                    return;
                }

                var notfificationChannel =
                    new NotificationChannel(id, name, importance);
                notificationManager.CreateNotificationChannel(notfificationChannel);
            }
        }

        Notification CreateNotification(bool onGoing, string title, string content, string channelId, bool autocancel = false)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.SingleTop);

            var flag = PendingIntentFlags.UpdateCurrent;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                flag |= PendingIntentFlags.Immutable;
            }
            
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, flag);
            return new Notification.Builder(this, channelId)
                .SetContentTitle(title)
                .SetContentText(content)
                .SetSmallIcon(Resource.Drawable.timetrackertree)
                .SetOngoing(onGoing)
                .SetContentIntent(pendingIntent)
                .SetAutoCancel(autocancel)
                .Build();
        }

        void UpdateNotify(bool onGoing, string title, string content, string channelId, int notificationId)
        {
            var notification = CreateNotification(onGoing, title, content, channelId);
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.Notify(notificationId, notification);
        }
    }
}