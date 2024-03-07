using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects;
using TimeTrackerXamarin._UseCases.Contracts.Projects.Tickets;

namespace TimeTrackerXamarin._Domains.Projects.Tickets
{
    public class TicketService : ITicketService
    {

        private readonly ITicketRepository repository;

        public TicketService(ITicketRepository repository)
        {
            this.repository = repository;
        }

        public Task<List<Ticket>> GetTickets(int projectId, int companyId)
        {
            return repository.GetAll(projectId, companyId);
        }

        public Task<TicketDetails> GetDetails(int projectId, int ticketId, int companyId)
        {
            return repository.GetDetails(projectId, ticketId, companyId);
        }

        public Task<Ticket> GetTicket(int ticketId)
        {
            return repository.GetTicket(ticketId);
        }

        public Task CreateTicket(NewTicket ticket, int companyId)
        {
            return repository.CreateTicket(ticket, companyId);
        }
    }
}