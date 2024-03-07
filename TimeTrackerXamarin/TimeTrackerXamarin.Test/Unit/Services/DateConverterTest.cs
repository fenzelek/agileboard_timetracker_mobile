using TimeTrackerXamarin.Services;
using Xunit;

namespace TimeTrackerXamarin.Test.Unit.Services
{
    public class DateConverterTest
    {
        /*
         * @feature Services
         * @scenario Convert timestamp to time
         * @case Return time string
         */
        [Fact]
        public void ConvertTest()
        {
            var dateConverter = new DateConverter();            
            object result = dateConverter.Convert((long)3661, null, null, null);
            Assert.Equal("01:01:01", result.ToString());
        }
    }
}