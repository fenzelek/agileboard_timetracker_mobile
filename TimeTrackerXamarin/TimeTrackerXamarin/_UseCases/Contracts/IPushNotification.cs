using System.Threading.Tasks;

namespace TimeTrackerXamarin._UseCases.Contracts
{
    public interface IPushNotification
    {
        Task<bool> Create(int id, string subtitle, string content);
        void Delete(int id);       
    }
}
