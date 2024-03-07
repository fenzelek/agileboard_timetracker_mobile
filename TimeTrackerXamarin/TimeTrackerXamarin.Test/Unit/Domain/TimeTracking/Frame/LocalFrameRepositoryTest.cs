using System;
using System.Collections.Generic;
using Moq;
using TimeTrackerXamarin._Domains.TimeTracking.Frame;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking.Frame;
using Xunit;

namespace TimeTrackerXamarin.Test.Unit.Domain.TimeTracking.Frame
{
    public class LocalFrameRepositoryTest
    {

        private readonly LocalFrameRepository repository;
        private readonly Mock<ILocalFrameSource> source;
        
        public LocalFrameRepositoryTest()
        {
            source = new Mock<ILocalFrameSource>();
            repository = new LocalFrameRepository(source.Object);
        }

        /*
         * @feature TimeTracking
         * @scenario Get unfinished frame
         * @case Gets unfinished frame from local source
         */
        [Fact]
        public async void GetUnfinishedFrame()
        {
            // GIVEN

            // WHEN
            await repository.GetUnfinishedFrame();

            // THEN
            source.Verify(mock => mock.GetUnfinishedFrame(), Times.Once);
        }

        /*
         * @feature TimeTracking
         * @scenario Save frame
         * @case Saves frame to local source
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
            await repository.SaveFrame(frame);

            // THEN
            source.Verify(mock => mock.SaveFrame(frame), Times.Once);
        }

        /*
         * @feature TimeTracking
         * @scenario Get saved frames
         * @case Gets unfinished frames from local source
         */
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void GetSaveFrames(bool all)
        {
            // GIVEN

            // WHEN
            await repository.GetSavedFrames(all);

            // THEN
            source.Verify(mock => mock.GetSavedFrames(all), Times.Once);
        }

        /*
         * @feature TimeTracking
         * @scenario Remove saved frame
         * @case Removes saved frame from local source  
         */
        [Fact]
        public async void RemoveSavedFrame()
        {
            // GIVEN
            var frameId = 1;

            // WHEN
            await repository.RemoveSavedFrame(frameId);

            // THEN
            source.Verify(mock => mock.RemoveSavedFrame(frameId), Times.Once);
        }

        /*
         * @feature TimeTracking
         * @scenario Update frame
         * @case Updates frame in local source  
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
            await repository.UpdateFrame(frame);

            // THEN
            source.Verify(mock => mock.UpdateFrame(frame), Times.Once);
        }

        /*
         * @feature TimeTracking
         * @scenario Update sent frame
         * @case Makes frame sent in local source
         */
        [Fact]
        public async void UpdateSentFrame()
        {
            // GIVEN
            var frameId = 1;

            // WHEN
            await repository.UpdateSentFrame(frameId);

            // THEN
            source.Verify(mock => mock.UpdateSentFrame(frameId), Times.Once);
        }
        
        /*
         * @feature TimeTracking
         * @scenario Send frames
         * @case Throws exception, it is local repository
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

            // WHEN / THEN
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await repository.SendFrames(frames);
            });
        }
    }
}