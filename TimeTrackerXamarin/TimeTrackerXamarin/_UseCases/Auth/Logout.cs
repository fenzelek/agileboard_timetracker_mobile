using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Auth;

namespace TimeTrackerXamarin._UseCases.Auth
{
    public class Logout
    {
        private readonly IAuthService authService;

        public Logout(IAuthService authService)
        {
            this.authService = authService;
        }
        public Task Exec()
        {
            return authService.Logout();
        }
    }
}
