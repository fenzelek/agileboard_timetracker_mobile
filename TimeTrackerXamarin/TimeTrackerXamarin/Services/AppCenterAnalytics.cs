using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Analytics;
using TimeTrackerXamarin._UseCases.Contracts;
using Xamarin.Essentials;

namespace TimeTrackerXamarin.Services
{
    public class AppCenterAnalytics : IAppCenterAnalytics
    {
        public void TrackOperation(string operation)
        {
            string userName;
            if (Preferences.ContainsKey("current_user_name"))
                userName = Preferences.Get("current_user_name", "Unknown");
            else
                userName = "Unkown";
            Analytics.TrackEvent(userName, new Dictionary<string, string>
            {
                {operation, DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}
            });
        }
    }
}