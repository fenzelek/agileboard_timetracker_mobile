using TimeTrackerXamarin._UseCases.Contracts;

namespace TimeTrackerXamarin._Domains.Projects.Tickets.Mapper
{
    public class TicketDetailsMapper : IMapper<TicketDetails,TicketDetailsDto>
    {
        public TicketDetails Map(TicketDetailsDto ticket)
        {
            TicketDetails result = new TicketDetails();
            result.id = ticket.id;
            result.project_id = ticket.project_id;
            result.sprint_id = ticket.sprint_id;
            result.assigned_id = ticket.assigned_id;            

            result.sprint_name = ticket.sprint.data.name;
            result.assigned_user_fullname = (ticket.assigned_user.data != null) ? $"{ticket.assigned_user.data.first_name} {ticket.assigned_user.data.last_name}" : "";
            result.reporting_user_fullname = (ticket.reporting_user.data != null) ? $"{ticket.reporting_user.data.first_name} {ticket.reporting_user.data.last_name}" : "";
            result.estimate_time = ticket.estimate_time;
            result.title = ticket.title;
            result.name = ticket.name;
            result.description = ticket.description;
            result.alltime = (ticket.stats == null) ? 0 : ticket.stats.data.tracked_summary;

            return result;
        }

        public TicketDetails MapDB(TicketDetailsDto tomap)
        {
            return Map(tomap);
        }
    }
}
