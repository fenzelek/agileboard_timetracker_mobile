using System.Threading.Tasks;
using Moq;
using TimeTrackerXamarin.Services;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;
using Xunit;

namespace TimeTrackerXamarin.Test.Unit.Services
{
    public class LocalizationTest
    {
        private readonly Mock<IGeolocation> geolocation;
        private readonly Localization localization;
        private readonly Mock<IPermissions> permissions;

        public LocalizationTest()
        {
            geolocation = new Mock<IGeolocation>();
            permissions = new Mock<IPermissions>();
            localization = new Localization(geolocation.Object, permissions.Object);
        }
        /*
        * @feature Services
        * @scenario Show localization
        * @case Localization is shown
        */
        [Fact(Skip = "Not work in portable ver")]
        public async void Get_Location()
        {
            //GIVEN
            var expectedLocation = new Location
            {
                Longitude = 1.234,
                Latitude = 1.234
            };
            permissions.Setup((p) => p.CheckStatusAsync<Permissions.LocationAlways>())
                .Returns(Task.FromResult(PermissionStatus.Granted));
            geolocation.Setup((g) => g.GetLocationAsync(It.IsAny<GeolocationRequest>())).Returns(Task.FromResult(expectedLocation));
            
            //WHEN
            var result = await localization.Get();

            //THEN
            Assert.IsType<Location>(result);
            Assert.Equal(expectedLocation.Longitude, (result as Location).Longitude);
            Assert.Equal(expectedLocation.Latitude, (result as Location).Latitude);
        }

        /*
        * @feature Services
        * @scenario Show localization fail
        * @case Localization is null
        */
        [Fact]
        public async void Get_null()
        {
            //GIVEN
            var expectedLocation = new Location
            {
                Longitude = 1.234,
                Latitude = 1.234
            };
            permissions.Setup((p) => p.CheckStatusAsync<Permissions.LocationAlways>())
                .Returns(Task.FromResult(PermissionStatus.Restricted));
            geolocation.Setup((g) => g.GetLastKnownLocationAsync()).Returns(Task.FromResult(expectedLocation));
            //WHEN
            var result = await localization.Get();
            //THEN
            Assert.Null(result);
        }

        /*
        * @feature Services
        * @scenario Parse localization
        * @case Parsed location to string format
        */
        [Fact]
        public void Parse_string()
        {
            //GIVEN
            var expectedLocation = new Location
            {
                Longitude = 1.2340,
                Latitude = 1.2340
            };
            string expectedString = "1,2340;1,2340";
            
            //WHEN
            var result = localization.Parse(expectedLocation);
            
            //THEN
            Assert.Equal(expectedString, result);
        }
        
        /*
        * @feature Services
        * @scenario Fail parse localization
        * @case Empty location string
        */
        [Fact]
        public void Parse_empty()
        {
            //GIVEN
            string expectedString = "";
            
            //WHEN
            var result = localization.Parse(null);
            
            //THEN
            Assert.Equal(expectedString, result);
        }
    }
}