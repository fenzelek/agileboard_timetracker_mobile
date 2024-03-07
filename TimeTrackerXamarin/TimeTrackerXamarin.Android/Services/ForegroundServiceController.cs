using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts;
using Activity = Android.App.Activity;
using Context = Android.Content.Context;

namespace TimeTrackerXamarin.Droid.Services
{
    public class ForegroundServiceController : IForegroundServiceController
    {
        private Context context = Android.App.Application.Context;
        
        private Intent intent;
        public ForegroundServiceController()
        {
            intent = new Intent(context, typeof(ForegroundService));
        }
        

        public bool IsEnabled()
        {
            ActivityManager manager = (ActivityManager)context.GetSystemService(Context.ActivityService);
            foreach (var service in manager.GetRunningServices(int.MaxValue))
            {
                if (service.Service.ClassName.Equals(Java.Lang.Class.FromType(typeof(ForegroundService)).CanonicalName))
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> StartService(bool blockSending = false)
        {
            if (IsEnabled()) return true;
            intent.PutExtra("initialBlockState", blockSending);
            context.StartService(intent);
            return IsEnabled();
        }

        public async Task<bool> StopService()
        {
            if (!IsEnabled()) return false;
            context.StopService(intent);
            return !IsEnabled();
        }
    }
}