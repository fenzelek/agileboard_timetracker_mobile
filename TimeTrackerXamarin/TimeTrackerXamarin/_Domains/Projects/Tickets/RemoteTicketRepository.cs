using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects.Tickets;

namespace TimeTrackerXamarin._Domains.Projects.Tickets
{
    public class RemoteTicketRepository : ITicketRepository
    {
        private readonly IRemoteTicketSource remoteSource;
        private readonly ILocalTicketSource localSource;

        public RemoteTicketRepository(IRemoteTicketSource remoteSource, ILocalTicketSource localSource)
        {
            this.remoteSource = remoteSource;
            this.localSource = localSource;
        }

        public async Task<List<Ticket>> GetAll(int projectId, int companyId)
        {
            var tickets = await remoteSource.GetAll(projectId, companyId);
            await localSource.SaveTickets(tickets);
            return tickets;
        }

        public async Task<TicketDetails> GetDetails(int projectId, int ticketId, int companyId)
        {
            var details = await remoteSource.GetDetails(projectId, ticketId, companyId);
            await localSource.SaveTicketDetails(details);
            return details;
        }

        public Task<Ticket> GetTicket(int ticketId)
        {
            return localSource.GetTicket(ticketId);
        }

        public Task CreateTicket(NewTicket ticket, int companyId)
        {
            return remoteSource.CreateTicket(ticket, companyId);
        }
    }
}