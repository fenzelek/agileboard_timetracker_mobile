using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;

namespace TimeTrackerXamarin._UseCases.TimeTracking
{
    public class RemoveFrame
    {
        private readonly IFactory<IFrameService> factory;
        private IFrameService service;
        
        public RemoveFrame(IFactory<IFrameService> factory)
        {
            this.factory = factory;
        }

        public void SetConnection(bool connection)
        {
            service = factory.Create(connection);
        }

        public Task<bool> Exec(int frameId)
        {
            return service.RemoveSavedFrame(frameId);
        }
    }
}