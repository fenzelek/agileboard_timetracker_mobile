using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts;

namespace TimeTrackerXamarin._UseCases
{
    public class FlushDatabase
    {

        private readonly IDatabaseFlushService flushService;

        public FlushDatabase(IDatabaseFlushService flushService)
        {
            this.flushService = flushService;
        }

        public Task Flush()
        {
            return flushService.Flush();
        }
    }
}