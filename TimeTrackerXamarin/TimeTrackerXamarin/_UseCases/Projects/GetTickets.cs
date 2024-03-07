using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects;

namespace TimeTrackerXamarin._UseCases.Projects
{
    public class GetTickets
    {
        private readonly IFactory<ITicketService> ticketServiceFactory;
        private ITicketService ticketService;

        public GetTickets(IFactory<ITicketService> ticketServiceFactory)
        {
            this.ticketServiceFactory = ticketServiceFactory;
        }

        public void SwitchConnection(bool connection)
        {
            ticketService = ticketServiceFactory.Create(connection);
        }

        public Task<List<Ticket>> GetAll(int projectId, int companyId)
        {
            return ticketService.GetTickets(projectId, companyId);
        }

        public Task<TicketDetails> GetDetails(int projectId, int ticketId, int companyId)
        {
            return ticketService.GetDetails(projectId, ticketId, companyId);
        }

        public Task<Ticket> GetOne(int ticketId)
        {
            return ticketService.GetTicket(ticketId);
        }
    }
}