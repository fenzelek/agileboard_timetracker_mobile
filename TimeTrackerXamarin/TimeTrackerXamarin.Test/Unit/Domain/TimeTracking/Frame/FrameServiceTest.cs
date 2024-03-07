using System;
using System.Collections.Generic;
using Moq;
using TimeTrackerXamarin._Domains.TimeTracking.Frame;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking.Frame;
using Xunit;

namespace TimeTrackerXamarin.Test.Unit.Domain.TimeTracking.Frame
{
    public class FrameServiceTest
    {
        private readonly FrameService service;

        private readonly Mock<IFrameRepository> repository;

        public FrameServiceTest()
        {
            repository = new Mock<IFrameRepository>();
            service = new FrameService(repository.Object);
        }

        /*
         * @feature TimeTracking
         * @scenario Get unfinished frame from service
         * @case Gets unfinished frame
         */
        [Fact]
        public async void GetUnfinishedFrame()
        {
            // GIVEN

            // WHEN
            await service.GetUnfinishedFrame();

            // THEN
            repository.Verify(mock => mock.GetUnfinishedFrame(), Times.Once);
        }

        /*
         * @feature TimeTracking
         * @scenario Save frame from service
         * @case Saves frame
         */
        [Fact]
        public async void SaveFrame()
        {
            // GIVEN
            var frame = new TimeFrame
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
            };

            // WHEN
            await service.SaveFrame(frame);

            // THEN
            repository.Verify(mock => mock.SaveFrame(frame), Times.Once);
        }

        /*
         * @feature TimeTracking
         * @scenario Get saved frames
         * @case Gets unfinished frames   
         */
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void GetSaveFrames(bool all)
        {
            // GIVEN

            // WHEN
            await service.GetSavedFrames(all);

            // THEN
            repository.Verify(mock => mock.GetSavedFrames(all), Times.Once);
        }

        /*
         * @feature TimeTracking
         * @scenario Remove saved frame
         * @case Removes saved frame   
         */
        [Fact]
        public async void RemoveSavedFrame()
        {
            // GIVEN
            var frameId = 1;

            // WHEN
            await service.RemoveSavedFrame(frameId);

            // THEN
            repository.Verify(mock => mock.RemoveSavedFrame(frameId), Times.Once);
        }

        /*
         * @feature TimeTracking
         * @scenario Update frame
         * @case Updates frame   
         */
        [Fact]
        public async void UpdateFrame()
        {
            // GIVEN
            var frame = new TimeFrame
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
            };

            // WHEN
            await service.UpdateFrame(frame);

            // THEN
            repository.Verify(mock => mock.UpdateFrame(frame), Times.Once);
        }

        /*
         * @feature TimeTracking
         * @scenario Update sent frame
         * @case Makes frame sent 
         */
        [Fact]
        public async void UpdateSentFrame()
        {
            // GIVEN
            var frameId = 1;

            // WHEN
            await service.UpdateSentFrame(frameId);

            // THEN
            repository.Verify(mock => mock.UpdateSentFrame(frameId), Times.Once);
        }
        
        /*
         * @feature TimeTracking
         * @scenario Send frames
         * @case Sends all frames to remote
         */
        [Fact]
        public async void SaveFrames()
        {
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

            // WHEN
            await service.SendFrames(frames);

            // THEN
            repository.Verify(mock => mock.SendFrames(frames), Times.Once);
        }
    }
}