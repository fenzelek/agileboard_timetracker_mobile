using System.Collections.Generic;
using System.Threading.Tasks;

namespace TimeTrackerXamarin._UseCases.Contracts.TimeTracking.Frame
{
    public interface IRemoteFrameSource
    {
        Task SendFrames(List<TimeFrame> frames);
    }
}