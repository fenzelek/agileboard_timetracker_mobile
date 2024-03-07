using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using Xamarin.Forms;
using TimeTrackerXamarin.Config;
using Xamarin.Essentials.Interfaces;
using System.Net;
using Microsoft.AppCenter.Crashes;
using TimeTrackerXamarin._Domains.TimeTracking.Frame;
using TimeTrackerXamarin._UseCases.Contracts.Projects;
using TimeTrackerXamarin.i18n;

namespace TimeTrackerXamarin._Domains.TimeTracking
{
    public class TimeTrackingService : ITimeTracking
    {
        public int currentID = -1;
        public int currentProject;
        public int currentCompany;
        private long start;
        private readonly ITicketService localTicketService;
        private readonly IPushNotification pushNotification;
        private readonly ITranslationManager translation;
        private readonly ILocalization position;
        private readonly IMessagingCenter messagingCenter;
        private readonly IConnectivity connectivity;
        private readonly IAppCenterAnalytics analytics;
        private readonly IForegroundServiceController serviceController;
        private readonly IPreferences preferences;
        private readonly IFactory<ITimeSummaryService> timeSummaryServiceFactory;
        private readonly IFactory<IFrameService> frameServiceFactory;
        private readonly ILogger logger;
        private Ticket currentTicket;
        private long totalTime;
        private long taskTotalTime;
        private bool stopping = false;

        public TimeTrackingService(
            IPushNotification pushNotification, 
            ITranslationManager translation, 
            ILocalization position,
            IPreferences preferences,
            IMessagingCenter messagingCenter, 
            IConnectivity connectivity,
            IAppCenterAnalytics analytics,
            IForegroundServiceController serviceController,
            IFactory<ITimeSummaryService> timeSummaryServiceFactory,
            IFactory<ITicketService> ticketServiceFactory, 
            IFactory<IFrameService> frameServiceFactory, ILogger logger)
        {
            this.pushNotification = pushNotification;
            this.translation = translation;
            this.position = position;
            localTicketService = ticketServiceFactory.Create(false);
            this.messagingCenter = messagingCenter;
            this.connectivity = connectivity;
            this.analytics = analytics;
            this.serviceController = serviceController;
            this.preferences = preferences;
            this.timeSummaryServiceFactory = timeSummaryServiceFactory;
            this.frameServiceFactory = frameServiceFactory;
            this.logger = logger;
        }
        public async Task<bool> Start(int company_id, int project_id, int ticket_id)
        {
            messagingCenter.Send<App, BlockingState>((App)App.Current, "BlockingState", BlockingState.Never);
            analytics.TrackOperation("TimeTracking.Start");
            currentID = ticket_id;
            currentCompany = company_id;
            currentProject = project_id;
            totalTime = await GetTotalTime();
            taskTotalTime = await GetTaskTotal(currentID);
            start = DateTimeOffset.Now.ToUnixTimeSeconds();
            await GetFrameService().SaveFrame(CreateFrame(start, 0));
            currentTicket = await localTicketService.GetTicket(ticket_id);
            messagingCenter.Send<App>((App)App.Current, "ServiceStarted");
            logger.Info("Frame has been started.");
            return true;
        }

        public async Task<bool> ContinueLast(bool unblock = true)
        {
            var unfinished = await GetFrameService().GetUnfinishedFrame();
            if (unfinished == null)
            {
                return false;
            }
            
            currentID = unfinished.taskId;
            currentCompany = unfinished.companyId;
            currentProject = unfinished.projectId;
            totalTime = await GetTotalTime();
            taskTotalTime = await GetTaskTotal(currentID);
            currentTicket = await localTicketService.GetTicket(currentID);
            await serviceController.StartService(true);
            if (unblock)                
                messagingCenter.Send<App, BlockingState>((App)App.Current, "BlockingState", BlockingState.Never);
            else
            {
                //await Task.Delay(500);
                messagingCenter.Send<App, BlockingState>((App)App.Current, "BlockingState", BlockingState.Always);
            }
            return true;
        }

        public async Task<bool> Stop()
        {
            stopping = true;
            analytics.TrackOperation("TimeTracking.Stop");
            messagingCenter.Send<App>((App)App.Current, "ServiceStopped");
            currentID = -1;
            var frameService = GetFrameService();
            long end = DateTimeOffset.Now.ToUnixTimeSeconds();
            var currentFrame = await frameService.GetUnfinishedFrame();
            if (currentFrame == null)
            {
                stopping = false;
                return false;
            }
            currentFrame.to = end;
            currentFrame.activity = (int)currentFrame.to - (int)currentFrame.from;
            var posHelper = await position.Get();
            currentFrame.gpsPositionDB = position.Parse(posHelper);

            if (await frameService.UpdateFrame(currentFrame))
                logger.Info($"Frame {currentFrame.Id} has been updated.");
            else
                logger.Error($"Frame {currentFrame.Id} has not been updated.");
            
            Sync();
            
            logger.Info($"Frame {currentFrame.Id} has been stopped.");
            stopping = false;
            return true;
        }
        public async Task<bool> Sync()
        {
            if(connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                return false;
            }

            var frameService = GetFrameService();
            List<TimeFrame> list = frameService.GetSavedFrames(false).Result;
            List<TimeFrame> listToSend = new List<TimeFrame>();
            if (list.Count > 0)
            {
                foreach (TimeFrame frame in list)
                {
                    frame.screens = Array.Empty<string>();
                    if (!string.IsNullOrEmpty(frame.gpsPositionDB))
                    {
                        var gpsPositionTab = frame.gpsPositionDB.Split(';');
                        double.TryParse(gpsPositionTab[0], out var latitude);
                        double.TryParse(gpsPositionTab[1], out var longitude);
                        var gpsPositionDict = new Dictionary<string, double?>();
                        gpsPositionDict["latitude"] = latitude;
                        gpsPositionDict["longitude"] = longitude;
                        frame.gpsPosition = gpsPositionDict;
                    }

                    listToSend.Add(frame);
                    await frameService.UpdateSentFrame(frame.Id);
                }

                try
                {
                    await frameService.SendFrames(listToSend);
                }
                catch (FrameRejectedException e)
                {
                    //todo exclude to higher layer
                    
                    logger.Warn($"There were frames rejected. ({e.RejectedFrames.Count})");
                    var notificationContent = $"{translation.Translate("api.cancelledframe")}: \n";
                    var errorParams = new Dictionary<string, string>();
                    foreach (var frame in e.RejectedFrames)
                    {
                        errorParams.Add("frame", $"{frame.from} - {frame.to}");                                                                                   
                        notificationContent += $"{DateTimeOffset.FromUnixTimeSeconds(frame.from):dd-MM-yy HH:mm}({TimeSpan.FromSeconds(frame.to - frame.from)})\n";
                    }
                    Crashes.TrackError(e, errorParams);

                    var subtitle = "Last task";
                    
                    var rejected = e.RejectedFrames.FirstOrDefault();
                    if (rejected != null)
                    {
                        var details = await localTicketService.GetDetails(rejected.projectId, rejected.taskId, rejected.companyId);
                        subtitle = details.title;
                    }
                    await pushNotification.Create(1, subtitle, notificationContent);
                    return false;
                }
                logger.Info("Frames have been synced.");
                return true;
            }

            return false;
        }
        
        async Task<long> GetTotalTime()
        {
            var connection = connectivity.NetworkAccess == NetworkAccess.Internet;
            var timeSummaryService = timeSummaryServiceFactory.Create(connection);
            return await timeSummaryService.GetTodaySum(int.Parse(preferences.Get("current_company_id", "")));
        }

        async Task<long> GetTaskTotal(int id)
        {
            return await timeSummaryServiceFactory.Create(false).GetTaskSum(id, false);
        }

        IFrameService GetFrameService()
        {
            var connection = connectivity.NetworkAccess == NetworkAccess.Internet;
            var frameService = frameServiceFactory.Create(connection);
            return frameService;
        }

        public async Task EndFrame(long endTime, bool createFrame = true)
        {
            analytics.TrackOperation("TimeTracking.EndFrame");
            logger.Info($"Saving frame {start} -> {endTime}");
            start = endTime;
            var frameService = GetFrameService();
            TimeFrame frame = await frameService.GetUnfinishedFrame();
            if (frame == null)
            {
                return;
            }

            frame.to = endTime;
            frame.activity = (int)frame.to - (int)frame.from;
            frame.gpsPositionDB = position.Parse(await position.Get());
            currentID = frame.taskId;
            currentProject = frame.projectId;
            currentCompany = frame.companyId;

            await frameService.UpdateFrame(frame);
            if (createFrame)
            {
                await frameService.SaveFrame(CreateFrame(start, 0));
            }
            totalTime = await GetTotalTime() + frame.activity;
            taskTotalTime = await GetTaskTotal(currentID) ;
            Sync();
            
        }
        public TimeFrame CreateFrame(long start,long end)
        {
            TimeFrame frame = new TimeFrame()
            {
                companyId = currentCompany,
                projectId = currentProject,
                taskId = currentID,
                from = start,
                to = end,
                activity = 100,
                screens = Array.Empty<string>(),
                gpsPositionDB = "",
                sended = false
            };
            return frame;
        }

        public async Task<CurrentTrackingInfo> GetCurrentTracking()
        {
            if (stopping) return null;
            var frameService = GetFrameService();
            TimeFrame currentFrame = null;
            if (currentID == -1)
            {
                var unfinished = await frameService.GetUnfinishedFrame();
                if (unfinished == null)
                {
                    return null;
                }

                currentFrame = unfinished;
                currentID = unfinished.taskId;
                totalTime = await GetTotalTime();
                taskTotalTime = await GetTaskTotal(currentID);
            }

            if (currentFrame == null)
            {
                currentFrame = await frameService.GetUnfinishedFrame();
            }

            if (currentTicket == null)
            {
                currentTicket = await localTicketService.GetTicket(currentID);
            }
            return new CurrentTrackingInfo
            {
                Frame = currentFrame,
                TaskTime = taskTotalTime,
                TotalTime = totalTime,
                TaskTitle = currentTicket.name,
                TaskId = currentID
            };
        }
        
    }
}
