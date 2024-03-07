using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using TimeTrackerXamarin._Domains.API;
using TimeTrackerXamarin._Domains.API.dto;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking.Summary;
using Xamarin.Essentials;

namespace TimeTrackerXamarin._Domains.TimeTracking.Summary
{
    public class RemoteTimeSummarySource : IRemoteTimeSummarySource
    {
        private readonly IFlurlClient client;
        private readonly ITokenService tokenService;

        public RemoteTimeSummarySource(IFlurlClient client, ITokenService tokenService)
        {
            this.client = client;
            this.tokenService = tokenService;
        }

        public async Task<List<TrackHistory>> GetTrackHistory(int companyId, string from, string to)
        {
            return await RequestHelper.HandleRequest(
                action: async () =>
                {
                    var result = await client.Request("integrations", "time_tracking", "activities", "daily-summary")
                        .WithOAuthBearerToken(tokenService.Get())
                        .SetQueryParam("started_at", from, true)
                        .SetQueryParam("finished_at", to, true)
                        .SetQueryParam("selected_company_id", companyId)
                        .GetJsonAsync<JSONDataDto<List<TrackHistory>>>();

                    var resultList = result.data;
                    resultList.Sort((a, b) => a.date.CompareTo(b.date));
                    return resultList;
                },
                unexpectedError: e => new List<TrackHistory>()
            );
        }

        public async Task<TimeSummary> GetTimeSummary(int companyId)
        {
            return await RequestHelper.HandleRequest(
                action: async () =>
                {
                    var result = await client.Request("time-tracker", "time-summary")
                        .WithOAuthBearerToken(tokenService.Get())
                        .GetJsonAsync<JSONDataDto<TimeSummaryData>>();

                    var tickets = new Dictionary<int, long>();
                    foreach (var keyValuePair in result.data.tickets)
                    {
                        var split = keyValuePair.Key.Split(':');
                        var id = int.Parse(split[2]);
                        tickets[id] = keyValuePair.Value;
                    }

                    var projects = new Dictionary<int, long>();
                    foreach (var keyValuePair in result.data.projects)
                    {
                        var split = keyValuePair.Key.Split(':');
                        var id = int.Parse(split[1]);
                        projects[id] = keyValuePair.Value;
                    }

                    var companies = new Dictionary<int, long>();
                    foreach (var keyValuePair in result.data.companies)
                    {
                        var id = int.Parse(keyValuePair.Key);
                        companies[id] = keyValuePair.Value;
                    }

                    return new TimeSummary
                    {
                        Companies = companies,
                        Projects = projects,
                        Tickets = tickets
                    };
                },
                unexpectedError: e => new TimeSummary
                {
                    Companies = new Dictionary<int, long>(),
                    Projects = new Dictionary<int, long>(),
                    Tickets = new Dictionary<int, long>()
                });
        }

        public async Task<long> GetTodaySum(int companyId)
        {
            DateTime now = DateTime.Now;
            string max = new DateTime(now.Year, now.Month, now.Day, 22, 59, 59, DateTimeKind.Utc).ToString("yyyy-MM-dd+HH:mm:ss");
            string min = new DateTime(now.Year, now.Month, now.Day, 23, 00, 00, DateTimeKind.Utc)
                .AddDays(-1)
                .ToString("yyyy-MM-dd+HH:mm:ss");
            return await RequestHelper.HandleRequest(
                action: async () =>
                {
                    //todo user_id 
                    var result = await client.Request("integrations", "time_tracking", "activities", "summary")
                        .WithOAuthBearerToken(tokenService.Get())
                        .SetQueryParam("max_utc_started_at", max, true)
                        .SetQueryParam("min_utc_started_at", min, true)
                        .SetQueryParam("user_id",  Preferences.Get("current_user_id", ""))
                        .SetQueryParam("selected_company_id", companyId)
                        .GetJsonAsync<JSONDataDto<TodaySum>>();

                    return result.data.sum_time ?? 0;
                },
                unexpectedError: e => throw e
            );
        }
    }
}