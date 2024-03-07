using TimeTrackerXamarin._UseCases.Companies;

namespace TimeTrackerXamarin._UseCases.Contracts
{
    public interface ITokenService
    {
        string Get();
        void Set(string token);
        void Remove();
        bool Refresh();
    }
}