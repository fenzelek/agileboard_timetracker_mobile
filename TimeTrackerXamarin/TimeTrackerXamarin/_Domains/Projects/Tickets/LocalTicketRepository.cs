using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects.Tickets;

namespace TimeTrackerXamarin._Domains.Projects.Tickets
{
    public class LocalTicketRepository : ITicketRepository
    {
        private readonly ILocalTicketSource source;
        private readonly IMapper<TicketDetails, Ticket> ticketDetailsMapper;

        public LocalTicketRepository(ILocalTicketSource source, IMapper<TicketDetails, Ticket> ticketDetailsMapper)
        {
            this.source = source;
            this.ticketDetailsMapper = ticketDetailsMapper;
        }

        public Task<List<Ticket>> GetAll(int projectId, int companyId)
        {
            return source.GetAll(projectId);
        }

        public async Task<TicketDetails> GetDetails(int projectId, int ticketId, int companyId)
        {
            var details = await source.GetDetails(ticketId);
            if (details != null)
            {
                return details;
            }

            var ticket = await GetTicket(ticketId);
            return ticketDetailsMapper.Map(ticket);
        }

        public Task<Ticket> GetTicket(int ticketId)
        {
            return source.GetTicket(ticketId);
        }

        public Task CreateTicket(NewTicket ticket, int companyId)
        {
            throw new InvalidOperationException("Ticket cannot be created offline.");
        }
    }
}