using System;
using System.Collections.Generic;
using System.Globalization;
using TimeTrackerXamarin._UseCases.Contracts;
using Xamarin.Forms;

namespace TimeTrackerXamarin.Services
{
    public class LogToColorConverter : IValueConverter
    {
        private Dictionary<Log.LogType, Color> LogToColor = new Dictionary<Log.LogType, Color>
        {
            { Log.LogType.Debug, Color.LightSkyBlue },
            { Log.LogType.Info, Color.LightGray },
            { Log.LogType.Error, Color.Red },
            { Log.LogType.Warn, Color.Orange },
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Color.LightGray;
            var type = (Log.LogType) value;
            if (!LogToColor.ContainsKey(type))
            {
                return Color.LightGray;
            }

            return LogToColor[type];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}