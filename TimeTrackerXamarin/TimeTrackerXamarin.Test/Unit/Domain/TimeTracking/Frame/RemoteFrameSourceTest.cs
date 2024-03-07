using System;
using System.Collections.Generic;
using Flurl.Http;
using Flurl.Http.Testing;
using Moq;
using TimeTrackerXamarin._Domains.API.dto;
using TimeTrackerXamarin._Domains.TimeTracking.Dto;
using TimeTrackerXamarin._Domains.TimeTracking.Frame;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using Xunit;

namespace TimeTrackerXamarin.Test.Unit.Domain.TimeTracking.Frame
{
    public class RemoteFrameSourceTest
    {

        private readonly IFlurlClient client;
        private readonly Mock<ITokenService> tokenService;
        
        private readonly RemoteFrameSource source;
        private Mock<ILogger> logger;

        public RemoteFrameSourceTest()
        {
            client = new FlurlClient("http://api.api");
            tokenService = new Mock<ITokenService>();
            logger = new Mock<ILogger>();
            source = new RemoteFrameSource(client, tokenService.Object, logger.Object);
        }

        /*
         * @feature TimeTracking
         * @scenario Send frames
         * @case Sends frames to remote api
         */
        [Fact]
        public async void SendFrames()
        {
            using var httpTest = new HttpTest();
            // GIVEN
            var frames = new List<TimeFrame>
            {
                new TimeFrame
                {
                    companyId = 1,
                    projectId = 2,
                    taskId = 3,
                    from = 100,
                    to = 200,
                    activity = 100,
                    screens = Array.Empty<string>(),
                    gpsPositionDB = "",
                    sended = false
                }
            };

            var dto = new FramesPackageDto
            {
                frames = frames
            };

            httpTest.ForCallsTo("*time-tracker/add-frames*")
                .RespondWithJson(new JSONDataDto<FrameMessage>
                {
                    data = new FrameMessage
                    {
                        reject_frames = new List<TimeFrame>()
                    }
                });

            // WHEN
            await source.SendFrames(frames);
            
            // THEN
            httpTest
                .ShouldHaveCalled("*time-tracker/add-frames*")
                .WithRequestJson(dto)
                .Times(1);
        }
    }
}