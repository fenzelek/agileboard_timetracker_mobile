namespace TimeTrackerXamarin._UseCases.Contracts
{
    public class NewTicket
    {
        public string name { get; set; }
        public int type_id { get; set; } = 2;
        public int estimate_time { get; set; }
        //Y-m-d H:i:s
        public int sprint_id { get; set; }
        public int reporter_id { get; set; }
        public int project_id { get; set; }
    }
}
