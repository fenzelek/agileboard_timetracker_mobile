using System.Collections.Generic;
using Flurl.Http;
using TimeTrackerXamarin._Domains.API;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects;
using TimeTrackerXamarin._UseCases.Contracts.Projects.Tickets;

namespace TimeTrackerXamarin._Domains.Projects.Tickets
{
    public class TicketServiceFactory : IFactory<ITicketService>
    {
        private readonly IRemoteTicketSource remoteSource;
        private readonly ILocalTicketSource localSource;
        private readonly IMapper<TicketDetails, Ticket> ticketDetailsMapper;

        public TicketServiceFactory(IRemoteTicketSource remoteSource, ILocalTicketSource localSource, IMapper<TicketDetails, Ticket> ticketDetailsMapper)
        {
            this.remoteSource = remoteSource;
            this.localSource = localSource;
            this.ticketDetailsMapper = ticketDetailsMapper;
        }

        private ITicketRepository CreateRepository(bool connection)
        {
            if (connection)
            {
                return new RemoteTicketRepository(remoteSource, localSource);
            }

            return new LocalTicketRepository(localSource, ticketDetailsMapper);
        }

        public ITicketService Create(bool connection)
        {
            var repository = CreateRepository(connection);
            return new TicketService(repository);
        }
    }
}