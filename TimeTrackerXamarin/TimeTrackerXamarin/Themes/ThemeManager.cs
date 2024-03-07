using System;
using System.Linq;
using TimeTrackerXamarin._UseCases.Contracts;
using Xamarin.Forms;

namespace TimeTrackerXamarin.Themes
{
    public class ThemeManager : IThemeManager
    {
        private readonly ILogger logger;

        public ThemeManager(ILogger logger)
        {
            this.logger = logger;
        }

        public void UpdateAppTheme(OSAppTheme theme)
        {
            var mergedDictionaries = Application.Current.Resources.MergedDictionaries;
            if (mergedDictionaries == null) return;

            ResourceDictionary resourceDictionaryTheme;
            switch (theme)
            {
                case OSAppTheme.Dark:
                    resourceDictionaryTheme = new Dark();
                    break;
                case OSAppTheme.Unspecified:
                case OSAppTheme.Light:
                default:
                    resourceDictionaryTheme = new Light();
                    break;
            }

            if (mergedDictionaries.Any(entry => entry.GetType() == resourceDictionaryTheme.GetType()))
            {
                return;
            }
            logger.Info($"Theme has been changed to {theme}");
            mergedDictionaries.Clear();
            mergedDictionaries.Add(resourceDictionaryTheme);
        }
    }
}