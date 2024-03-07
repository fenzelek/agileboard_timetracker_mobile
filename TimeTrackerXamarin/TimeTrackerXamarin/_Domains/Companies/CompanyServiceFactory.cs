using TimeTrackerXamarin._Domains.API;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Companies;

namespace TimeTrackerXamarin._Domains.Companies
{
    public class CompanyServiceFactory : IFactory<ICompanyService>
    {
        private readonly IRemoteCompanySource remoteSource;
        private readonly ILocalCompanySource localSource;
        
        public CompanyServiceFactory(IRemoteCompanySource remoteSource, ILocalCompanySource localSource)
        {
            this.localSource = localSource;
            this.remoteSource = remoteSource;
        }
        
        private ICompanyRepository CreateRepository(bool connection)
        {
            if (connection)
            {
                return new RemoteCompanyRepository(remoteSource, localSource);
            }
            return new LocalCompanyRepository(localSource);
        }

        public ICompanyService Create(bool connection)
        {
            var repository = CreateRepository(connection);
            return new CompanyService(repository);
        }
        
    }
}