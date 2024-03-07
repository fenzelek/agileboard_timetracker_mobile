using System;
using Moq;
using TimeTrackerXamarin._Domains.Projects.Tickets;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects.Tickets;
using Xunit;

namespace TimeTrackerXamarin.Test.Unit.Domain.Projects.Tickets
{
    public class LocalTicketRepositoryTest
    {
        private readonly Mock<ILocalTicketSource> source;
        private readonly Mock<IMapper<TicketDetails, Ticket>> mapper;

        private readonly LocalTicketRepository repository;

        public LocalTicketRepositoryTest()
        {
            source = new Mock<ILocalTicketSource>();
            mapper = new Mock<IMapper<TicketDetails, Ticket>>();

            repository = new LocalTicketRepository(source.Object, mapper.Object);
        }

        /*
         * @feature Projects
         * @scenario Get all project's tasks
         * @case Returns tasks from local data source
         */
        [Fact]
        public async void GetAllProjectTasksFromLocal()
        {
            //GIVEN
            var projectId = 1;
            var companyId = 2;

            //WHEN
            await repository.GetAll(projectId, companyId);

            //THEN
            source.Verify(mock => mock.GetAll(projectId), Times.Once);
        }

        /*
         * @feature Projects
         * @scenario Get task's details
         * @case Returns task's details from local data source
         */
        [Fact]
        public async void GetTaskDetailsFromLocal()
        {
            //GIVEN
            var projectId = 1;
            var ticketId = 2;
            var companyId = 3;

            //WHEN
            await repository.GetDetails(1, 2, 3);

            //THEN
            source.Verify(mock => mock.GetDetails(ticketId), Times.Once);
            mapper.Verify(mock => mock.Map(It.IsAny<Ticket>()));
        }
        
        /*
         * @feature Projects
         * @scenario Get task
         * @case Returns task from local data source
         */
        [Fact]
        public async void GetTicketFromLocal()
        {
            //GIVEN
            var ticketId = 1;

            //WHEN
            await repository.GetTicket(1);

            //THEN
            source.Verify(mock => mock.GetTicket(ticketId), Times.Once);
        }
        
        /*
         * @feature Projects
         * @scenario Create task
         * @case Throws exception, task cannot be created locally
         */
        [Fact]
        public async void CreateTaskLocal_Exception()
        {
            //GIVEN
            var companyId = 1;
            var ticket = new NewTicket();

            //WHEN / THEN
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await repository.CreateTicket(ticket, companyId);
            });
        }
    }
}