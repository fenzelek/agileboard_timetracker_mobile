namespace TimeTrackerXamarin._UseCases.Contracts
{
    public interface IToastNotification
    {
        void ShowInfo(string msg);
        void ShowWarn(string msg);
        void ShowError(string msg);
    }
}
