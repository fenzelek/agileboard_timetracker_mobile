using System;
using System.Collections.Generic;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;

namespace TimeTrackerXamarin._Domains.TimeTracking.Frame
{
    public class FrameRejectedException : Exception
    {
        public List<TimeFrame> RejectedFrames { get; }

        public FrameRejectedException(List<TimeFrame> rejectedFrames)
        {
            RejectedFrames = rejectedFrames;
        }
    }
}