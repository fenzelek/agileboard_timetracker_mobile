using System;
using System.Collections.Generic;

namespace TimeTrackerXamarin._UseCases.Contracts
{
    public interface ILogStorage
    {

        void Write(Log log);
        
        List<Log> GetLogs(DateTime dateTime);

        List<DateTime> GetSavedLogDates();
    }
}