using System;
using System.Collections.Generic;

namespace TimeTrackerXamarin._UseCases.Contracts
{
    public interface ILogger
    {
        
        void SetDebug(bool enabled);
        bool IsDebug();
        
        void Info(object any);
        void Warn(object any);
        void Debug(object any);
        void Error(object any);
        void Error(object any, Exception exception);
        
    }
}