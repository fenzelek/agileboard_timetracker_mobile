using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http.Configuration;
using Xamarin.Forms.Internals;

namespace TimeTrackerXamarin.Test.Mock
{
    public class MockSerializer : IDeserializer
    {
        private IDictionary<string, object> properties = new Dictionary<string, object>();
        
        public async Task<IDictionary<string, object>> DeserializePropertiesAsync()
        {
            return properties;
        }

        public async Task SerializePropertiesAsync(IDictionary<string, object> properties)
        {
            this.properties = properties;
        }
    }
}