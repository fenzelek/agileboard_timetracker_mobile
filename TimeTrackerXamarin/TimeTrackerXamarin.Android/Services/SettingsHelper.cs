using System;
using System.Threading.Tasks;
using Android.Content;
using TimeTrackerXamarin._UseCases.Contracts;

namespace TimeTrackerXamarin.Droid.Services
{
    public class SettingsHelper:ISettingsHelper
    {
        private readonly ILogger logger;

        public SettingsHelper(ILogger logger)
        {
            this.logger = logger;
        }

        public void OpenSettings()
        {
            var context = GetContext();
            if (context == null) return;
            var settingsIntent = new Intent();
            settingsIntent.SetAction(Android.Provider.Settings.ActionApplicationDetailsSettings);
            settingsIntent.AddCategory(Intent.CategoryDefault);
            settingsIntent.SetData(Android.Net.Uri.Parse("package:" + context.PackageName));
            context.StartActivity(settingsIntent);
        }
        private Context GetContext()
        {
            var context = Xamarin.Essentials.Platform.CurrentActivity ?? Xamarin.Essentials.Platform.AppContext;
            if (context == null)
            {
                logger.Error("Unable to detect current Activity or App Context. Please ensure Xamarin.Essentials is installed in your Android project initialized.");				
            }

            return context;
        }
    } 
    
    
}