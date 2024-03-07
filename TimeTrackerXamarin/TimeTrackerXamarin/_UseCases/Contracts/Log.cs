using System;

namespace TimeTrackerXamarin._UseCases.Contracts
{
    public class Log
    {
        public enum LogType
        {
            Info,
            Warn,
            Error,
            Debug
        }
        
        public LogType Type { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Message { get; set; }
    }
}