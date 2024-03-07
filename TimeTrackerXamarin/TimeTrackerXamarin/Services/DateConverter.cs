using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TimeTrackerXamarin.Services
{
    public class DateConverter : IValueConverter
    {
        #region IValueConverter implementation

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool withSeconds;
            if (value == null)
                return string.Empty;
            if (parameter == null)
                parameter = (bool)true;
            withSeconds = bool.Parse(parameter.ToString());
            TimeSpan datetime = TimeSpan.FromSeconds((long)value);
            if(withSeconds)
                return datetime.ToString(@"hh\:mm\:ss");
            else
                return datetime.ToString(@"hh\:mm");
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
