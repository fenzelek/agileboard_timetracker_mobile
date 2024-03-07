using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTrackerXamarin._Domains.API;
using TimeTrackerXamarin._Domains.Projects;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Companies;

namespace TimeTrackerXamarin._Domains.Companies
{
    public class RemoteCompanyRepository : ICompanyRepository
    {

        private readonly IRemoteCompanySource remoteSource;
        private readonly ILocalCompanySource localSource;

        public RemoteCompanyRepository(IRemoteCompanySource remoteSource, ILocalCompanySource localSource)
        {
            this.remoteSource = remoteSource;
            this.localSource = localSource;
        }

        public async Task<List<Company>> GetAll()
        {
            var companies = await remoteSource.GetAll();
            await localSource.SaveCompanies(companies);
            return companies;
        }
        
    }
}