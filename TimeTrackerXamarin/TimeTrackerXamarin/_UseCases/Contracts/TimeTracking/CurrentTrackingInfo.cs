namespace TimeTrackerXamarin._UseCases.Contracts.TimeTracking
{
    public class CurrentTrackingInfo
    {
        public TimeFrame Frame { get; set; }
        public long TotalTime { get; set; }
        public long TaskTime { get; set; }
        public string TaskTitle { get; set; }
        public int TaskId { get; set; }
    }
}