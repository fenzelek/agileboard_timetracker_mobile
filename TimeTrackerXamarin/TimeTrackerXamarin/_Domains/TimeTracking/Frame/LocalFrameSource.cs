using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking.Frame;

namespace TimeTrackerXamarin._Domains.TimeTracking.Frame
{
    public class LocalFrameSource : ILocalFrameSource
    {

        private readonly SQLiteAsyncConnection db;
        private readonly ILogger logger;

        public LocalFrameSource(IDatabaseConnector connector, ILogger logger)
        {
            this.logger = logger;
            db = connector.Create();
            
            db.CreateTableAsync<TimeFrame>().Wait();
        }

        public async Task<bool> SaveFrame(TimeFrame frame)
        {
            try
            {
                return await db.InsertAsync(frame) == 1;
            }
            catch(Exception err)
            {
                logger.Error("There was an error.", err);
                return false;
            }
        }

        public async Task<TimeFrame> GetUnfinishedFrame()
        {
            var list = await db.Table<TimeFrame>().Where(frame => frame.to == 0).ToListAsync();
            if (list.Count((x) => x.from == list[0].from) > 1)
            {
                foreach (var frame in list.Skip(1))
                {
                    await RemoveSavedFrame(frame.Id);
                }
            }
            return list.FirstOrDefault();
        }

        public Task<List<TimeFrame>> GetSavedFrames(bool all)
        {
            if (all)
                return db.Table<TimeFrame>().Where((frame) => frame.to != 0).ToListAsync();
            else
                return db.Table<TimeFrame>().Where((frame) => frame.to != 0 && frame.sended != true).ToListAsync();
        }

        public async Task<bool> RemoveSavedFrame(int frameId)
        {
            return await db.DeleteAsync<TimeFrame>(frameId) == 1;
        }

        public async Task<bool> UpdateFrame(TimeFrame frame)
        {
            return await db.UpdateAsync(frame) == 1;
        }

        public async Task<bool> UpdateSentFrame(int frameId)
        {
            var frameToUpdate = await db.Table<TimeFrame>().Where(frame => frame.Id == frameId).FirstOrDefaultAsync();
            if (frameToUpdate == null) return false;
            frameToUpdate.sended = true; 
            var rowsUpdated = await db.UpdateAsync(frameToUpdate).ConfigureAwait(false);
            return rowsUpdated == 1;
        }
    }
}