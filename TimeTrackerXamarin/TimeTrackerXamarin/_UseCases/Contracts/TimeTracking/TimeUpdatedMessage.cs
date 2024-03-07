namespace TimeTrackerXamarin._UseCases.Contracts.TimeTracking
{
    public class TimeUpdatedMessage
    {
        public string TaskName { get; set; }
        public long TotalTime { get; set; }
        
        public long TotalTaskTime { get; set; }
    }
}