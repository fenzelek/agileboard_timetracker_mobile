using System.Collections.Generic;
using System.Threading.Tasks;

namespace TimeTrackerXamarin._UseCases.Contracts.Companies
{
    public interface ICompanyService
    {
        Task<List<Company>> GetCompanies();
    }
}