using System;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace TimeTrackerXamarin.Services
{
    public class Localization : ILocalization
    {
        private readonly IGeolocation geolocation;
        private readonly IPermissions permissions;

        public Localization(IGeolocation geolocation, IPermissions permissions)
        {
            this.geolocation = geolocation;
            this.permissions = permissions;
        }

        public async Task<Location> Get()
        {           
            try
            {

                if (permissions.CheckStatusAsync<Permissions.LocationAlways>().Result != PermissionStatus.Granted)
                {
                    return null;
                }

                Location location = null;
                if (Device.RuntimePlatform == Device.Android)
                {
                    try
                    {
                        location = await geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Low, TimeSpan.FromSeconds(5)));
                    }
                    catch (Exception e)
                    {
                        //ignored
                    }
                }

                if (location == null)
                {
                    location = await geolocation.GetLastKnownLocationAsync();   
                }

                return location;
            }
            catch (Exception ex)
            {                
                return null;
            }            
        }

        public string Parse(Location position)
        {
            if(position == null) return "";
            var location = position as Location;
            string latitude = String.Format("{0:0.0000}", location.Latitude);
            string longitude = String.Format("{0:0.0000}", location.Longitude);
            var parsed = $"{latitude};{longitude}";
            //"gpsPosition": "latitude = 49.1234567, longitude = 47.1234567"

            return parsed;
        }
    }
}
