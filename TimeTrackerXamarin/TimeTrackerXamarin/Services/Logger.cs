using System;
using System.Collections.Generic;
using System.Linq;
using TimeTrackerXamarin._UseCases.Contracts;
using Xamarin.Forms.Internals;
using Log = TimeTrackerXamarin._UseCases.Contracts.Log;
using LogType = TimeTrackerXamarin._UseCases.Contracts.Log.LogType;

namespace TimeTrackerXamarin.Services
{
    public class Logger : ILogger
    {
        private readonly ILogStorage logStorage;

        private bool debug = false;

        public Logger(ILogStorage logStorage)
        {
            this.logStorage = logStorage;
        }

        private void Log(LogType type, object any)
        {
            if (type == LogType.Debug && !debug)
            {
                return;
            }

            var log = new Log
            {
                Type = type, 
                Date = DateTimeOffset.Now, 
                Message = $"{any}"
            };
            logStorage.Write(log);
        }

        public void SetDebug(bool enabled)
        {
            debug = enabled;
        }

        public bool IsDebug()
        {
            return debug;
        }

        public void Info(object any)
        {
            Log(LogType.Info, any);
        }

        public void Warn(object any)
        {
            Log(LogType.Warn, any);
        }

        public void Debug(object any)
        {
            Log(LogType.Debug, any);
        }

        public void Error(object any)
        {
            Log(LogType.Error, any);
        }

        public void Error(object any, Exception exception)
        {
            Log(LogType.Error, $"{any} Exception: {exception}");
        }
    }
}