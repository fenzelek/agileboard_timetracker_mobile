using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using Xamarin.Forms;

namespace TimeTrackerXamarin._UseCases.TimeTracking
{
    public class GetUnfinishedFrame
    {
        private readonly IFactory<IFrameService> factory;
        private IFrameService service;
        
        public GetUnfinishedFrame(IFactory<IFrameService> factory)
        {
            this.factory = factory;
        }

        public void SetConnection(bool connection)
        {
            service = factory.Create(connection);
        }

        public Task<TimeFrame> Get()
        {
            return service.GetUnfinishedFrame();
        }
    }
}