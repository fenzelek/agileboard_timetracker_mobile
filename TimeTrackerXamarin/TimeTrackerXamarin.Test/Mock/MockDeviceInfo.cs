using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace TimeTrackerXamarin.Test.Mock
{
    public class MockDeviceInfo : DeviceInfo
    {
        public override Size PixelScreenSize { get; }
        public override Size ScaledScreenSize { get; }
        public override double ScalingFactor { get; }
    }
}