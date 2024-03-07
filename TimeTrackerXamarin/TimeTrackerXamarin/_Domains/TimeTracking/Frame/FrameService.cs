using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking.Frame;

namespace TimeTrackerXamarin._Domains.TimeTracking.Frame
{
    public class FrameService : IFrameService
    {

        private readonly IFrameRepository repository;

        public FrameService(IFrameRepository repository)
        {
            this.repository = repository;
        }

        public Task<bool> SaveFrame(TimeFrame frame)
        {
            return repository.SaveFrame(frame);
        }

        public Task<TimeFrame> GetUnfinishedFrame()
        {
            return repository.GetUnfinishedFrame();
        }

        public Task<List<TimeFrame>> GetSavedFrames(bool all)
        {
            return repository.GetSavedFrames(all);
        }

        public Task<bool> RemoveSavedFrame(int frameId)
        {
            return repository.RemoveSavedFrame(frameId);
        }

        public Task<bool> UpdateFrame(TimeFrame frame)
        {
            return repository.UpdateFrame(frame);
        }

        public Task<bool> UpdateSentFrame(int frameId)
        {
            return repository.UpdateSentFrame(frameId);
        }

        public Task SendFrames(List<TimeFrame> frames)
        {
            return repository.SendFrames(frames);
        }
    }
}