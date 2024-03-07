using System;
using Moq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace TimeTrackerXamarin.Test.Mock
{
    public static class XamarinFormsMock
    {

        public static void Init()
        {
            Device.PlatformServices = new MockPlatformServices();
            Device.Info = new MockDeviceInfo();
            DependencyService.Register<MockResourceProvider>();
            DependencyService.Register<MockSerializer>();
        }
        
    }
}