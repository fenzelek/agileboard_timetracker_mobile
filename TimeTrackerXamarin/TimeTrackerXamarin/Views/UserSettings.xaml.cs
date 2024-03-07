using TimeTrackerXamarin.Config;
using Xamarin.Forms;

namespace TimeTrackerXamarin.Views
{
    public partial class UserSettings : ContentPage
    {
        public UserSettings(IConfiguration configuration)
        {
            InitializeComponent();
            if (!configuration.IsDebug) return;
            var text = new Label
            {
                Text = "This is debug version of the app!",
                TextColor = Color.Red,
                FontAttributes = FontAttributes.Bold
            };
            var debugStack = new StackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                Children = { text }
            };

            SettingsStack.Children.Add(debugStack);
        }
    }
}
