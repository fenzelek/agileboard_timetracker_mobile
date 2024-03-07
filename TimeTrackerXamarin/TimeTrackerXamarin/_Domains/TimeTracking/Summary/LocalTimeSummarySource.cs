using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeTrackerXamarin._Domains.TimeTracking.Frame;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking.Frame;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking.Summary;

namespace TimeTrackerXamarin._Domains.TimeTracking.Summary
{
    public class LocalTimeSummarySource : ILocalTimeSummarySource
    {
        private readonly ILocalFrameSource localFrameSource;

        public LocalTimeSummarySource(ILocalFrameSource localFrameSource)
        {
            this.localFrameSource = localFrameSource;
        }

        public async Task<List<TrackHistory>> GetTrackHistory(int companyId, string from, string to)
        {
            var fromDate = DateTimeOffset.Parse(from);
            var toDate = DateTimeOffset.Parse(to);
            
            var frames = await localFrameSource.GetSavedFrames(true);
            var actual = new List<TimeFrame>();
            foreach (var frame in frames)
            {
                var frameFrom = DateTimeOffset.FromUnixTimeSeconds(frame.from);
                var frameTo = DateTimeOffset.FromUnixTimeSeconds(frame.to);

                if (frameFrom < fromDate)
                {
                    if (frameTo > fromDate)
                    {
                        frame.from = fromDate.ToUnixTimeSeconds();
                        actual.Add(frame);
                    }
                    continue;
                }

                if (frameTo > toDate)
                {
                    if (frameFrom < toDate)
                    {
                        frame.to = toDate.ToUnixTimeSeconds();
                        actual.Add(frame);
                    }
                    continue;
                }
                
                actual.Add(frame);
            }
            
            var trackHistoryList = new List<TrackHistory>();
            foreach (var frame in actual)
            {
                var date = DateTimeOffset.FromUnixTimeSeconds(frame.to).Date;
                var history = trackHistoryList.FirstOrDefault(h => h.date.Equals(date));
                if (history == null)
                {
                    history = new TrackHistory
                    {
                        date = date,
                        tracked = frame.to - frame.from
                    };
                    trackHistoryList.Add(history);
                    continue;
                }

                history.tracked += frame.to - frame.from;
            }

            return trackHistoryList;
        }

        public async Task<TimeSummary> GetTimeSummary(int companyId)
        {
            var dbList = await localFrameSource.GetSavedFrames(true);
            var now = DateTimeOffset.Now;
            var today = new DateTimeOffset(new DateTime(now.Year, now.Month, now.Day, 0, 0, 1)).ToUnixTimeSeconds();
            var list = dbList.FindAll((x) => x.from >= today);

            var tickets = new Dictionary<int, long>();
            var projects = new Dictionary<int, long>();
            var companies = new Dictionary<int, long>();

            foreach (var item in list)
            {
                if (item.to < item.from) continue;

                var time = item.to - item.from;
                if (tickets.TryGetValue(item.taskId, out var ticket))
                {
                    tickets[item.taskId] = ticket + time;
                }
                else
                {
                    tickets[item.taskId] = time;
                }

                if (projects.TryGetValue(item.projectId, out var project))
                {
                    projects[item.projectId] = project + time;
                }
                else
                {
                    projects[item.projectId] = time;
                }

                if (companies.TryGetValue(item.companyId, out var company))
                {
                    companies[item.companyId] = company + time;
                }
                else
                {
                    companies[item.companyId] = time;
                }
            }

            var summary = new TimeSummary
            {
                Tickets = tickets,
                Companies = companies,
                Projects = projects
            };

            return summary;
        }

        public async Task<long> GetTaskSum(int taskId, bool addUnfinished)
        {
            var dblist = await localFrameSource.GetSavedFrames(true);
            if (dblist == null) return 0;
            var now = DateTimeOffset.Now;
            var today = new DateTimeOffset(new DateTime(now.Year, now.Month, now.Day, 0, 0, 1)).ToUnixTimeSeconds();
            var list = dblist.FindAll((x) => x.from >= today && x.taskId == taskId);
            long result = 0;
            foreach (TimeFrame item in list)
            {
                if (item.to < item.from) continue;
                result += item.to - item.from;
            }

            if (!addUnfinished)
                return result;

            var lastFrame = await localFrameSource.GetUnfinishedFrame();
            if (lastFrame == null)
                return result;
            
            if (lastFrame.taskId != taskId)
                return result;
            
            return result + (DateTimeOffset.Now.ToUnixTimeSeconds() - lastFrame.from);
        }

        public async Task<long> GetTodaySum(int companyId)
        {
            var dblist = await localFrameSource.GetSavedFrames(true);
            if (dblist == null) return 0;
            var now = DateTimeOffset.Now;
            var today = new DateTimeOffset(new DateTime(now.Year, now.Month, now.Day, 0, 0, 1)).ToUnixTimeSeconds();
            var list = dblist.FindAll((x) => x.from >= today);
            long result = 0;
            foreach (TimeFrame item in list)
            {
                if (item.to < item.from) continue;
                result += item.to - item.from;
            }

            return result;
        }
    }
}