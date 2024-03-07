using Moq;
using TimeTrackerXamarin._Domains.Projects.Tickets;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects.Tickets;
using Xunit;

namespace TimeTrackerXamarin.Test.Unit.Domain.Projects.Tickets
{
    public class TicketServiceFactoryTest
    {
        private readonly Mock<IRemoteTicketSource> remote;
        private readonly Mock<ILocalTicketSource> local;
        private readonly Mock<IMapper<TicketDetails, Ticket>> mapper;

        private readonly TicketServiceFactory factory;

        public TicketServiceFactoryTest()
        {
            remote = new Mock<IRemoteTicketSource>();
            local = new Mock<ILocalTicketSource>();
            mapper = new Mock<IMapper<TicketDetails, Ticket>>();

            factory = new TicketServiceFactory(remote.Object, local.Object, mapper.Object);
        }

        /*
         * @feature Projects
         * @scenario Create ticket service
         * @case With internet connection, creates remote service                                        
         */
        [Fact]
        public async void CreateTicketService_Remote()
        {
            //GIVEN
            var companyId = 1;
            var projectId = 2;
            var ticketId = 3;
            var connection = true;

            //WHEN
            var service = factory.Create(connection);
            await service.GetDetails(projectId, ticketId, companyId);

            //THEN
            remote.Verify(mock => mock.GetDetails(projectId, ticketId, companyId), Times.Once);
        }

        /*
         * @feature Projects
         * @scenario Create ticket service
         * @case Without internet connection, creates local service                                         
         */
        [Fact]
        public async void CreateTicketService_Local()
        {
            //GIVEN
            var companyId = 1;
            var projectId = 2;
            var ticketId = 3;
            var connection = false;

            //WHEN
            var service = factory.Create(connection);
            await service.GetDetails(projectId, ticketId, companyId);

            //THEN
            local.Verify(mock => mock.GetDetails(ticketId), Times.Once);
        }
        
    }
}