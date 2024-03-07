using TimeTrackerXamarin._UseCases.Contracts;
using AVFoundation;
using CoreLocation;
using Foundation;
using Photos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Essentials;

#if __IOS__
using AddressBook;
using CoreMotion;
using Speech;
using EventKit;
using MediaPlayer;
#endif

namespace TimeTrackerXamarin.iOS.Services
{
    public class SettingsHelper:ISettingsHelper
    {
        public async void OpenSettings()
        {
            await Permissions.RequestAsync<Permissions.LocationAlways>();
            UIApplication.SharedApplication.OpenUrl(new NSUrl(UIApplication.OpenSettingsUrlString));
        }
    }
}