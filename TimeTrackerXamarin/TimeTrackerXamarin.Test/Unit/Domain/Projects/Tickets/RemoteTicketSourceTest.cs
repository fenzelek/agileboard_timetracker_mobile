using System.Collections.Generic;
using Flurl.Http;
using Flurl.Http.Testing;
using Moq;
using TimeTrackerXamarin._Domains.API.dto;
using TimeTrackerXamarin._Domains.Projects.Tickets;
using TimeTrackerXamarin._Domains.Projects.Tickets.Dto;
using TimeTrackerXamarin._Domains.Projects.Tickets.Mapper;
using TimeTrackerXamarin._UseCases.Contracts;
using Xunit;

namespace TimeTrackerXamarin.Test.Unit.Domain.Projects.Tickets
{
    public class RemoteTicketSourceTest
    {
        private readonly IFlurlClient client;
        private readonly Mock<ITokenService> tokenService;
        private readonly IMapper<List<Ticket>, List<Sprint>> ticketMapper;
        private readonly IMapper<TicketDetails, TicketDetailsDto> ticketDetailsMapper;

        private readonly RemoteTicketSource source;

        public RemoteTicketSourceTest()
        {
            client = new FlurlClient("http://api.api");
            tokenService = new Mock<ITokenService>();
            ticketDetailsMapper = new TicketDetailsMapper();
            ticketMapper = new TicketListMapper();
            
            source = new RemoteTicketSource(client, tokenService.Object, ticketMapper,
                ticketDetailsMapper);
        }

        /*
         * @feature Projects
         * @scenario Get all tickets
         * @case Returns tickets from remote api
         */
        [Fact]
        public async void GetAllTickets_Remote()
        {
            using var httpTest = new HttpTest();

            //GIVEN
            var projectId = 1;
            var companyId = 2;
            var expectedTicket = new Ticket
            {
                assigned_id = 1,
                id = 2,
                name = "test",
                project_id = 3,
                sprint_id = 4,
                title = "testtitle"
            };

            var response = new JSONDataDto<List<Sprint>>
            {
                data = new List<Sprint>
                {
                    new Sprint
                    {
                        tickets = new JSONDataDto<List<Ticket>>
                        {
                            data = new List<Ticket>
                            {
                                expectedTicket
                            }
                        }
                    }
                }
            };
            httpTest
                .ForCallsTo($"*/projects/{projectId}/statuses*")
                .WithQueryParams(new
                {
                    selected_company_id = companyId,
                    tickets = 1
                })
                .RespondWithJson(response);

            //WHEN
            var result = await source.GetAll(projectId, companyId);
            
            Assert.NotEmpty(result);
            Assert.Single(result);
            Assert.Equivalent(expectedTicket, result[0]);
        }
        
        /*
         * @feature Projects
         * @scenario Get ticket details
         * @case Returns ticket's details from remote api
         */
        [Fact]
        public async void GetAllTicketDetails_Remote()
        {
            using var httpTest = new HttpTest();
            
            //GIVEN
            var projectId = 1;
            var companyId = 2;
            var ticketId = 3;

            var expected = new TicketDetails
            {
                id = 1,
                project_id = projectId,
                sprint_id = 3,
                assigned_id = 4,
                sprint_name = "name",
                estimate_time = 200,
                title = "title",
                name = "name",
                description = "desc",
                assigned_user_fullname = "",
                reporting_user_fullname = ""
            };

            var response = new JSONDataDto<TicketDetailsDto>
            {
                data = new TicketDetailsDto
                {
                    id = expected.id,
                    project_id = expected.project_id,
                    sprint_id = expected.sprint_id,
                    sprint = new JSONDataDto<Sprint>
                    {
                        data = new Sprint
                        {
                            name = expected.sprint_name
                        }
                    },
                    assigned_id = expected.assigned_id,
                    estimate_time = expected.estimate_time,
                    title = expected.title,
                    name = expected.name,
                    description = expected.description,
                    assigned_user = new JSONDataDto<User>(),
                    reporting_user = new JSONDataDto<User>()
                }
            };
            
            httpTest
                .ForCallsTo($"*/projects/{projectId}/tickets/{ticketId}*")
                .WithQueryParams(new
                {
                    selected_company_id = companyId,
                    tickets = 1
                })
                .RespondWithJson(response);

            tokenService.Setup(mock => mock.Get()).Returns("");
            
            //WHEN
            var result = await source.GetDetails(projectId, ticketId, companyId);
            
            Assert.NotNull(result);
            Assert.Equivalent(expected, result);
        }
        
        /*
         * @feature Projects
         * @scenario Create task
         * @case Creates ticket using remote api
         */
        [Fact]
        public async void CreateTask_Remote()
        {
            using var httpTest = new HttpTest();

            //GIVEN
            var companyId = 1;
            var projectId = 2;

            var newTicket = new NewTicket
            {
                estimate_time = 10,
                name = "name",
                project_id = projectId,
                reporter_id = 1,
                sprint_id = 2,
                type_id = 3
            };

            var body = new CreateTicketDto
            {
                estimate_time = newTicket.estimate_time,
                name = newTicket.name,
                reporter_id = newTicket.reporter_id,
                sprint_id = newTicket.sprint_id,
                type_id = newTicket.type_id
            };
            
            httpTest
                .ForCallsTo($"*/projects/{projectId}/tickets*")
                .WithRequestJson(body)
                .WithoutQueryParams(new
                {
                    selected_company_id = companyId,
                })
                .RespondWith();
            
            //WHEN
            await source.CreateTicket(newTicket, companyId);
            
            //THEN
            //no error
        }
    }
}