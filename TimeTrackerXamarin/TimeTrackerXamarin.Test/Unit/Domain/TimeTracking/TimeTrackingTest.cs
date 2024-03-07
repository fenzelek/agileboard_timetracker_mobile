using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using TimeTrackerXamarin._Domains.TimeTracking;
using TimeTrackerXamarin._Domains.TimeTracking.Frame;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Projects;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin.i18n;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;
using Xunit;

namespace TimeTrackerXamarin.Test.Unit.Domain.TimeTracking
{
    public class TimeTrackingTest
    {
        private readonly Mock<IPushNotification> pushNotification;
        private readonly Mock<ITranslationManager> translationManager;
        private readonly Mock<ILocalization> localization;
        private readonly Mock<IPreferences> preferences;
        private readonly Mock<IMessagingCenter> messagingCenter;
        private readonly Mock<IConnectivity> connectivity;
        private readonly Mock<IAppCenterAnalytics> appCenterAnalytics;

        private readonly Mock<ITimeSummaryService> timeSummaryService;
        private readonly Mock<IFactory<ITimeSummaryService>> timeSummaryServiceFactory;

        private readonly Mock<ITicketService> ticketService;
        private readonly Mock<IFactory<ITicketService>> ticketServiceFactory;

        private readonly Mock<IFrameService> frameService;
        private readonly Mock<IFactory<IFrameService>> frameServiceFactory;

        private readonly Mock<IForegroundServiceController> foregroundServiceController;
        
        private readonly TimeTrackingService service;
        private Mock<ILogger> logger;

        public TimeTrackingTest()
        {
            pushNotification = new Mock<IPushNotification>();
            translationManager = new Mock<ITranslationManager>();
            localization = new Mock<ILocalization>();
            preferences = new Mock<IPreferences>();
            messagingCenter = new Mock<IMessagingCenter>();
            connectivity = new Mock<IConnectivity>();
            appCenterAnalytics = new Mock<IAppCenterAnalytics>();
            logger = new Mock<ILogger>();
            

            timeSummaryService = new Mock<ITimeSummaryService>();
            timeSummaryServiceFactory = new Mock<IFactory<ITimeSummaryService>>();
            timeSummaryServiceFactory.Setup(mock => mock.Create(It.IsAny<bool>()))
                .Returns(timeSummaryService.Object);

            ticketService = new Mock<ITicketService>();
            ticketServiceFactory = new Mock<IFactory<ITicketService>>();
            ticketServiceFactory.Setup(mock => mock.Create(It.IsAny<bool>()))
                .Returns(ticketService.Object);

            frameService = new Mock<IFrameService>();
            frameServiceFactory = new Mock<IFactory<IFrameService>>();
            frameServiceFactory.Setup(mock => mock.Create(It.IsAny<bool>()))
                .Returns(frameService.Object);

            foregroundServiceController = new Mock<IForegroundServiceController>();

            service = new TimeTrackingService(
                pushNotification.Object, translationManager.Object, localization.Object, preferences.Object,
                messagingCenter.Object, connectivity.Object, appCenterAnalytics.Object,
                foregroundServiceController.Object, timeSummaryServiceFactory.Object,
                ticketServiceFactory.Object, frameServiceFactory.Object, logger.Object
            );
        }

        /*
         * @feature TimeTracking
         * @scenario Start tracking
         * @case Tracking starts successfully
         */
        [Fact]
        public async void StartTracking()
        {
            // GIVEN
            var companyId = 1;
            var projectId = 2;
            var ticketId = 3;

            preferences.Setup(mock => mock.Get("current_company_id", It.IsAny<string>()))
                .Returns(companyId.ToString());

            // WHEN
            var result = await service.Start(companyId, projectId, ticketId);

            // THEN
            Assert.True(result);
            Assert.Equal(companyId, service.currentCompany);
            Assert.Equal(projectId, service.currentProject);
            Assert.Equal(ticketId, service.currentID);

            messagingCenter.Verify(mock =>
                mock.Send(It.IsAny<App>(), "BlockingState", BlockingState.Never), Times.Once);
            messagingCenter.Verify(mock =>
                mock.Send(It.IsAny<App>(), "ServiceStarted"), Times.Once);

            timeSummaryService.Verify(mock => mock.GetTodaySum(companyId), Times.Once);
            timeSummaryService.Verify(mock => mock.GetTaskSum(ticketId, false), Times.Once);
            frameService.Verify(mock => mock.SaveFrame(It.IsAny<TimeFrame>()), Times.Once);
            ticketService.Verify(mock => mock.GetTicket(ticketId), Times.Once);
        }

        /*
         * @feature TimeTracking
         * @scenario Continue tracking
         * @case Tracking continues, unfinished frame exists
         */
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void ContinueTracking_Unfinished(bool unblock)
        {
            // GIVEN
            var companyId = 1;
            var projectId = 2;
            var ticketId = 3;
            var blockingState = unblock ? BlockingState.Never : BlockingState.Always;
            var unfinished = new TimeFrame
            {
                companyId = companyId,
                projectId = projectId,
                taskId = ticketId,
                from = DateTimeOffset.Now.ToUnixTimeSeconds(),
                to = 0,
                activity = 100,
                screens = Array.Empty<string>(),
                gpsPositionDB = "",
                sended = false
            };


            preferences.Setup(mock => mock.Get("current_company_id", It.IsAny<string>()))
                .Returns(companyId.ToString());
            frameService.Setup(mock => mock.GetUnfinishedFrame())
                .ReturnsAsync(unfinished);

            // WHEN
            var result = await service.ContinueLast(unblock);

            // THEN
            Assert.True(result);
            Assert.Equal(companyId, service.currentCompany);
            Assert.Equal(projectId, service.currentProject);
            Assert.Equal(ticketId, service.currentID);

            messagingCenter.Verify(mock =>
                mock.Send(It.IsAny<App>(), "BlockingState", blockingState), Times.Once);
            foregroundServiceController.Verify(mock =>
                mock.StartService(true), Times.Once);

            timeSummaryService.Verify(mock => mock.GetTodaySum(companyId), Times.Once);
            timeSummaryService.Verify(mock => mock.GetTaskSum(ticketId, false), Times.Once);
            ticketService.Verify(mock => mock.GetTicket(ticketId), Times.Once);
        }

        /*
         * @feature TimeTracking
         * @scenario Continue tracking
         * @case Tracking does not continue, unfinished frame does not exist
         */
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void ContinueTracking_NoUnfinished(bool unblock)
        {
            // GIVEN

            // WHEN
            var result = await service.ContinueLast(unblock);

            // THEN
            Assert.False(result);
        }

        /*
         * @feature TimeTracking
         * @scenario Stop tracking
         * @case Tracking stops, unfinished frame exists
         */
        [Fact]
        public async void StopTracking_Started()
        {
            // GIVEN
            var companyId = 1;
            var projectId = 2;
            var ticketId = 3;
            var unfinished = new TimeFrame
            {
                companyId = companyId,
                projectId = projectId,
                taskId = ticketId,
                from = DateTimeOffset.Now.ToUnixTimeSeconds(),
                to = 0,
                activity = 100,
                screens = Array.Empty<string>(),
                gpsPositionDB = "",
                sended = false
            };

            frameService.Setup(mock => mock.GetUnfinishedFrame())
                .ReturnsAsync(unfinished);

            // WHEN
            var result = await service.Stop();

            //THEN
            Assert.True(result);
            Assert.Equal(-1, service.currentID);

            messagingCenter.Verify(mock =>
                mock.Send(It.IsAny<App>(), "ServiceStopped"), Times.Once);
            frameService.Verify(mock => mock.GetUnfinishedFrame(), Times.Once);
            frameService.Verify(mock => mock.UpdateFrame(It.IsAny<TimeFrame>()), Times.Once);
            localization.Verify(mock => mock.Get(), Times.Once);
        }

        /*
         * @feature TimeTracking
         * @scenario Stop tracking
         * @case Tracking stops, unfinished frame does not exist
         */
        [Fact]
        public async void StopTracking_NotStarted()
        {
            // GIVEN

            // WHEN
            var result = await service.Stop();

            //THEN
            Assert.False(result);
            Assert.Equal(-1, service.currentID);

            messagingCenter.Verify(mock =>
                mock.Send(It.IsAny<App>(), "ServiceStopped"), Times.Once);
            frameService.Verify(mock => mock.GetUnfinishedFrame(), Times.Once);
        }

        /*
         * @feature TimeTracking
         * @scenario Get current tracking
         * @case Returns correct current tracking after start
         */
        [Fact]
        public async void GetCurrentTracking_Started()
        {
            // GIVEN
            var companyId = 1;
            var projectId = 2;
            var ticketId = 3;
            var taskTime = 10L;
            var totalTime = 20L;
            var taskName = "title";
            var unfinished = new TimeFrame
            {
                companyId = companyId,
                projectId = projectId,
                taskId = ticketId,
                from = DateTimeOffset.Now.ToUnixTimeSeconds(),
                to = 0,
                activity = 100,
                screens = Array.Empty<string>(),
                gpsPositionDB = "",
                sended = false
            };

            timeSummaryService.Setup(mock => mock.GetTaskSum(ticketId, false))
                .ReturnsAsync(taskTime);
            timeSummaryService.Setup(mock => mock.GetTodaySum(companyId))
                .ReturnsAsync(totalTime);
            ticketService.Setup(mock => mock.GetTicket(ticketId))
                .ReturnsAsync(new Ticket
                {
                    name = taskName
                });
            frameService.Setup(mock => mock.GetUnfinishedFrame())
                .ReturnsAsync(unfinished);

            preferences.Setup(mock => mock.Get("current_company_id", It.IsAny<string>()))
                .Returns(companyId.ToString());

            // WHEN
            await service.Start(companyId, projectId, ticketId);
            var result = await service.GetCurrentTracking();

            // THEN
            Assert.NotNull(result);
            Assert.Equal(ticketId, result.TaskId);
            Assert.Equal(taskTime, result.TaskTime);
            Assert.Equal(totalTime, result.TotalTime);
            Assert.Equal(taskName, result.TaskTitle);
            Assert.Equal(unfinished, result.Frame);
        }

        /*
         * @feature TimeTracking
         * @scenario Get current tracking
         * @case Returns null, no tracking started
         */
        [Fact]
        public async void GetCurrentTracking_NotStarted()
        {
            // GIVEN
            frameService.Setup(mock => mock.GetUnfinishedFrame())
                .Returns(Task.FromResult<TimeFrame>(null));

            // WHEN
            var result = await service.GetCurrentTracking();

            // THEN
            Assert.Null(result);
        }

        /*
         * @feature TimeTracking
         * @scenario Create frame
         * @case Creates correct frame after tracking started    
         */
        [Fact]
        public async void CreateFrame()
        {
            // GIVEN
            var companyId = 1;
            var projectId = 2;
            var ticketId = 3;
            var start = 10000;
            var end = start + 100;

            preferences.Setup(mock => mock.Get("current_company_id", It.IsAny<string>()))
                .Returns(companyId.ToString());

            // WHEN
            await service.Start(companyId, projectId, ticketId);
            var result = service.CreateFrame(start, end);

            //THEN
            Assert.NotNull(result);
            Assert.Equal(companyId, result.companyId);
            Assert.Equal(projectId, result.projectId);
            Assert.Equal(ticketId, result.taskId);
            Assert.Equal(start, result.from);
            Assert.Equal(end, result.to);
            Assert.Equal(100, result.activity);
            Assert.NotNull(result.screens);
            Assert.Empty(result.screens);
            Assert.Equal("", result.gpsPositionDB);
            Assert.False(result.sended);
        }

        /*
         * @feature TimeTracking
         * @scenario Sync frames
         * @case Returns false, no internet    
         */
        [Fact]
        public async void Sync_NoInternet()
        {
            // GIVEN
            connectivity.SetupGet(mock => mock.NetworkAccess)
                .Returns(NetworkAccess.None);

            // WHEN
            var result = await service.Sync();

            // THEN
            Assert.False(result);
        }

        /*
         * @feature TimeTracking
         * @scenario Sync frames
         * @case Returns true, internet, all frames sent
         */
        [Fact]
        public async void Sync_Internet_Success()
        {
            // GIVEN
            var savedFrames = new List<TimeFrame>
            {
                new TimeFrame
                {
                    Id = 1,
                    companyId = 2,
                    projectId = 3,
                    taskId = 4,
                    from = 1000,
                    to = 1100,
                    activity = 100,
                    screens = Array.Empty<string>(),
                    gpsPositionDB = "",
                    sended = false
                },
                new TimeFrame
                {
                    Id = 2,
                    companyId = 3,
                    projectId = 4,
                    taskId = 5,
                    from = 1200,
                    to = 1300,
                    activity = 200,
                    screens = Array.Empty<string>(),
                    gpsPositionDB = "",
                    sended = false
                }
            };
            frameService.Setup(mock => mock.GetSavedFrames(false))
                .ReturnsAsync(savedFrames);
            
            connectivity.SetupGet(mock => mock.NetworkAccess)
                .Returns(NetworkAccess.Internet);

            // WHEN
            var result = await service.Sync();

            // THEN
            Assert.True(result);
            frameService.Verify(mock => mock.GetSavedFrames(false), Times.Once);
            savedFrames.ForEach(frame =>
            {
                frameService.Verify(mock => mock.UpdateSentFrame(frame.Id), Times.Once);
            });
            frameService.Verify(mock => mock.SendFrames(savedFrames), Times.Once);
        }


        /*
         * @feature TimeTracking
         * @scenario Sync frames
         * @case Returns false, internet, frames rejected
         */
        [Fact]
        public async void Sync_Internet_FramesRejected()
        {
            // GIVEN
            var savedFrames = new List<TimeFrame>
            {
                new TimeFrame
                {
                    Id = 1,
                    companyId = 2,
                    projectId = 3,
                    taskId = 4,
                    from = 1000,
                    to = 1100,
                    activity = 100,
                    screens = Array.Empty<string>(),
                    gpsPositionDB = "",
                    sended = false
                }
            };
            frameService.Setup(mock => mock.GetSavedFrames(false))
                .ReturnsAsync(savedFrames);
            frameService.Setup(mock => mock.SendFrames(savedFrames))
                .Throws(new FrameRejectedException(savedFrames));
            
            connectivity.SetupGet(mock => mock.NetworkAccess)
                .Returns(NetworkAccess.Internet);

            // WHEN
            var result = await service.Sync();

            // THEN
            Assert.False(result);
            frameService.Verify(mock => mock.GetSavedFrames(false), Times.Once);
            savedFrames.ForEach(frame =>
            {
                frameService.Verify(mock => mock.UpdateSentFrame(frame.Id), Times.Once);
            });
            frameService.Verify(mock => mock.SendFrames(savedFrames), Times.Once);
            pushNotification.Verify(mock => mock.Create(1, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        /*
         * @feature TimeTracking
         * @scenario End frame
         * @case Ends current frame, unfinished frame exists    
         */
        [Fact]
        public async void EndFrame_Unfinished()
        {
            // GIVEN
            var companyId = 1;
            var startTime = 1000;
            var endTime = 1100;
            var position = new Location(10.1111, 12.2222);
            var unfinished = new TimeFrame
            {
                companyId = 1,
                projectId = 2,
                taskId = 3,
                from = startTime,
                to = endTime,
                activity = 100,
                screens = Array.Empty<string>(),
                gpsPositionDB = "",
                sended = false
            };
            frameService.Setup(mock => mock.GetUnfinishedFrame())
                .ReturnsAsync(unfinished);
            preferences.Setup(mock => mock.Get("current_company_id", It.IsAny<string>()))
                .Returns(companyId.ToString());
            localization.Setup(mock => mock.Get().Result)
                .Returns(position);

            // WHEN
            await service.EndFrame(endTime);

            // THEN
            frameService.Verify(mock => mock.GetUnfinishedFrame(), Times.Once);
            frameService.Verify(mock => mock.UpdateFrame(It.Is<TimeFrame>(frame => 
                frame.companyId == unfinished.companyId &&
                frame.projectId == unfinished.projectId &&
                frame.taskId == unfinished.taskId &&
                frame.from == startTime &&
                frame.to == endTime &&
                frame.activity == (endTime - startTime) &&
                frame.screens.Length == 0
            )), Times.Once);
            timeSummaryService.Verify(mock => mock.GetTodaySum(companyId), Times.Once);
            timeSummaryService.Verify(mock => mock.GetTaskSum(unfinished.taskId, false), Times.Once);
        } 
        
        /*
         * @feature TimeTracking
         * @scenario End frame
         * @case Does not end current frame, unfinished frame does not exist    
         */
        [Fact]
        public async void EndFrame_NotUnfinished()
        {
            // GIVEN

            // WHEN
            await service.EndFrame(1000);

            // THEN
            frameService.Verify(mock => mock.GetUnfinishedFrame(), Times.Once);
            frameService.Verify(mock => mock.UpdateFrame(It.IsAny<TimeFrame>()), Times.Never);
        }
    }
}