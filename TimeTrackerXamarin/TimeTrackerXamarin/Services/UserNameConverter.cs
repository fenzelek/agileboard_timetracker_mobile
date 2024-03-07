using System;
using System.Collections.Generic;
using System.Text;
using TimeTrackerXamarin._UseCases.Contracts;
using Xamarin.Forms;

namespace TimeTrackerXamarin.Services
{
    public class UserNameConverter : IValueConverter
    {
        #region IValueConverter implementation

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return string.Empty;
            
            var user = (User)value;

            return $"{user.first_name} {user.last_name}";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
