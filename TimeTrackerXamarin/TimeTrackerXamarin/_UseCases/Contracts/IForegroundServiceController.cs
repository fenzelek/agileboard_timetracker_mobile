using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimeTrackerXamarin._UseCases.Contracts
{
    public interface IForegroundServiceController
    {
        Task<bool> StartService(bool blockSending = false);
        Task<bool> StopService();
        bool IsEnabled();
    }
}
