using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace TimeTrackerXamarin.Test.Mock
{
    public class MockResourceProvider : ISystemResourcesProvider
    {
        private readonly ResourceDictionary dictionary = new ResourceDictionary();
        
        public IResourceDictionary GetSystemResources()
        {
            return dictionary;
        }
    }
}