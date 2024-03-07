using System;
using System.Threading.Tasks;
using Flurl.Http;
using Newtonsoft.Json;
using TimeTrackerXamarin._Domains.API.dto;
using TimeTrackerXamarin._UseCases.Contracts;

namespace TimeTrackerXamarin._Domains.API
{
    public static class RequestHelper
    {
        public static async Task<T> HandleRequest<T>(Func<Task<T>> action, Func<Exception, T> unexpectedError)
        {
            var sslErrorOccured = 0;
            while (true)
            {
                try
                {
                    return await action();
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("SSL handshake aborted"))
                    {
                        sslErrorOccured++;
                        if (sslErrorOccured >= 3)
                        {
                            return unexpectedError(e);
                        }

                        continue;
                    }

                    if (!(e is FlurlHttpException ex))
                    {
                        return unexpectedError(e);
                    }

                    if (ex.StatusCode == null)
                    {
                        return unexpectedError(e);
                    }

                    if (ex.InnerException is JsonSerializationException)
                    {
                        return unexpectedError(e);
                    }

                    var error = ex.GetResponseJsonAsync<JSONErrorDto>();
                    throw new ApiErrorException("api." + error.Result.code);
                }
            }
        }

        public static async Task HandleRequest(Func<Task> action, Action<Exception> unexpectedError)
        {
            var sslErrorOccured = 0;
            while (true)
            {
                try
                {
                    await action();
                    return;
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("SSL handshake aborted"))
                    {
                        sslErrorOccured++;
                        if (sslErrorOccured >= 3)
                        {
                            unexpectedError(e);
                            return;
                        }

                        continue;
                    }

                    if (!(e is FlurlHttpException ex))
                    {
                        unexpectedError(e);
                        return;
                    }

                    if (ex.StatusCode == null)
                    {
                        unexpectedError(e);
                        return;
                    }

                    if (ex.InnerException is JsonSerializationException)
                    {
                        unexpectedError(e);
                        return;
                    }

                    var error = ex.GetResponseJsonAsync<JSONErrorDto>();
                    throw new ApiErrorException("api." + error.Result.code);
                }
            }
        }
    }
}