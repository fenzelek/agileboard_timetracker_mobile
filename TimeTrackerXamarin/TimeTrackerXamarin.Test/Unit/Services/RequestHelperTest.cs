using System;
using Flurl.Http;
using TimeTrackerXamarin._Domains.API;
using Xunit;

namespace TimeTrackerXamarin.Test.Unit.Services
{
    public class RequestHelperTest
    {

        /**
         * @feature Services
         * @scenario Ssl handshake error occurs
         * @case Request retried 3 times, then exception is thrown
         */
        [Fact]
        async void RequestHelperTest_SllError()
        {
            //GIVEN
            var counter = 0;
            var sslException = new FlurlHttpException(new FlurlCall(), "SSL handshake aborted: ssl=0xc6d226c8: I/O error during system call, Connection reset by peer", new Exception());

            //WHEN / THEN
            await Assert.ThrowsAsync<FlurlHttpException>(async () =>
            {
                await RequestHelper.HandleRequest(() =>
                {
                    counter++;
                    throw sslException;
                }, e => throw e);
            });
            Assert.Equal(3, counter);
        }
        
    }
}