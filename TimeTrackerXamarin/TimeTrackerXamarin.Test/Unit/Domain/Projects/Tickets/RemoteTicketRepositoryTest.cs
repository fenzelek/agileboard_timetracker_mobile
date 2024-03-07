using System;
using System.Collections.Generic;
using Moq;
using TimeTrackerXamarin._Domains.Projects.Tickets;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects.Tickets;
using Xunit;

namespace TimeTrackerXamarin.Test.Unit.Domain.Projects.Tickets
{
    public class RemoteTicketRepositoryTest
    {
        private readonly Mock<ILocalTicketSource> local;
        private readonly Mock<IRemoteTicketSource> remote;

        private readonly RemoteTicketRepository repository;

        public RemoteTicketRepositoryTest()
        {
            local = new Mock<ILocalTicketSource>();
            remote = new Mock<IRemoteTicketSource>();

            repository = new RemoteTicketRepository(remote.Object, local.Object);
        }

        /*
         * @feature Projects
         * @scenario Get all project's tasks
         * @case Returns tasks from remote data source and saves it locally
         */
        [Fact]
        public async void GetAllProjectTasksFromRemote()
        {
            //GIVEN
            var projectId = 1;
            var companyId = 2;

            //WHEN
            await repository.GetAll(projectId, companyId);

            //THEN
            remote.Verify(mock => mock.GetAll(projectId, companyId), Times.Once);
            local.Verify(mock => mock.SaveTickets(It.IsAny<List<Ticket>>()), Times.Once);
        }

        /*
         * @feature Projects
         * @scenario Get task's details
         * @case Returns task's details from remote data source and saves it locally
         */
        [Fact]
        public async void GetTaskDetailsFromRemote()
        {
            //GIVEN
            var projectId = 1;
            var ticketId = 2;
            var companyId = 3;

            //WHEN
            await repository.GetDetails(1, 2, 3);

            //THEN
            remote.Verify(mock => mock.GetDetails(projectId, ticketId, companyId), Times.Once);
            local.Verify(mock => mock.SaveTicketDetails(It.IsAny<TicketDetails>()));
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
            local.Verify(mock => mock.GetTicket(ticketId), Times.Once);
        }

        /*
         * @feature Projects
         * @scenario Create task
         * @case Creates task
         */
        [Fact]
        public async void CreateTaskRemote()
        {
            //GIVEN
            var companyId = 1;
            var ticket = new NewTicket
            {
                estimate_time = 100,
                name = "test",
                project_id = 1,
                reporter_id = 2,
                sprint_id = 3,
                type_id = 4
            };

            //WHEN
            await repository.CreateTicket(ticket, companyId);
            
            //THEN
            remote.Verify(mock => mock.CreateTicket(ticket, companyId), Times.Once);
        }
    }
}