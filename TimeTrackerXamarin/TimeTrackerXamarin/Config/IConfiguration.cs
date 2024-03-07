using System.Threading.Tasks;

namespace TimeTrackerXamarin.Config
{
    public interface IConfiguration
    {
        string ApiUrl { get; }
        int FrameLength { get; }
        int FrameTimeout { get; }
        int InactivityNotificationTime { get; }
        bool IsDebug { get; }
        
        void Load();
    }
}