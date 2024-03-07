using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Auth;

namespace TimeTrackerXamarin._UseCases.Auth
{
    public class CheckLogin
    {
        private readonly IAuthService authService;

        public CheckLogin(IAuthService authService) 
        {
            this.authService = authService;
        }

        public Task<bool> Exec()
        {
            return authService.CheckLogin();
        }
    }
}
