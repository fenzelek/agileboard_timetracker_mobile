using System;
using System.Globalization;
using Prism.Ioc;
using TimeTrackerXamarin.i18n;
using Xamarin.Forms;

namespace TimeTrackerXamarin.Services
{
    public class DateTimeConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DateTime dt))
            {
                return null;
            }

            return dt.ToString("dddd, dd.MM.yyyy", ContainerLocator.Container.Resolve<ITranslationManager>().Language.Culture);
        }
        //duplicated method for testing
        public object ConvertTest(object value, Language language)
        {
            if (!(value is DateTime dt))
            {
                return null;
            }

            return dt.ToString("dddd, dd.MM.yyyy", language.Culture);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}