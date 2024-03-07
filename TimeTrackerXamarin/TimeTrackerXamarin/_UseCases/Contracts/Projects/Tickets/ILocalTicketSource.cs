using System.Collections.Generic;
using System.Threading.Tasks;

namespace TimeTrackerXamarin._UseCases.Contracts.Projects.Tickets
{
    public interface ILocalTicketSource
    {
        Task<List<Ticket>> GetAll(int projectId);
        Task<TicketDetails> GetDetails(int ticketId);
        Task<Ticket> GetTicket(int ticketId);
        Task<bool> SaveTickets(List<Ticket> tickets);
        Task<bool> SaveTicketDetails(TicketDetails ticketDetails);
    }
}