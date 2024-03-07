using TimeTrackerXamarin._UseCases.Contracts;

namespace TimeTrackerXamarin._Domains.Projects.Tickets.Mapper
{
    public class TicketDetailsFromTicketMapper : IMapper<TicketDetails, Ticket>
    {
        public TicketDetails Map(Ticket tomap)
        {
            return new TicketDetails
            {
                title = tomap.title,
                name = tomap.name,
                sprint_name = "...",
                assigned_user_fullname = "...",
                reporting_user_fullname = "...",
                estimate_time = 0,
                description = "...",
                alltime = 0
            };
        }

        public TicketDetails MapDB(Ticket tomap)
        {
            return Map(tomap);
        }
    }
}