using Xamarin.Forms;

namespace TimeTrackerXamarin.Views
{
    public class ThemeAwareNavigationPage : NavigationPage
    {
        public ThemeAwareNavigationPage()
        {
            SetDynamicResource(BackgroundColorProperty, "BackgroundColor");
            SetDynamicResource(BarBackgroundColorProperty, "BarBackgroundColor");
            SetDynamicResource(BarTextColorProperty, "BarTextColor");
        }

        public ThemeAwareNavigationPage(Page root) : base(root)
        {
            //if its not there, we get white flash on navigation when dark mode 
            SetDynamicResource(BackgroundColorProperty, "BackgroundColor");
            SetDynamicResource(BarBackgroundColorProperty, "BarBackgroundColor");
            SetDynamicResource(BarTextColorProperty, "BarTextColor");
        }
    }
}