using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeTrackerXamarin._UseCases.Contracts;
using Prism.Ioc;
using UIKit;
using System.Threading.Tasks;

namespace TimeTrackerXamarin.iOS.Services
{
    public class ForegroundServiceController : IForegroundServiceController
    {
        public bool IsEnabled()
        {
            return ContainerLocator.Container.Resolve<IForegroundIos>().IsTracking();
        }

        public async Task<bool> StartService(bool initialBlock = false)
        {
            ContainerLocator.Container.Resolve<IForegroundIos>().StartService();
            return true;
        }

        public async Task<bool> StopService()
        {
            ContainerLocator.Container.Resolve<IForegroundIos>().StopService();
            return true;
        }
    }
}