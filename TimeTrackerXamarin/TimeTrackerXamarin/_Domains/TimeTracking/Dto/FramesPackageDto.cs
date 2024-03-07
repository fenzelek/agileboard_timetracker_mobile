using System.Collections.Generic;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;

namespace TimeTrackerXamarin._Domains.TimeTracking.Dto
{
    public class FramesPackageDto
    {
        public List<TimeFrame> frames { get; set; }
    }
}
