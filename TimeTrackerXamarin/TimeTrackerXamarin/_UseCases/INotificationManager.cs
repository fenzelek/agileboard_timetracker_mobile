using System;


namespace TimeTrackerXamarin.iOS.Services
{
    public interface INotificationManager<T>
    {
        event EventHandler NotificationReceived;
        void Initialize(T appdelegate);
        void SendNotification(string title, string message, DateTime? notifyTime = null);
        void ReceiveNotification(string title, string message);

        void CancelScheduledNotifications();
    }
}