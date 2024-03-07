using System.Collections.Generic;
using TimeTrackerXamarin._UseCases.Contracts;

namespace TimeTrackerXamarin._Domains.Projects.Tickets.Mapper
{
    public class TicketListMapper: IMapper<List<Ticket>,List<Sprint>>
    {
        public List<Ticket> Map(List<Sprint> sprints)
        {
            if (sprints == null || sprints.Count < 1)
            {
                return null;
            }
            List<Ticket> result = new List<Ticket>();
            foreach(Sprint sprint in sprints)
            {
                foreach(Ticket ticket in sprint.tickets.data)
                {
                    result.Add(ticket);
                }
            }
            result.Sort((a, b) => b.title.CompareTo(a.title));
            return result;
        }

        public List<Ticket> MapDB(List<Sprint> tomap)
        {
            return Map(tomap);
        }
    }
}
