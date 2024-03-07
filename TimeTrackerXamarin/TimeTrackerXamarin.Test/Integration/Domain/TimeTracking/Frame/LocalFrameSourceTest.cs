using Moq;
using SQLite;
using TimeTrackerXamarin._Domains.TimeTracking.Frame;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using Xunit;

namespace TimeTrackerXamarin.Test.Integration.Domain.TimeTracking.Frame
{
    public class LocalFrameSourceTest
    {
        
        private readonly SQLiteAsyncConnection localDatabase;
        private readonly LocalFrameSource source;
        private readonly Mock<IDatabaseConnector> connector;
        private readonly Mock<ILogger> logger;
        
        public LocalFrameSourceTest()
        {
            localDatabase = new SQLiteAsyncConnection(":memory:");
            connector = new Mock<IDatabaseConnector>();
            logger = new Mock<ILogger>();
            connector.Setup((db) => db.Create())
                .Returns(localDatabase);
            source = new LocalFrameSource(connector.Object, logger.Object);
            localDatabase.DropTableAsync<TimeFrame>().Wait();
            localDatabase.CreateTableAsync<TimeFrame>().Wait();
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
            var savedFrame = new TimeFrame
            {
                Id = 2,
                companyId = 1,
                projectId = 2,
                taskId = 3,
                from = 100,
                to = 0,
                activity = 100,
                gpsPositionDB = "1.222;2.333",
                sended = false
            };

            await localDatabase.InsertAsync(savedFrame);

            // WHEN
            var result = await source.RemoveSavedFrame(savedFrame.Id);

            // THEN
            Assert.True(result);
            Assert.Equal(0, await localDatabase.Table<TimeFrame>().CountAsync());
        }
        
        /*
         * @feature TimeTracking
         * @scenario Saves frame
         * @case Saves frame to local database    
         */
        [Fact]
        public async void SaveFrame()
        {
            // GIVEN
            var expectedFrame = new TimeFrame
            {
                companyId = 1,
                projectId = 2,
                taskId = 3,
                from = 100,
                to = 200,
                activity = 100,
                gpsPositionDB = "1.222;2.333",
                sended = true
            };
            
            // WHEN
            var result = await source.SaveFrame(expectedFrame);

            // THEN
            Assert.True(result);
            
            var actual = await localDatabase.Table<TimeFrame>().FirstOrDefaultAsync();
            AssertFramesEqual(expectedFrame, actual);
        }

        /*
         * @feature TimeTracking
         * @scenario Update frame
         * @case Update frame in local source  
         */
        [Fact]
        public async void UpdateFrame()
        {
            // GIVEN
            var expectedFrame = new TimeFrame
            {
                companyId = 1,
                projectId = 2,
                taskId = 3,
                from = 100,
                to = 200,
                activity = 100,
                gpsPositionDB = "1.222;2.333",
                sended = true
            };
            
            await localDatabase.InsertAsync(expectedFrame);

            expectedFrame.companyId = 2;
            expectedFrame.projectId = 3;
            expectedFrame.taskId = 4;
            expectedFrame.from = 250;
            expectedFrame.to = 300;
            expectedFrame.activity = 50;
            expectedFrame.gpsPositionDB = "2.111;3.222";
            expectedFrame.sended = false;

            // WHEN
            await source.UpdateFrame(expectedFrame);

            // THEN
            var actual = await localDatabase.Table<TimeFrame>().FirstOrDefaultAsync();
            AssertFramesEqual(expectedFrame, actual);
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
            var expectedFrame = new TimeFrame
            {
                companyId = 1,
                projectId = 2,
                taskId = 3,
                from = 100,
                to = 0,
                activity = 100,
                gpsPositionDB = "1.222;2.333",
                sended = false
            };

            await localDatabase.InsertAsync(expectedFrame);

            // WHEN
            var actual = await source.GetUnfinishedFrame();

            // THEN
            AssertFramesEqual(expectedFrame, actual);
        }
        
        /*
         * @feature TimeTracking
         * @scenario Get unfinished frame
         * @case Gets unfinished frame from local source when more than one in DB
         */
        [Fact]
        public async void GetUnfinishedFrame_TimeFrame()
        {
            // GIVEN
            var expectedFrame = new TimeFrame
            {
                companyId = 1,
                projectId = 2,
                taskId = 3,
                from = 100,
                to = 0,
                activity = 100,
                gpsPositionDB = "1.222;2.333",
                sended = false
            };
            var expectedCount = 1;

            await localDatabase.InsertAsync(expectedFrame);
            await localDatabase.InsertAsync(expectedFrame);

            // WHEN
            var actual = await source.GetUnfinishedFrame();
            var list = await localDatabase.Table<TimeFrame>().Where((x) => x.to == 0).ToListAsync();

            // THEN
            Assert.Equal(expectedCount,list.Count);
            AssertFramesEqual(expectedFrame, actual);
        }

        /*
         * @feature TimeTracking
         * @scenario Get unfinished frame
         * @case Gets unfinished frame from local source when its not there
         */
        [Fact]
        public async void GetUnfinishedFrame_null()
        {
            // GIVEN            
            
            // WHEN
            var actual = await source.GetUnfinishedFrame();            

            // THEN
            Assert.Equal(null, actual);
        }

        private void AssertFramesEqual(TimeFrame expected, TimeFrame actual)
        {
            Assert.Equal(expected.companyId, actual.companyId);
            Assert.Equal(expected.projectId, actual.projectId);
            Assert.Equal(expected.taskId, actual.taskId);
            Assert.Equal(expected.from, actual.from);
            Assert.Equal(expected.to, actual.to);
            Assert.Equal(expected.activity, actual.activity);
            Assert.Equal(expected.gpsPositionDB, actual.gpsPositionDB);
            Assert.Equal(expected.sended, actual.sended);
        }
    }
}