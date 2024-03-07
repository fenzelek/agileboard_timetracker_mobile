using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects;

namespace TimeTrackerXamarin._UseCases.Projects
{
    public class CreateTicket
    {
        private readonly ITicketService ticketService;

        public CreateTicket(IFactory<ITicketService> ticketServiceFactory)
        {
            ticketService = ticketServiceFactory.Create(true);
        }

        public Task Create(NewTicket ticket, int companyId)
        {
            return ticketService.CreateTicket(ticket, companyId);
        }

    }
}