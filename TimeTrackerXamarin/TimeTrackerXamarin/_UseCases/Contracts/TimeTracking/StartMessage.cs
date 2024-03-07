namespace TimeTrackerXamarin._UseCases.Contracts.TimeTracking
{
    public class StartMessage
    {
        public string Title { get; set; }
        public long TotalTime { get; set; }
        public string RemindMessage { get; set; }
    }
}