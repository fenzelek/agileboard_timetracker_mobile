using System;
using System.Collections.Generic;
using Moq;
using TimeTrackerXamarin._Domains.TimeTracking.Frame;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking.Frame;
using Xunit;

namespace TimeTrackerXamarin.Test.Unit.Domain.TimeTracking.Frame
{
    public class RemoteFrameRepositoryTest
    {

        private readonly RemoteFrameRepository repository;
        private readonly Mock<ILocalFrameSource> localSource;
        private readonly Mock<IRemoteFrameSource> remoteSource;
        
        public RemoteFrameRepositoryTest()
        {
            localSource = new Mock<ILocalFrameSource>();
            remoteSource = new Mock<IRemoteFrameSource>();
            
            repository = new RemoteFrameRepository(remoteSource.Object, localSource.Object);
        }

        /*
         * @feature TimeTracking
         * @scenario Get unfinished frame
         * @case Gets unfinished frame from remote repository
         */
        [Fact]
        public async void GetUnfinishedFrame()
        {
            // GIVEN

            // WHEN
            await repository.GetUnfinishedFrame();

            // THEN
            localSource.Verify(mock => mock.GetUnfinishedFrame(), Times.Once);
        }

        /*
         * @feature TimeTracking
         * @scenario Save frame
         * @case Saves frame to remote repository
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
            localSource.Verify(mock => mock.SaveFrame(frame), Times.Once);
        }

        /*
         * @feature TimeTracking
         * @scenario Get saved frames
         * @case Gets unfinished frames from remote repository   
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
            localSource.Verify(mock => mock.GetSavedFrames(all), Times.Once);
        }

        /*
         * @feature TimeTracking
         * @scenario Remove saved frame
         * @case Removes saved frame from remote repository   
         */
        [Fact]
        public async void RemoveSavedFrame()
        {
            // GIVEN
            var frameId = 1;

            // WHEN
            await repository.RemoveSavedFrame(frameId);

            // THEN
            localSource.Verify(mock => mock.RemoveSavedFrame(frameId), Times.Once);
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
            localSource.Verify(mock => mock.UpdateFrame(frame), Times.Once);
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
            localSource.Verify(mock => mock.UpdateSentFrame(frameId), Times.Once);
        }
        
        /*
         * @feature TimeTracking
         * @scenario Send frames
         * @case Sends frames to remote data source
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
            await repository.SendFrames(frames);

            // THEN
            remoteSource.Verify(mock => mock.SendFrames(frames));
        }
    }
}