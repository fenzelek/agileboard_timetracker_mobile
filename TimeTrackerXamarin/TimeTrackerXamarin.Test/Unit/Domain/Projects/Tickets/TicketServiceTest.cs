using Moq;
using TimeTrackerXamarin._Domains.Projects.Tickets;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects.Tickets;
using Xunit;

namespace TimeTrackerXamarin.Test.Unit.Domain.Projects.Tickets
{
    public class TicketServiceTest
    {
        private readonly TicketService service;
        private readonly Mock<ITicketRepository> repository;

        public TicketServiceTest()
        {
            repository = new Mock<ITicketRepository>();
            service = new TicketService(repository.Object);
        }

        /*
         * @feature Projects
         * @scenario Get all tickets from service
         * @case Lists all tickets 
         */
        [Fact]
        public async void GetTickets_All()
        {
            // GIVEN
            var companyId = 1;
            var projectId = 2;

            // WHEN
            await service.GetTickets(projectId, companyId);

            // THEN
            repository.Verify(mock => mock.GetAll(projectId, companyId), Times.Once);
        }
        
        /*
         * @feature Projects
         * @scenario Get one ticket from service
         * @case Gets one ticket
         */
        [Fact]
        public async void GetTickets_One()
        {
            // GIVEN
            var ticketId = 1;

            // WHEN
            await service.GetTicket(ticketId);

            // THEN
            repository.Verify(mock => mock.GetTicket(ticketId), Times.Once);
        }
        
        /*
         * @feature Projects
         * @scenario Get ticket's details from service
         * @case Gets ticket's details
         */
        [Fact]
        public async void GetTickets_Details()
        {
            // GIVEN
            var projectId = 1;
            var ticketId = 2;
            var companyId = 3;

            // WHEN
            await service.GetDetails(projectId, ticketId, companyId);

            // THEN
            repository.Verify(mock => mock.GetDetails(projectId, ticketId, companyId), Times.Once);
        }
        
        /*
         * @feature Projects
         * @scenario Get ticket's details from service
         * @case Gets ticket's details
         */
        [Fact]
        public async void CreateTicket()
        {
            // GIVEN
            var companyId = 1;
            var ticket = new NewTicket
            {
                estimate_time = 10,
                name = "test",
                project_id = 2,
                reporter_id = 3,
                sprint_id = 4,
                type_id = 5
            };

            // WHEN
            await service.CreateTicket(ticket, companyId);

            // THEN
            repository.Verify(mock => mock.CreateTicket(ticket, companyId), Times.Once);
        }
    }
}