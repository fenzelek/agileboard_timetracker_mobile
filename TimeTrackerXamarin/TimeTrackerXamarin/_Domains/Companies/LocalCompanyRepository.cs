using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Companies;

namespace TimeTrackerXamarin._Domains.Companies
{
    public class LocalCompanyRepository : ICompanyRepository
    {
        private readonly ILocalCompanySource dataBase;

        public LocalCompanyRepository(ILocalCompanySource dataBase)
        {
            this.dataBase = dataBase;
        }

        public Task<List<Company>> GetAll()
        {
            return dataBase.GetCompanies();
        }
        
    }
}