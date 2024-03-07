using System.Collections.Generic;
using System.Threading.Tasks;

namespace TimeTrackerXamarin._UseCases.Contracts.Projects.Tickets
{
    public interface IRemoteTicketSource
    {
        Task<List<Ticket>> GetAll(int projectId, int companyId);
        Task<TicketDetails> GetDetails(int projectId, int ticketId, int companyId);
        Task CreateTicket(NewTicket ticket, int companyId);
    }
}