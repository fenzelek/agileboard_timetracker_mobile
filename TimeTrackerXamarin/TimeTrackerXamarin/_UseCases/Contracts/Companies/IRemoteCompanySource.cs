using System.Collections.Generic;
using System.Threading.Tasks;
using Company = TimeTrackerXamarin._UseCases.Contracts.Companies.Company;

namespace TimeTrackerXamarin._UseCases.Contracts.Companies
{
    public interface IRemoteCompanySource
    {
        Task<List<Company>> GetAll();
    }
}