using System.Threading.Tasks;
using Xamarin.Forms;

namespace TimeTrackerXamarin._UseCases.Contracts
{
    public interface IDatabaseFlushService
    {
        Task Flush();
    }
}