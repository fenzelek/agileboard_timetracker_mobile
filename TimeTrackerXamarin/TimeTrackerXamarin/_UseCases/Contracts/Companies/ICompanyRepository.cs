using System.Collections.Generic;
using System.Threading.Tasks;

namespace TimeTrackerXamarin._UseCases.Contracts.Companies
{
    public interface ICompanyRepository
    {
        Task<List<Company>> GetAll();
    }
}
