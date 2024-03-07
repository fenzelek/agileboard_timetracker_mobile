using System;
using System.Globalization;
using TimeTrackerXamarin.i18n;
using Xamarin.Forms;

namespace TimeTrackerXamarin.Services
{
    public class LanguageFlagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var lang = (Language)value;
            return $"{lang.Flag} {lang.Name}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}