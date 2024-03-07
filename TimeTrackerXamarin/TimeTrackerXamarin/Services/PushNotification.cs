using System;
using System.Threading.Tasks;
using Plugin.LocalNotification;
using TimeTrackerXamarin._UseCases.Contracts;

namespace TimeTrackerXamarin.Services
{
    public class PushNotification : IPushNotification
    {
        private readonly string title = "TimeTracker";
        public Task<bool> Create(int id, string subtitle, string content)
        {
            var notification = new NotificationRequest
            {
                BadgeNumber = 1,
                Title = title,
                Description = content,
                NotificationId = id,
                Subtitle = subtitle,
                Schedule =
                {
                    NotifyTime =  DateTime.Now.AddSeconds(20)
                },
            };                        
            return LocalNotificationCenter.Current.Show(notification);
        }

        public void Delete(int id)
        {
            LocalNotificationCenter.Current.CancelAll();
        }
    }
}
