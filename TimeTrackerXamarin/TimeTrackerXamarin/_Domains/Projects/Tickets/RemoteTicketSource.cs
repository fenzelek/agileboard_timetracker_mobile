using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using TimeTrackerXamarin._Domains.API;
using TimeTrackerXamarin._Domains.API.dto;
using TimeTrackerXamarin._Domains.Projects.Tickets.Dto;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects.Tickets;

namespace TimeTrackerXamarin._Domains.Projects.Tickets
{
    public class RemoteTicketSource : IRemoteTicketSource
    {
        
        private readonly IFlurlClient client;
        private readonly ITokenService tokenService;
        private readonly IMapper<List<Ticket>, List<Sprint>> ticketMapper;
        private readonly IMapper<TicketDetails, TicketDetailsDto> ticketDetailsMapper;

        public RemoteTicketSource(IFlurlClient client, ITokenService tokenService, IMapper<List<Ticket>, List<Sprint>> ticketMapper, IMapper<TicketDetails, TicketDetailsDto> ticketDetailsMapper)
        {
            this.client = client;
            this.tokenService = tokenService;
            this.ticketMapper = ticketMapper;
            this.ticketDetailsMapper = ticketDetailsMapper;
        }

        public async Task<List<Ticket>> GetAll(int projectId, int companyId)
        {
            return await RequestHelper.HandleRequest(
                action: async () =>
                {
                    var result = await client.Request("projects", projectId, "statuses")
                        .WithOAuthBearerToken(tokenService.Get())
                        .SetQueryParams(new
                        {
                            selected_company_id = companyId,
                            tickets = 1
                        })
                        .GetJsonAsync<JSONDataDto<List<Sprint>>>();
                    
                    return ticketMapper.Map(result.data);
                },
                unexpectedError: e => throw e);
        }

        public async Task<TicketDetails> GetDetails(int projectId, int ticketId, int companyId)
        {
            return await RequestHelper.HandleRequest(
                action: async () =>
                {
                    var result = await client.Request("projects", projectId, "tickets", ticketId)
                        .WithOAuthBearerToken(tokenService.Get())
                        .SetQueryParams(new
                        {
                            selected_company_id = companyId,
                            tickets = 1
                        })
                        .GetJsonAsync<JSONDataDto<TicketDetailsDto>>();

                    return ticketDetailsMapper.Map(result.data);
                },
                unexpectedError: e => throw e);
        }

        public async Task CreateTicket(NewTicket ticket, int companyId)
        {
            var dto = new CreateTicketDto
            {
                estimate_time = ticket.estimate_time,
                name = ticket.name,
                sprint_id = ticket.sprint_id,
                reporter_id = ticket.reporter_id,
                type_id = 2
            };

            await RequestHelper.HandleRequest(
                action: async () =>
                {
                    await client.Request("projects", ticket.project_id, "tickets")
                        .WithOAuthBearerToken(tokenService.Get())
                        .SetQueryParams(new
                        {
                            selected_company_id = companyId
                        })
                        .PostJsonAsync(dto);
                },
                unexpectedError: e => throw e);
        }
    }
}