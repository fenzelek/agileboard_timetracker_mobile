using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Auth;

namespace TimeTrackerXamarin._UseCases.Auth
{
    public class LoggedUser
    {
        private readonly IAuthService authService;

        public LoggedUser(IAuthService authService)
        {
            this.authService = authService;
        }
        public Task<User> Get()
        {
            return authService.Current();
        }
    }
}
