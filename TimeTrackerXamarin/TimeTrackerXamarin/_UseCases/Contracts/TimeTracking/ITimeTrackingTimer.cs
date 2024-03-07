using System.Threading;
using System.Threading.Tasks;

namespace TimeTrackerXamarin._UseCases.Contracts.TimeTracking
{
    public interface ITimeTrackingTimer
    {

        Task Run(CancellationToken token);

    }
}