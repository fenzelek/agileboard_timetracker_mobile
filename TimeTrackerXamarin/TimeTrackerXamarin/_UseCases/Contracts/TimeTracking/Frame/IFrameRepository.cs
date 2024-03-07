using System.Collections.Generic;
using System.Threading.Tasks;

namespace TimeTrackerXamarin._UseCases.Contracts.TimeTracking.Frame
{
    public interface IFrameRepository
    {
        Task<bool> SaveFrame(TimeFrame frame);
        Task<TimeFrame> GetUnfinishedFrame();
        Task<List<TimeFrame>> GetSavedFrames(bool all);
        Task<bool> RemoveSavedFrame(int frameId);
        Task<bool> UpdateFrame(TimeFrame frame);
        Task<bool> UpdateSentFrame(int frameId);
        Task SendFrames(List<TimeFrame> frames);
    }
}