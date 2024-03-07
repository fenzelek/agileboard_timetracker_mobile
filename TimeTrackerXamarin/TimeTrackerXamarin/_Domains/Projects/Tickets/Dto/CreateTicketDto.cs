namespace TimeTrackerXamarin._Domains.Projects.Tickets.Dto
{
    public class CreateTicketDto
    {
        public string name { get; set; }
        public int type_id { get; set; }
        public int estimate_time { get; set; }
        public int sprint_id { get; set; }
        public int reporter_id { get; set; }
    }
}
