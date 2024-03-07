using System.Collections.Generic;
using System.Threading.Tasks;

namespace TimeTrackerXamarin._UseCases.Contracts.Projects
{
    public interface IRemoteProjectSource
    {
        Task<List<Project>> GetAll(int companyId);
    }
}