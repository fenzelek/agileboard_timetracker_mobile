using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace TimeTrackerXamarin.iOS.Services
{
    public interface IForegroundIos
    {
        void StartService();
        void StopService();
        bool IsTracking();
    }
}