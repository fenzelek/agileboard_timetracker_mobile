using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTrackerXamarin._Domains.Companies;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Companies;

namespace TimeTrackerXamarin._UseCases.Companies
{
    public class GetCompanies
    {
        private readonly IFactory<ICompanyService> companyServiceFactory;
        private ICompanyService companyService;

        public GetCompanies(IFactory<ICompanyService> companyServiceFactory)
        {
            this.companyServiceFactory = companyServiceFactory;
        }

        public void SwitchConnection(bool connection)
        {
            companyService = companyServiceFactory.Create(connection);
        }

        public Task<List<Company>> GetAll()
        {
            return companyService.GetCompanies();
        }
    }
}