using System;
using System.Net.Http;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;

namespace TimeTrackerXamarin._Domains.API
{
    public class FlurlClientFactory : FlurlClientFactoryBase
    {
        protected override IFlurlClient Create(Url url)
        {
            var cl = new HttpClient();
            cl.BaseAddress = url.ToUri();
            var client = new FlurlClient(cl)
                .WithHeader("Accept", "application/json")
                .WithTimeout(TimeSpan.FromSeconds(30));

            return client;
        }

        protected override string GetCacheKey(Url url)
        {
            return url.ToString();
        }
    }
}