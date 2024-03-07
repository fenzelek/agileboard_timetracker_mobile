using System;
using TimeTrackerXamarin.iOS.Renderers;
using UIKit;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Color = System.Drawing.Color;

[assembly: ResolutionGroupName("PlainEntryGroup")]
[assembly: ExportEffect(typeof(IOSPlainEntryEffect), "PlainEntryEffect")]
namespace TimeTrackerXamarin.iOS.Renderers
{
    public class IOSPlainEntryEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                if (Control is UITextField field)
                {
                    field.Layer.BorderWidth = 0;
                    field.BorderStyle = UITextBorderStyle.None;
                }
            } catch (Exception ex) {
                Console.WriteLine ("Cannot set property on attached control. Error: ", ex.Message);
            }
        }

        protected override void OnDetached()
        {
        }
    }
}