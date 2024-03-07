using System.Threading.Tasks;
using Xamarin.Essentials;

namespace TimeTrackerXamarin._UseCases.Contracts
{
    public interface ILocalization
    {
        Task<Location> Get();
        string Parse(Location position);
    }
}
