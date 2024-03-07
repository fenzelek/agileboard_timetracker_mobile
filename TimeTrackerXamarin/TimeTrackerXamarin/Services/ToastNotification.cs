using Acr.UserDialogs;
using TimeTrackerXamarin._UseCases.Contracts;
using Xamarin.Forms;

namespace TimeTrackerXamarin.Services
{
    public class ToastNotification : IToastNotification
    {
        private ToastConfig toastConfig;
        public void ShowError(string msg)
        {
            toastConfig = new ToastConfig(msg)
            {
                BackgroundColor = Color.FromHex("#dc3545"),
                MessageTextColor = Color.WhiteSmoke,
            };
            UserDialogs.Instance.Toast(toastConfig);
        }

        public void ShowInfo(string msg)
        {
            toastConfig = new ToastConfig(msg)
            {
                BackgroundColor = Color.FromHex("#17a2b8"),
                MessageTextColor = Color.WhiteSmoke,
            };
            UserDialogs.Instance.Toast(toastConfig);
        }

        public void ShowWarn(string msg)
        {
            toastConfig = new ToastConfig(msg)
            {
                BackgroundColor = Color.FromHex("#ffc107"),
                MessageTextColor = Color.WhiteSmoke,
            };
            UserDialogs.Instance.Toast(toastConfig);
        }
    }
}
