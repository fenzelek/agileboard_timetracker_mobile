using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking.Frame;

namespace TimeTrackerXamarin._Domains.TimeTracking.Frame
{
    public class RemoteFrameRepository : IFrameRepository
    {

        private readonly IRemoteFrameSource remoteSource;
        private readonly ILocalFrameSource localSource;

        public RemoteFrameRepository(IRemoteFrameSource remoteSource, ILocalFrameSource localSource)
        {
            this.remoteSource = remoteSource;
            this.localSource = localSource;
        }

        public Task<bool> SaveFrame(TimeFrame frame)
        {
            return localSource.SaveFrame(frame);
        }

        public Task<TimeFrame> GetUnfinishedFrame()
        {
            return localSource.GetUnfinishedFrame();
        }

        public Task<List<TimeFrame>> GetSavedFrames(bool all)
        {
            return localSource.GetSavedFrames(all);
        }

        public Task<bool> RemoveSavedFrame(int frameId)
        {
            return localSource.RemoveSavedFrame(frameId);
        }

        public Task<bool> UpdateFrame(TimeFrame frame)
        {
            return localSource.UpdateFrame(frame);
        }

        public Task<bool> UpdateSentFrame(int frameId)
        {
            return localSource.UpdateSentFrame(frameId);
        }

        public Task SendFrames(List<TimeFrame> frames)
        {
            return remoteSource.SendFrames(frames);
        }
    }
}