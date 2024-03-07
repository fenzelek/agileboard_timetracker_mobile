using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TimeTrackerXamarin._UseCases.Contracts.Companies
{
    public interface ILocalCompanySource
    {
        Task<List<Company>> GetCompanies();
        Task<bool> SaveCompanies(List<Company> companies);
    }
}
