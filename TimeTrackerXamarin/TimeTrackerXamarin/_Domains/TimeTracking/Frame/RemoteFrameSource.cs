using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using Newtonsoft.Json;
using TimeTrackerXamarin._Domains.API;
using TimeTrackerXamarin._Domains.API.dto;
using TimeTrackerXamarin._Domains.Projects.Tickets.Dto;
using TimeTrackerXamarin._Domains.TimeTracking.Dto;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking.Frame;

namespace TimeTrackerXamarin._Domains.TimeTracking.Frame
{
    public class RemoteFrameSource : IRemoteFrameSource
    {

        private readonly IFlurlClient client;
        private readonly ITokenService tokenService;
        private readonly ILogger logger;

        public RemoteFrameSource(IFlurlClient client, ITokenService tokenService, ILogger logger)
        {
            this.client = client;
            this.tokenService = tokenService;
            this.logger = logger;
        }

        public async Task SendFrames(List<TimeFrame> frames)
        {
            var dto = new FramesPackageDto { frames = frames };
            logger.Debug("Frames: " + JsonConvert.SerializeObject(frames));
            await RequestHelper.HandleRequest(
                action: async () =>
                {
                    var result = await client.Request("time-tracker", "add-frames")
                        .WithOAuthBearerToken(tokenService.Get())
                        .PostJsonAsync(dto)
                        .ReceiveJson<JSONDataDto<FrameMessage>>();

                    var data = result.data;
                    if (data.reject_frames != null && data.reject_frames.Count > 0)
                    {
                        throw new FrameRejectedException(data.reject_frames);
                    }
                },
                unexpectedError: e => throw e);
        }
    }
}