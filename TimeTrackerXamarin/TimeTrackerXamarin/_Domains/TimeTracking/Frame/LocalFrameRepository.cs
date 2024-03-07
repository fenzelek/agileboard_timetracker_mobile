using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking.Frame;

namespace TimeTrackerXamarin._Domains.TimeTracking.Frame
{
    public class LocalFrameRepository : IFrameRepository
    {

        private readonly ILocalFrameSource source;

        public LocalFrameRepository(ILocalFrameSource source)
        {
            this.source = source;
        }

        public Task<bool> SaveFrame(TimeFrame frame)
        {
            return source.SaveFrame(frame);
        }

        public Task<TimeFrame> GetUnfinishedFrame()
        {
            return source.GetUnfinishedFrame();
        }

        public Task<List<TimeFrame>> GetSavedFrames(bool all)
        {
            return source.GetSavedFrames(all);
        }

        public Task<bool> RemoveSavedFrame(int frameId)
        {
            return source.RemoveSavedFrame(frameId);
        }

        public Task<bool> UpdateFrame(TimeFrame frame)
        {
            return source.UpdateFrame(frame);
        }

        public Task<bool> UpdateSentFrame(int frameId)
        {
            return source.UpdateSentFrame(frameId);
        }

        public Task SendFrames(List<TimeFrame> frames)
        {
            throw new InvalidOperationException("Frames cannot be sent offline.");
        }
    }
}